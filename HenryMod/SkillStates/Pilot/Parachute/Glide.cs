using MoffeinPilot.Content.Components;
using RoR2;
using UnityEngine;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class Glide : BaseState
    {
        public static float maxFallVelocity = -6f;
        public static float exitHopVelocity = 17f;
        public static GameObject jumpEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact");

        private PilotController pilotController;
        private int origJumpCount;
        private bool jumpReleased = false;
        public override void OnEnter()
        {
            base.OnEnter();

            pilotController = base.GetComponent<PilotController>();
            if (pilotController)
            {
                pilotController.isParachuting = true;
            }
            if (base.characterMotor)
            {
                origJumpCount = base.characterMotor.jumpCount;
                base.characterMotor.jumpCount = base.characterBody ? base.characterBody.maxJumpCount : 1;
            }
        }

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
                bool jumped = false;
                if (base.inputBank)
                {
                    if (!jumpReleased) jumpReleased = !base.inputBank.jump.down;
                    jumped = jumpReleased && base.inputBank.jump.down;
                }
                

                if (isGrounded || jumped)
                {
                    if (jumped)
                    {
                        base.SmallHop(base.characterMotor, Glide.exitHopVelocity);
                        if (base.characterBody) EffectManager.SpawnEffect(jumpEffect, new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            scale = base.characterBody.radius
                        }, true);
                    }
                    this.outer.SetNextStateToMain();
                    return;
                }

            }
        }

        public override void OnExit()
        {
            if (base.characterMotor)
            {
                if (base.characterMotor.isGrounded)
                    base.characterMotor.jumpCount = 0;
                else
                    base.characterMotor.jumpCount = Mathf.Max(origJumpCount, 1);
            }
            if (pilotController)
            {
                pilotController.isParachuting = false;
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}