using Pilot.Content.Components;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Weapon
{
    public class RapidFire : BaseState
	{
		//Railgunner 300 for 5 shots per second
		public static float selfKnockbackForce = 0f;

		public static float damageCoefficient = 1f;
		public static float force = 80f;
		public static float baseDuration = 0.12f;
        public static float spreadBloomValue = 0.5f;
		public static float recoilAmplitude = 1f;
		public static string attackSoundString = "Play_Pilot_Primary_Rapid";
		public static string muzzleName = "";
		public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoShotgun.prefab").WaitForCompletion();
		public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();
		public static GameObject muzzleEffectPrefab;

		private PilotController pilotController;

		private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
			pilotController = base.GetComponent<PilotController>();

			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 3f, false);
			duration = baseDuration/this.attackSpeedStat;
			if (muzzleEffectPrefab)
            {
				EffectManager.SimpleMuzzleFlash(RapidFire.muzzleEffectPrefab, base.gameObject, RapidFire.muzzleName, false);
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
					maxSpread = base.characterBody.spreadBloomAngle,
					bulletCount = 1u,
					damage = RapidFire.damageCoefficient * this.damageStat,
					force = RapidFire.force,
					tracerEffectPrefab = RapidFire.tracerEffectPrefab,
					muzzleName = RapidFire.muzzleName,
					hitEffectPrefab = RapidFire.hitEffectPrefab,
					isCrit = base.RollCrit(),
					radius = 0.2f,
					smartCollision = true,
					damageType = DamageType.Generic,
					falloffModel = BulletAttack.FalloffModel.DefaultBullet,
					procCoefficient = 1f
				}.Fire();
				if (RapidFire.selfKnockbackForce != 0f//pilotController && pilotController.isParachuting && 
					&& base.characterBody.characterMotor && !base.characterMotor.isGrounded && base.characterBody.characterMotor.velocity != Vector3.zero)
					base.characterBody.characterMotor.ApplyForce(-RapidFire.selfKnockbackForce * aimRay.direction, false, false);
			}
			base.AddRecoil(-0.4f * RapidFire.recoilAmplitude, -0.8f * RapidFire.recoilAmplitude, -0.3f * RapidFire.recoilAmplitude, 0.3f * RapidFire.recoilAmplitude);
			Util.PlaySound(RapidFire.attackSoundString, base.gameObject);
			base.characterBody.AddSpreadBloom(RapidFire.spreadBloomValue);
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
			return InterruptPriority.Skill;
        }
    }
}
