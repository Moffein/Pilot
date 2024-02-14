using EntityStates.MoffeinPilot.Airstrike;
using MoffeinPilot.Content.Components;
using MoffeinPilot.Modules;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.BulletAttack;

namespace EntityStates.MoffeinPilot.Weapon
{
    public class ClusterFire : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public static float spreadBloomValueCosmetic = 1f; //Show crosshair spread but keep it perfectly accurate
        public static float recoilAmplitude = 1f;
        public static float comboRecoilAmplitude = 2f;

        public static float damageCoefficient = 1.6f;
        public static float force = 500f;
        
        public static float comboDamageCoefficient = 3.2f;
        public static float comboForce = 1500f;
        public static float comboBlastRadius = 11f;

        public static float comboMinDistance = 1.5f;    //Distance where blast radius stays at minimum
        public static float comboMinBlastRadius = 3f;   //Blast Radius at min distance

        //Railgunner 300 for 5 shots per second
        public static float selfKnockbackForce = 0f;
        public static float comboSelfKnockbackForce = 0f;

        public static float shotRadius = 0.5f;
        public static float comboShotRadius = 0.5f;

        public static float baseDuration = 0.3f;

        public static string muzzleName = "Muzzle";
        public static string attackSoundString = "Play_MoffeinPilot_Primary_Cluster";
        public static string comboAttackSoundString = "Play_MoffeinPilot_Primary_Cluster_Combo";

        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/TracerCaptainShotgun.prefab").WaitForCompletion();
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/HitsparkCaptainShotgun.prefab").WaitForCompletion();

        public static GameObject comboExplosionEffectPrefab = global::MoffeinPilot.Modules.Assets.ExplosionGolemButScale;
        public static GameObject comboTracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/CaptainDefenseMatrix/TracerCaptainDefenseMatrix.prefab").WaitForCompletion();
        public static GameObject comboHitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/HitsparkCaptainShotgun.prefab").WaitForCompletion();
        public static GameObject muzzleEffectPrefab, comboMuzzleEffectPrefab;

        private PilotController pilotController;
        private bool triggeredComboExplosion;

        private float duration;
        private int step;
        public void SetStep(int i)
        {
            this.step = i;
        }

		public override void OnEnter()
		{
			base.OnEnter();

            triggeredComboExplosion = false;
            pilotController = base.GetComponent<PilotController>();

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 3f, false);
            duration = baseDuration / this.attackSpeedStat;
			base.characterBody.AddSpreadBloom(RapidFire.spreadBloomValue);
            base.PlayAnimation("Gesture, Additive", "Shoot1", "ShootGun.playbackRate", duration * 2f);
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
                if (ClusterFire.selfKnockbackForce != 0f//pilotController && pilotController.isParachuting && 
                    && base.characterMotor && !base.characterMotor.isGrounded && base.characterMotor.velocity != Vector3.zero)
                    base.characterMotor.ApplyForce(-ClusterFire.selfKnockbackForce * aimRay.direction, false, false);
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
            if (base.isAuthority)
            {
                new BulletAttack
                {
                    tracerEffectPrefab = ClusterFire.comboTracerEffectPrefab,
                    damage = 0f,
                    procCoefficient = 0f,
                    damageType = DamageType.Silent | DamageType.NonLethal,
                    owner = base.gameObject,
                    aimVector = aimRay.direction,
                    isCrit = false,
                    minSpread = 0f,
                    maxSpread = 0f,
                    origin = aimRay.origin,
                    maxDistance = 2000f,
                    muzzleName = ClusterFire.muzzleName,
                    radius = ClusterFire.shotRadius,
                    hitCallback = ComboHitCallback
                }.Fire();

                if (ClusterFire.comboSelfKnockbackForce != 0f   //pilotController && pilotController.isParachuting && 
                    && base.characterMotor && !base.characterMotor.isGrounded && base.characterMotor.velocity != Vector3.zero)
                    base.characterMotor.ApplyForce(-ClusterFire.comboSelfKnockbackForce * aimRay.direction, false, false);
            }

            //This is intentional, need to play both to make it sound right.
            Util.PlaySound(ClusterFire.attackSoundString, base.gameObject);
            Util.PlaySound(ClusterFire.comboAttackSoundString, base.gameObject);

            base.AddRecoil(-0.4f * ClusterFire.comboRecoilAmplitude, -0.8f * ClusterFire.comboRecoilAmplitude, -0.3f * ClusterFire.comboRecoilAmplitude, 0.3f * ClusterFire.comboRecoilAmplitude);
            base.characterBody.AddSpreadBloom(ClusterFire.spreadBloomValueCosmetic);
        }

        private bool ComboHitCallback(BulletAttack bulletRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && !triggeredComboExplosion && base.characterBody)
            {
                triggeredComboExplosion = true;

                Vector3 attackForce = bulletRef.aimVector != null ? ClusterFire.comboForce * bulletRef.aimVector.normalized : Vector3.zero;

                float hitDistance = (hitInfo.point - base.characterBody.corePosition).magnitude;
                float calcRadius = comboBlastRadius;
                if (hitDistance < comboBlastRadius)
                {
                    if (hitDistance <= comboMinDistance)
                    {
                        calcRadius = comboMinBlastRadius;
                    }
                    else
                    {
                        calcRadius = Mathf.Lerp(comboMinBlastRadius, comboBlastRadius, (hitDistance - comboMinDistance) / (comboBlastRadius - comboMinDistance));
                    }
                }

                //golem explosion effect was definitely not set up to scale 1:1 with radius
                if (ClusterFire.comboExplosionEffectPrefab) EffectManager.SpawnEffect(ClusterFire.comboExplosionEffectPrefab, new EffectData { origin =  hitInfo.point, scale = calcRadius / 4.83f }, true);
                new BlastAttack()
                {
                    attacker = base.gameObject,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    baseDamage = this.damageStat * ClusterFire.comboDamageCoefficient,
                    baseForce = 0f,
                    bonusForce = attackForce,
                    canRejectForce = true,
                    crit = base.RollCrit(),
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BlastAttack.FalloffModel.None,
                    inflictor = base.gameObject,
                    position = hitInfo.point,
                    procChainMask = default,
                    procCoefficient = 1f,
                    radius = calcRadius,
                    teamIndex = base.GetTeam()
                }.Fire();
            }

            return false;
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
