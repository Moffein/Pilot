using RoR2;
using RoR2.Skills;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.Pilot.FireSelect
{
    public class TargetAcquired : BaseState
    {
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerCrosshair.prefab").WaitForCompletion();
        public static SkillDef primaryOverride;

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private GenericSkill overriddenSkill;

        public override void OnEnter()
        {
            base.OnEnter();
            if (TargetAcquired.crosshairOverridePrefab)
            {
                this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, TargetAcquired.crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
            }
            SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                this.TryOverrideSkill(genericSkill);
                genericSkill.onSkillChanged += this.TryOverrideSkill;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                bool inputPressed = base.inputBank && base.inputBank.skill2.down;
                if (!inputPressed)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {

            SkillLocator skillLocator = base.skillLocator;
            GenericSkill genericSkill = (skillLocator != null) ? skillLocator.primary : null;
            if (genericSkill)
            {
                genericSkill.onSkillChanged -= this.TryOverrideSkill;
            }
            if (this.overriddenSkill)
            {
                this.overriddenSkill.UnsetSkillOverride(this, TargetAcquired.primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
            }

            if (this.crosshairOverrideRequest != null)
            {
                this.crosshairOverrideRequest.Dispose();
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        //Copied from Railgunner
        private void TryOverrideSkill(GenericSkill skill)
        {
            if (skill && !this.overriddenSkill && !skill.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual))
            {
                this.overriddenSkill = skill;
                this.overriddenSkill.SetSkillOverride(this, TargetAcquired.primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = base.skillLocator.secondary.stock;
            }
        }
    }
}
