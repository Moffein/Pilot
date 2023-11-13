using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Weapon
{
    public class RapidFire : BaseState
	{
		public static float damageCoefficient = 0.75f;
		public static float force = 75f;
		public static float baseDuration = 0.15f;
        public static float spreadBloomValue = 0.2f;
		public static string attackSoundString = "Play_commando_M1";
		public static string muzzleName = "";
		public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoShotgun.prefab").WaitForCompletion();
		public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();

		private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

			Ray aimRay = base.GetAimRay();
            duration = baseDuration/this.attackSpeedStat;
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
					damageType = DamageType.Stun1s,
					falloffModel = BulletAttack.FalloffModel.None,
					procCoefficient = 0.5f
				}.Fire();
			}
			Util.PlaySound(RapidFire.attackSoundString, base.gameObject);
			base.characterBody.AddSpreadBloom(RapidFire.spreadBloomValue);
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
