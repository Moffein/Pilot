﻿using MoffeinPilot.Content.Components;
using MoffeinPilot.Modules;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.MoffeinPilot.FireSelect
{
    public class TargetAcquired : BaseState
    {
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerCrosshair.prefab").WaitForCompletion();
        public static SkillDef primaryOverride;
        public static string entrySoundString = "Play_railgunner_m2_scope_in";
        public static string exitSoundString = "Play_railgunner_m2_scope_out";

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private GenericSkill overriddenSkill;
        private PilotController pilotController;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(TargetAcquired.entrySoundString, base.gameObject);
            Util.PlaySound("Play_railgunner_m2_scope_loop", base.gameObject);
            pilotController = base.GetComponent<PilotController>();
            if (pilotController) pilotController.BeginAutoAim();
            if (TargetAcquired.crosshairOverridePrefab)
            {
                this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, TargetAcquired.crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);
            }
            if (isAuthority && characterBody && !characterBody.HasBuff(Buffs.ParachuteSpeed))
            {
                characterBody.isSprinting = false;
            }

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

            if (pilotController)
            {
                pilotController.EndAutoAim();
            }
            Util.PlaySound("Stop_railgunner_m2_scope_loop", base.gameObject);
            Util.PlaySound(TargetAcquired.exitSoundString, base.gameObject);
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
