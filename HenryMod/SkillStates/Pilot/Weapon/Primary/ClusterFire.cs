using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Weapon
{
    public class ClusterFire : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public static float spreadBloomValueCosmetic = 1f; //Show crosshair spread but keep it perfectly accurate
        public static float recoilAmplitude = 1f;
        public static float comboRecoilAmplitude = 2f;

        public static float damageCoefficient = 1f;
        public static float force = 100f;

        public static float comboDamageCoefficient = 2f;
        public static float comboForce = 200f;

        public static float shotRadius = 0.5f;
        public static float comboShotRadius = 1f;

        public static float baseDuration = 0.3f;

        public static string muzzleName = "";
        public static string attackSoundString = "Play_commando_M1";
        public static string comboAttackSoundString = "Play_bandit2_R_fire";

        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/TracerCaptainShotgun.prefab").WaitForCompletion();
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/HitsparkCaptainShotgun.prefab").WaitForCompletion();

        public static GameObject comboTracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/TracerGolem.prefab").WaitForCompletion();
        public static GameObject comboHitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion();

        public static GameObject muzzleEffectPrefab, comboMuzzleEffectPrefab;

        private float duration;
        private int step;
        public void SetStep(int i)
        {
            this.step = i;
        }

		public override void OnEnter()
		{
			base.OnEnter();

			Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 3f, false);
            duration = baseDuration / this.attackSpeedStat;
			base.characterBody.AddSpreadBloom(RapidFire.spreadBloomValue);

            if (this.step == 2)
            {
                FireBulletCombo(aimRay);
            }
            else
            {
                FireBullet(aimRay);
            }
		}

        public void FireBullet(Ray aimRay)
        {
            if (muzzleEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(ClusterFire.muzzleEffectPrefab, base.gameObject, RapidFire.muzzleName, false);
            }
            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1u,
                    damage = ClusterFire.damageCoefficient * this.damageStat,
                    force = ClusterFire.force,
                    tracerEffectPrefab = ClusterFire.tracerEffectPrefab,
                    muzzleName = ClusterFire.muzzleName,
                    hitEffectPrefab = ClusterFire.hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    radius = ClusterFire.shotRadius,
                    smartCollision = true,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.None,
                    procCoefficient = 1f
                }.Fire();
            }
            Util.PlaySound(ClusterFire.attackSoundString, base.gameObject);
            base.AddRecoil(-0.4f * ClusterFire.recoilAmplitude, -0.8f * ClusterFire.recoilAmplitude, -0.3f * ClusterFire.recoilAmplitude, 0.3f * ClusterFire.recoilAmplitude);
            base.characterBody.AddSpreadBloom(ClusterFire.spreadBloomValueCosmetic);
        }

        public void FireBulletCombo(Ray aimRay)
        {
            if (comboMuzzleEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(ClusterFire.comboMuzzleEffectPrefab, base.gameObject, RapidFire.muzzleName, false);
            }
            new BulletAttack
            {
                owner = base.gameObject,
                weapon = base.gameObject,
                origin = aimRay.origin,
                aimVector = aimRay.direction,
                minSpread = 0f,
                maxSpread = 0f,
                bulletCount = 1u,
                damage = ClusterFire.comboDamageCoefficient * this.damageStat,
                force = ClusterFire.comboForce,
                tracerEffectPrefab = ClusterFire.comboTracerEffectPrefab,
                muzzleName = ClusterFire.muzzleName,
                hitEffectPrefab = ClusterFire.comboHitEffectPrefab,
                isCrit = base.RollCrit(),
                radius = ClusterFire.comboShotRadius,
                smartCollision = true,
                damageType = DamageType.Generic,
                falloffModel = BulletAttack.FalloffModel.None,
                procCoefficient = 1f,
                stopperMask = LayerIndex.world.mask
            }.Fire();
            Util.PlaySound(ClusterFire.comboAttackSoundString, base.gameObject);
            base.AddRecoil(-0.4f * ClusterFire.comboRecoilAmplitude, -0.8f * ClusterFire.comboRecoilAmplitude, -0.3f * ClusterFire.comboRecoilAmplitude, 0.3f * ClusterFire.comboRecoilAmplitude);
            base.characterBody.AddSpreadBloom(ClusterFire.spreadBloomValueCosmetic);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
