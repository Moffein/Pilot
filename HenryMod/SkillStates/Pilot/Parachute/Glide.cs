using RoR2;
using UnityEngine;

namespace EntityStates.Pilot.Parachute
{
    public class Glide : BaseState
    {
        public static float maxFallVelocity = -4f;
        public static float exitHopVelocity = 17f;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    if (base.characterMotor.velocity.y <= Glide.maxFallVelocity) base.characterMotor.velocity.y = Glide.maxFallVelocity;
                }

                bool isGrounded = base.characterMotor && base.characterMotor.isGrounded;
                bool jumped = base.inputBank && base.inputBank.jump.down;
                if (isGrounded || jumped)
                {
                    if (jumped)
                    {
                        base.SmallHop(base.characterMotor, Glide.exitHopVelocity);
                    }
                    this.outer.SetNextStateToMain();
                    return;
                }

            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}