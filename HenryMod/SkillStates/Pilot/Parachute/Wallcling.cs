using EntityStates.MoffeinPilot.Weapon;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class Wallcling : BaseState
    {
        public static float jumpExitSpeed = 12f;
        public static float jumpExitVerticalMult = 0.7f;
        public static string entrySoundString = "Play_loader_m2_impact";
        public static string jumpExitSoundString = "Play_loader_m1_swing";
        public static SkillDef utilityOverride;

        private GenericSkill overriddenSkill;
        private bool jumpExit = false;
        private Vector3 initialPosition;
        public Vector3 clingPoint;
        public Vector3 clingStartDirection;

        public override void OnEnter()
        {
            base.OnEnter();

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
                    Util.PlaySound(jumpExitSoundString, base.gameObject);
                    if (base.characterMotor)
                    {
                        base.characterMotor.disableAirControlUntilCollision = false;
                        base.characterMotor.jumpCount = base.characterMotor.isGrounded ? 0 : 1;
                    }

                    Vector3 exitDirection = base.GetAimRay().direction;
                    

                    this.outer.SetNextState(new WavedashWallcling()
                    {
                        entryDirection = exitDirection,
                        clingStartDirection = this.clingStartDirection,
                        initialSpeed = jumpExitSpeed * this.moveSpeedStat
                    });
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
