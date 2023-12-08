using EntityStates.MoffeinPilot.Weapon;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class Wallcling : BaseState
    {
        public static float jumpExitForce = 7000f;
        public static float jumpExitVerticalDistanceMult = 0.5f;
        public static string entrySoundString = "Play_loader_m2_impact";
        public static string jumpExitSoundString = "Play_loader_m1_swing";
        public static SkillDef utilityOverride;

        private GenericSkill overriddenSkill;
        private bool jumpExit = false;
        private Vector3 initialPosition;
        public Vector3 clingPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            if (base.characterBody && base.characterBody.isSprinting) base.characterBody.isSprinting = false;

            Util.PlaySound(entrySoundString, base.gameObject);

            initialPosition = base.transform.position;

            if (base.characterMotor)
            {
                base.characterMotor.jumpCount = base.characterBody ? base.characterBody.maxJumpCount : 1;
                if (base.isAuthority)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterMotor.rootMotion = Vector3.zero;
                    base.characterMotor.disableAirControlUntilCollision = true;
                }
            }

            /*SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.utility : null;
            if (genericSkill)
            {
                this.TryOverrideSkill(genericSkill);
                genericSkill.onSkillChanged += this.TryOverrideSkill;
            }*/
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //Works but the mushroom ward spawns on the ground, jank.
            //if (NetworkServer.active && base.characterBody && base.characterBody.notMovingStopwatch < 1f) base.characterBody.notMovingStopwatch = 1f;

            if (base.characterBody && base.characterBody.isSprinting) base.characterBody.isSprinting = false;

            if (base.isAuthority)
            {
                bool isGrounded = false;
                if (base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterMotor.rootMotion = Vector3.zero;
                    isGrounded = base.characterMotor.isGrounded;

                    base.characterMotor.Motor.SetPosition(initialPosition);
                }

                if (!jumpExit && (base.inputBank && base.inputBank.jump.down))
                {
                    jumpExit = true;
                }

                //bool outOfStock = overriddenSkill && !(base.skillLocator && base.skillLocator.utility && base.skillLocator.utility.stock > 0);
                if (isGrounded) //|| outOfStock
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
                else if (jumpExit)
                {
                    this.outer.SetNextState(new AirSpeedBoost());
                    return;
                }
            }
        }
        public override void OnExit()
        {
            /*SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.utility : null;
            if (genericSkill)
            {
                genericSkill.onSkillChanged -= this.TryOverrideSkill;
            }
            if (this.overriddenSkill)
            {
                this.overriddenSkill.UnsetSkillOverride(this, Wallbounce.utilityOverride, GenericSkill.SkillOverridePriority.Contextual);
            }*/
            if (base.characterBody && !base.characterBody.isSprinting) base.characterBody.isSprinting = true;

            if (base.characterMotor)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
                base.characterMotor.jumpCount = base.characterMotor.isGrounded ? 0 : 1;

                if (base.isAuthority && jumpExit)
                {
                    Util.PlaySound(jumpExitSoundString, base.gameObject);

                    Vector3 jumpForce = base.GetAimRay().direction;
                    if (jumpForce.y < 0.25f) jumpForce.y = 0.25f;
                    jumpForce.Normalize();
                    jumpForce *= Wallcling.jumpExitForce;
                    jumpForce.y *= Wallcling.jumpExitVerticalDistanceMult;

                    base.characterMotor.ApplyForce(jumpForce, true, false);
                }
            }

            base.OnExit();
        }

        //Copied from Railgunner
        private void TryOverrideSkill(GenericSkill skill)
        {
            if (skill && !this.overriddenSkill && !skill.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual))
            {
                this.overriddenSkill = skill;
                this.overriddenSkill.SetSkillOverride(this, Wallcling.utilityOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = 1;
            }
        }
    }
}
