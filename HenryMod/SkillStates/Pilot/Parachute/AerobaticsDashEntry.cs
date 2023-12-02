using EntityStates.Pilot.Airstrike;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.BulletAttack;

namespace EntityStates.Pilot.Parachute
{
    public class AerobaticsDashEntry : AerobaticsDashBase
    {
        public static float minDurationBeforeWallbounce = 0.06f;

        private bool detectedWallbounce = false;
        private bool triggeredWallbounce = false;

        protected override void CheckStateExit()
        {
            if (!base.isAuthority) return;
            if (base.fixedAge >= AerobaticsDashBase.baseDuration)
            {
                CheckWallBounceExit();
                return;
            }
        }

        public override void FixedUpdate()
        {
            if (base.isAuthority)
            {
                if (AttemptTriggerWallbounce()) return;
            }
            base.FixedUpdate();
        }

        private void CheckWallBounceExit()
        {
            if (!AttemptTriggerWallbounce())
                this.outer.SetNextStateToMain();
            return;
        }

        private bool AttemptTriggerWallbounce()
        {
            CheckWallBounce();
            if (detectedWallbounce && !triggeredWallbounce)
            {
                triggeredWallbounce = true;
                this.outer.SetNextState(new Wallbounce());
                return true;
            }

            return false;
        }

        private void CheckWallBounce()
        {
            if (detectedWallbounce || base.fixedAge < AerobaticsDashEntry.minDurationBeforeWallbounce || !base.characterBody) return;
            BulletAttack ForwardCheck = new BulletAttack
            {
                tracerEffectPrefab = null,
                damage = 0f,
                procCoefficient = 0.1f,
                damageType = DamageType.Silent | DamageType.NonLethal,
                owner = base.gameObject,
                aimVector = blinkVector,
                isCrit = false,
                minSpread = 0f,
                maxSpread = 0f,
                origin = base.characterBody.corePosition,
                maxDistance = base.characterBody.radius,
                muzzleName = null,
                radius = base.characterBody.radius,
                hitCallback = CheckWallbounceHitCallback,
                stopperMask = LayerIndex.world.mask
            };
            ForwardCheck.Fire();
        }

        private bool CheckWallbounceHitCallback(BulletAttack bulletRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && !detectedWallbounce)
            {
                detectedWallbounce = true;
            }
            return BulletAttack.defaultHitCallback.Invoke(bulletRef, ref hitInfo);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
