using RoR2;
using RoR2.Skills;

namespace EntityStates.Pilot.Parachute
{
    public class Wallbounce : BaseState
    {
        public static string soundString = "Play_loader_m2_impact";
        public static float baseDuration = 4f;
        public static float smallHopVelocity = 17f;
        //public static float minFallVelocity = -3f;

        public static SkillDef utilityOverride;
        private GenericSkill overriddenSkill;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound(soundString, base.gameObject);

            if (base.isAuthority && base.characterMotor)
            {
                base.SmallHop(base.characterMotor, Wallbounce.smallHopVelocity);
            }

            SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.utility : null;
            if (genericSkill)
            {
                this.TryOverrideSkill(genericSkill);
                genericSkill.onSkillChanged += this.TryOverrideSkill;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            //if (base.characterMotor && base.characterMotor.velocity.y < Wallbounce.minFallVelocity) base.characterMotor.velocity.y = Wallbounce.minFallVelocity;

            bool outOfStock = overriddenSkill && !(base.skillLocator && base.skillLocator.utility && base.skillLocator.utility.stock > 0);

            if (outOfStock || base.fixedAge >= Wallbounce.baseDuration || (base.characterMotor && base.characterMotor.isGrounded))
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.utility : null;
            if (genericSkill)
            {
                genericSkill.onSkillChanged -= this.TryOverrideSkill;
            }
            if (this.overriddenSkill)
            {
                this.overriddenSkill.UnsetSkillOverride(this, Wallbounce.utilityOverride, GenericSkill.SkillOverridePriority.Contextual);
            }

            base.OnExit();
        }

        //Copied from Railgunner
        private void TryOverrideSkill(GenericSkill skill)
        {
            if (skill && !this.overriddenSkill && !skill.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual))
            {
                this.overriddenSkill = skill;
                this.overriddenSkill.SetSkillOverride(this, Wallbounce.utilityOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = 1;
            }
        }
    }
}
