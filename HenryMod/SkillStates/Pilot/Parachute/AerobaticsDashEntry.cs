using EntityStates.MoffeinPilot.Airstrike;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.BulletAttack;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class AerobaticsDashEntry : AerobaticsDashBase
    {
        public static float minDurationBeforeWallbounce = 0.06f;

        private bool detectedWallbounce = false;
        private bool triggeredWallbounce = false;

        private Vector3 forwardCheckDirection;
        private Vector3 clingPoint;

        protected override void CheckStateExit()
        {
            if (!base.isAuthority) return;
            if (base.fixedAge >= AerobaticsDashBase.baseDuration)
            {
                CheckWallBounceExit();
                return;
            }
        }

        public override void SetBlinkVector()
        {
            base.SetBlinkVector();

            forwardCheckDirection = blinkVector;
            forwardCheckDirection.y = 0f;
            forwardCheckDirection.Normalize();
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
            if (detectedWallbounce && !triggeredWallbounce && !(base.characterMotor && base.characterMotor.isGrounded))
            {
                triggeredWallbounce = true;
                this.outer.SetNextState(new Wallcling() { clingPoint = this.clingPoint, clingStartDirection = forwardCheckDirection });
                return true;
            }

            return false;
        }

        private void CheckWallBounce()
        {
            if (detectedWallbounce || base.fixedAge < AerobaticsDashEntry.minDurationBeforeWallbounce || !base.characterBody) return;
            
            //BodyRadius 0.5
            BulletAttack ForwardCheck = new BulletAttack
            {
                tracerEffectPrefab = null,
                damage = 0f,
                procCoefficient = 0f,
                damageType = DamageType.Silent | DamageType.NonLethal,
                owner = base.gameObject,
                aimVector = forwardCheckDirection,
                isCrit = false,
                minSpread = 0f,
                maxSpread = 0f,
                origin = base.characterBody.corePosition,
                maxDistance = 1f,
                muzzleName = null,
                radius = 0.5f,
                hitCallback = CheckWallbounceHitCallback,
                stopperMask = LayerIndex.world.mask
            };
            ForwardCheck.Fire();
        }

        private bool CheckWallbounceHitCallback(BulletAttack bulletRef, ref BulletHit hitInfo)
        {
            if (!detectedWallbounce && hitInfo.point != null && hitInfo.collider && hitInfo.collider.gameObject.layer == LayerIndex.world.intVal)
            {
                clingPoint = hitInfo.point;
                detectedWallbounce = true;
            }
            return false;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
