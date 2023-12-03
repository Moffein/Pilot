using Pilot.Content.Components;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2.Skills;
using RoR2;
using RoR2.UI;

namespace EntityStates.MoffeinPilot.FireSelect
{
    public class ColdWar : BaseState
    {
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherCrosshair.prefab").WaitForCompletion();
        public static SkillDef primaryOverride;
        public static string entrySoundString = "Play_railgunner_m2_scope_in";
        public static string exitSoundString = "Play_railgunner_m2_scope_out";

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private GenericSkill overriddenSkill;
        private PilotController pilotController;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(ColdWar.entrySoundString, base.gameObject);
            Util.PlaySound("Play_railgunner_m2_scope_loop", base.gameObject);
            pilotController = base.GetComponent<PilotController>();
            if (ColdWar.crosshairOverridePrefab)
            {
                this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, ColdWar.crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
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
                //Enforce stock consistency
                //Would be more ideal to directly hook when stocks are gained/lost
                if (base.skillLocator)
                {
                    if (base.skillLocator.secondary.stock != this.overriddenSkill.stock) this.overriddenSkill.stock = base.skillLocator.secondary.stock;
                }

                bool outOfStock = !(base.skillLocator && base.skillLocator.secondary && base.skillLocator.secondary.stock > 0);
                bool inputPressed = base.inputBank && base.inputBank.skill2.down;
                if (!inputPressed || outOfStock)
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
                this.overriddenSkill.UnsetSkillOverride(this, ColdWar.primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
            }

            if (this.crosshairOverrideRequest != null)
            {
                this.crosshairOverrideRequest.Dispose();
            }
            Util.PlaySound("Stop_railgunner_m2_scope_loop", base.gameObject);
            Util.PlaySound(ColdWar.exitSoundString, base.gameObject);
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
                this.overriddenSkill.SetSkillOverride(this, ColdWar.primaryOverride, GenericSkill.SkillOverridePriority.Contextual);
                this.overriddenSkill.stock = base.skillLocator.secondary.stock;
            }
        }
    }
}
