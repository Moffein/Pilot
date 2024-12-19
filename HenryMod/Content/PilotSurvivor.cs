using BepInEx.Configuration;
using EntityStates;
using EntityStates.MoffeinPilot;
using EntityStates.MoffeinPilot.Airstrike;
using EntityStates.MoffeinPilot.FireSelect;
using EntityStates.MoffeinPilot.Parachute;
using EntityStates.MoffeinPilot.Weapon;
using MoffeinPilot.Content.Components;
using MoffeinPilot.Modules.Characters;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace MoffeinPilot.Modules.Survivors
{
    internal class PilotSurvivor : SurvivorBase
    {
        //used when building your character using the prefabs you set up in unity
        //don't upload to thunderstore without changing this
        public override string prefabBodyName => "MoffeinPilot";

        public const string BODY_PREFIX = PilotPlugin.DEVELOPER_PREFIX + "_PILOT_BODY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => BODY_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "MoffeinPilotBody",
            bodyNameToken = BODY_PREFIX + "NAME",
            subtitleNameToken = BODY_PREFIX + "SUBTITLE",

            characterPortrait = Asset.mainAssetBundle.LoadAsset<Texture>("texIconPilot"),
            bodyColor = new Color32(56, 148, 77, 255),

            crosshair = Modules.Asset.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 90f,
            healthGrowth = 27f,
            healthRegen = 1f,
            regenGrowth = 0.2f,
            damage = 12f,
            damageGrowth = 2.4f,
            armor = 0f,

            jumpCount = 1
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] 
        {
                //new CustomRendererInfo
                //{
                //    childName = "SwordModel",
                //    material = Materials.CreateHopooMaterial("matHenry"),
                //},
                //new CustomRendererInfo
                //{
                //    childName = "GunModel",
                //},
                //new CustomRendererInfo
                //{
                //    childName = "Model",
                //}
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => null;// new PilotItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();

            bodyPrefab.AddComponent<PilotController>();

            EntityStateMachine bodyMachine = EntityStateMachine.FindByCustomName(bodyPrefab, "Body");
            bodyMachine.mainStateType = new SerializableEntityStateType(typeof(PilotMainState));
            
            NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();

            EntityStateMachine airstrikeMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            airstrikeMachine.customName = "Airstrike";
            airstrikeMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
            airstrikeMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
            nsm.stateMachines = nsm.stateMachines.Append(airstrikeMachine).ToArray();
            
            EntityStateMachine targetingMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            targetingMachine.customName = "FireSelect";
            targetingMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
            targetingMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
            nsm.stateMachines = nsm.stateMachines.Append(targetingMachine).ToArray();

            EntityStateMachine parachuteStateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            parachuteStateMachine.customName = "Parachute";
            parachuteStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
            parachuteStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
            nsm.stateMachines = nsm.stateMachines.Append(parachuteStateMachine).ToArray();

            SetStateOnHurt ssoh = bodyPrefab.GetComponent<SetStateOnHurt>();
            if (ssoh)
            {
                Array.Resize(ref ssoh.idleStateMachine, ssoh.idleStateMachine.Length + 1);
                ssoh.idleStateMachine[ssoh.idleStateMachine.Length - 1] = parachuteStateMachine;
            }
            FinalizeCSSPreviewDisplayController();
        }

        private void InitializeDoppelgangerAfterSkills()
        {
            GameObject doppelganger = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoMonsterMaster.prefab").WaitForCompletion(), "MoffeinPilotMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
            Modules.Prefabs.RemoveAISkillDrivers(doppelganger);


            Modules.Prefabs.AddAISkillDriver(doppelganger, "Primary", SkillSlot.Primary, SkillDefs.Secondaries.FireTargetAcquired,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Primary", SkillSlot.Primary, SkillDefs.Secondaries.FireColdWar,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Secondary", SkillSlot.Secondary, SkillDefs.Secondaries.TargetAcquired,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Secondary", SkillSlot.Secondary, SkillDefs.Secondaries.ColdWar,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Special", SkillSlot.Special, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 30f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                true,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "UseMovementWhenClose", SkillSlot.Utility, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 30f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtMoveTarget,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                true,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Strafe", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 30f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Chase", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                30f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);

            Modules.Content.AddMasterPrefab(doppelganger);
        }

        public override void InitializeUnlockables()
        {
            //uncomment this when you have a mastery skin. when you do, make sure you have an icon too
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Modules.Achievements.MasteryAchievement>();
        }

        public override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();

            //example of how to create a hitbox
            //Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            //Modules.Prefabs.SetupHitbox(prefabCharacterModel.gameObject, hitboxTransform, "Sword");
        }

        public override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            InitPrimaries();
            InitSecondaries();
            InitUtilities();
            InitSpecials();

            InitializeDoppelgangerAfterSkills();
        }

        private void InitPrimaries()
        {
            SteppedSkillDef primaryDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDef.activationState = new SerializableEntityStateType(typeof(ClusterFire));
            primaryDef.activationStateMachineName = "Weapon";
            primaryDef.baseMaxStock = 1;
            primaryDef.baseRechargeInterval = 0f;
            primaryDef.beginSkillCooldownOnSkillEnd = false;
            primaryDef.canceledFromSprinting = false;
            primaryDef.dontAllowPastMaxStocks = true;
            primaryDef.forceSprintDuringState = false;
            primaryDef.fullRestockOnAssign = true;
            primaryDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillClusterFire");
            primaryDef.interruptPriority = InterruptPriority.Skill;
            primaryDef.isCombatSkill = true;
            primaryDef.keywordTokens = new string[] { };
            primaryDef.mustKeyPress = false;
            primaryDef.cancelSprintingOnActivation = true;
            primaryDef.rechargeStock = 1;
            primaryDef.requiredStock = 1;
            primaryDef.skillName = "PilotPrimary";
            primaryDef.skillNameToken = "MOFFEIN_PILOT_BODY_PRIMARY_NAME";
            primaryDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_PRIMARY_DESCRIPTION";
            primaryDef.stockToConsume = 1;
            primaryDef.stepCount = 3;
            Skills.FixSkillName(primaryDef);
            MoffeinPilot.Modules.Content.AddSkillDef(primaryDef);
            SkillDefs.Primaries.ClusterFire = primaryDef;

            SkillDef primaryAltDef = ScriptableObject.CreateInstance<SkillDef>();
            primaryAltDef.activationState = new SerializableEntityStateType(typeof(RapidFire));
            primaryAltDef.activationStateMachineName = "Weapon";
            primaryAltDef.baseMaxStock = 1;
            primaryAltDef.baseRechargeInterval = 0f;
            primaryAltDef.beginSkillCooldownOnSkillEnd = false;
            primaryAltDef.canceledFromSprinting = false;
            primaryAltDef.dontAllowPastMaxStocks = true;
            primaryAltDef.forceSprintDuringState = false;
            primaryAltDef.fullRestockOnAssign = true;
            primaryAltDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillRapidFire");
            primaryAltDef.interruptPriority = InterruptPriority.Skill;
            primaryAltDef.isCombatSkill = true;
            primaryAltDef.keywordTokens = new string[] { };
            primaryAltDef.mustKeyPress = false;
            primaryAltDef.cancelSprintingOnActivation = true;
            primaryAltDef.rechargeStock = 1;
            primaryAltDef.requiredStock = 1;
            primaryAltDef.skillName = "PilotPrimaryAlt";
            primaryAltDef.skillNameToken = "MOFFEIN_PILOT_BODY_PRIMARY_ALT_NAME";
            primaryAltDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_PRIMARY_ALT_DESCRIPTION";
            primaryAltDef.stockToConsume = 1;
            //primaryAltDef.reloadInterruptPriority = InterruptPriority.Any;
            //primaryAltDef.reloadState = new SerializableEntityStateType(typeof(ReloadRapidFire));
            //primaryAltDef.graceDuration = 0.4f;
            Skills.FixSkillName(primaryAltDef);
            MoffeinPilot.Modules.Content.AddSkillDef(primaryAltDef);
            SkillDefs.Primaries.RapidFire = primaryAltDef;

            SkillDef primarySilencerDef = ScriptableObject.CreateInstance<SkillDef>();
            primarySilencerDef.activationState = new SerializableEntityStateType(typeof(FireSilencedPistol));
            primarySilencerDef.activationStateMachineName = "Weapon";
            primarySilencerDef.baseMaxStock = 1;
            primarySilencerDef.baseRechargeInterval = 0f;
            primarySilencerDef.beginSkillCooldownOnSkillEnd = false;
            primarySilencerDef.canceledFromSprinting = false;
            primarySilencerDef.dontAllowPastMaxStocks = true;
            primarySilencerDef.forceSprintDuringState = false;
            primarySilencerDef.fullRestockOnAssign = true;
            primarySilencerDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("sPilotSkills_0");
            primarySilencerDef.interruptPriority = InterruptPriority.Skill;
            primarySilencerDef.isCombatSkill = true;
            primarySilencerDef.keywordTokens = new string[] { "KEYWORD_SLAYER" };
            primarySilencerDef.mustKeyPress = false;
            primarySilencerDef.cancelSprintingOnActivation = true;
            primarySilencerDef.rechargeStock = 1;
            primarySilencerDef.requiredStock = 1;
            primarySilencerDef.skillName = "PilotPrimarySilencer";
            primarySilencerDef.skillNameToken = "MOFFEIN_PILOT_BODY_PRIMARY_SILENCER_NAME";
            primarySilencerDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_PRIMARY_SILENCER_DESCRIPTION";
            primarySilencerDef.stockToConsume = 1;
            Skills.FixSkillName(primarySilencerDef);
            MoffeinPilot.Modules.Content.AddSkillDef(primarySilencerDef);
            SkillDefs.Primaries.Silencer = primarySilencerDef;

            GameObject altPrimaryTracer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoShotgun.prefab").WaitForCompletion().InstantiateClone("MoffeinPilotRapidFireTracerEffect", false);
            Tracer t = altPrimaryTracer.GetComponent<Tracer>();
            t.speed = 502f;
            Content.AddEffectDef(new EffectDef(altPrimaryTracer));
            RapidFire.tracerEffectPrefab = altPrimaryTracer;

            GameObject secondaryTracer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/TracerHuntressSnipe.prefab").WaitForCompletion().InstantiateClone("MoffeinPilotSecondaryTracerEffect", false);
            UnityEngine.Object.Destroy(secondaryTracer.GetComponent<BeamPointsFromTransforms>());
            UnityEngine.Object.Destroy(secondaryTracer.GetComponent<EventFunctions>());
            t = secondaryTracer.GetComponent<Tracer>();
            t.headTransform = null;
            Content.AddEffectDef(new EffectDef(secondaryTracer));
            FireTargetAcquired.tracerEffectPrefab = secondaryTracer;

            GameObject visualizer = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerSniperTargetVisualizerLight.prefab").WaitForCompletion().InstantiateClone("MoffeinPilotTargetVisualizer", false);
            GameObject crosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("MoffeinPilotWeakpointCrosshair", false);
            AddWeakpointUI(crosshairPrefab, visualizer);
            //PilotController.weakpointCrosshairPrefab = crosshairPrefab;

            GameObject effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/SniperTargetHitEffect.prefab").WaitForCompletion().InstantiateClone("MoffeinPilotHeadshotEffect", false);
            EffectComponent ec = effect.GetComponent<EffectComponent>();
            ec.soundName = "";//"Play_SniperClassic_headshot";
            Content.AddEffectDef(new EffectDef(effect));

            FireSilencedPistol.weakpointEffectPrefab = effect;

            Modules.Skills.AddPrimarySkills(bodyPrefab, new SkillDef[] { primaryDef, primaryAltDef, primarySilencerDef });
            visualizer.transform.localScale = 10f * Vector3.one;

            var locator = bodyPrefab.GetComponent<SkillLocator>();
            AddCssPreviewSkill(0, locator.primary.skillFamily, primaryDef);
            AddCssPreviewSkill(1, locator.primary.skillFamily, primaryAltDef);
            AddCssPreviewSkill(2, locator.primary.skillFamily, primarySilencerDef);
        }


        private void AddWeakpointUI(GameObject crosshair, GameObject visualizerPrefab)
        {
            PointViewer pv = crosshair.AddComponent<PointViewer>();
            SniperTargetViewer stv = crosshair.AddComponent<SniperTargetViewer>();
            stv.visualizerPrefab = visualizerPrefab;
        }

        private void InitSecondaries()
        {
            SkillDef secondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef.activationState = new SerializableEntityStateType(typeof(TargetAcquired));
            secondaryDef.activationStateMachineName = "FireSelect";
            secondaryDef.baseMaxStock = 2;
            secondaryDef.baseRechargeInterval = 4f;
            secondaryDef.beginSkillCooldownOnSkillEnd = false;
            secondaryDef.canceledFromSprinting = false;
            secondaryDef.dontAllowPastMaxStocks = true;
            secondaryDef.forceSprintDuringState = false;
            secondaryDef.fullRestockOnAssign = true;
            secondaryDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillTargetAcquired");
            secondaryDef.interruptPriority = InterruptPriority.Any;
            secondaryDef.isCombatSkill = false;
            secondaryDef.keywordTokens = new string[] { };
            secondaryDef.mustKeyPress = false;
            secondaryDef.cancelSprintingOnActivation = true;
            secondaryDef.rechargeStock = 1;
            secondaryDef.requiredStock = 1;
            secondaryDef.skillName = "PilotSecondary";
            secondaryDef.skillNameToken = "MOFFEIN_PILOT_BODY_SECONDARY_NAME";
            secondaryDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SECONDARY_DESCRIPTION";
            secondaryDef.stockToConsume = 0;    //Toggling doesn't consume stocks
            secondaryDef.autoHandleLuminousShot = false;
            Skills.FixSkillName(secondaryDef);
            MoffeinPilot.Modules.Content.AddSkillDef(secondaryDef);
            SkillDefs.Secondaries.TargetAcquired = secondaryDef;

            SkillDef secondaryOverrideDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryOverrideDef.activationState = new SerializableEntityStateType(typeof(FireTargetAcquired));
            secondaryOverrideDef.activationStateMachineName = "Weapon";
            secondaryOverrideDef.baseMaxStock = 1;
            secondaryOverrideDef.baseRechargeInterval = 0f;
            secondaryOverrideDef.beginSkillCooldownOnSkillEnd = false;
            secondaryOverrideDef.canceledFromSprinting = false;
            secondaryOverrideDef.dontAllowPastMaxStocks = false;
            secondaryOverrideDef.forceSprintDuringState = false;
            secondaryOverrideDef.fullRestockOnAssign = true;
            secondaryOverrideDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillTargetAcquired");
            secondaryOverrideDef.interruptPriority = InterruptPriority.Skill;
            secondaryOverrideDef.isCombatSkill = true;
            secondaryOverrideDef.keywordTokens = new string[] { };
            secondaryOverrideDef.mustKeyPress = false;
            secondaryOverrideDef.cancelSprintingOnActivation = true;
            secondaryOverrideDef.rechargeStock = 0;
            secondaryOverrideDef.requiredStock = 1;
            secondaryOverrideDef.skillName = "PilotSecondaryOverride";
            secondaryOverrideDef.skillNameToken = "MOFFEIN_PILOT_BODY_SECONDARY_NAME";
            secondaryOverrideDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SECONDARY_DESCRIPTION";
            secondaryOverrideDef.stockToConsume = 1;
            Skills.FixSkillName(secondaryOverrideDef);
            MoffeinPilot.Modules.Content.AddSkillDef(secondaryOverrideDef);
            SkillDefs.Secondaries.FireTargetAcquired = secondaryOverrideDef;
            TargetAcquired.primaryOverride = secondaryOverrideDef;

            SkillDef secondaryAltDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryAltDef.activationState = new SerializableEntityStateType(typeof(ColdWar));
            secondaryAltDef.activationStateMachineName = "FireSelect";
            secondaryAltDef.baseMaxStock = 2;
            secondaryAltDef.baseRechargeInterval = 4f;
            secondaryAltDef.beginSkillCooldownOnSkillEnd = false;
            secondaryAltDef.canceledFromSprinting = false;
            secondaryAltDef.dontAllowPastMaxStocks = true;
            secondaryAltDef.forceSprintDuringState = false;
            secondaryAltDef.fullRestockOnAssign = true;
            secondaryAltDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("sPilotSkills_1");
            secondaryAltDef.interruptPriority = InterruptPriority.Any;
            secondaryAltDef.isCombatSkill = false;
            secondaryAltDef.keywordTokens = new string[] { };
            secondaryAltDef.mustKeyPress = false;
            secondaryAltDef.cancelSprintingOnActivation = true;
            secondaryAltDef.rechargeStock = 1;
            secondaryAltDef.requiredStock = 1;
            secondaryAltDef.skillName = "PilotSecondaryAlt";
            secondaryAltDef.skillNameToken = "MOFFEIN_PILOT_BODY_SECONDARY_ALT_NAME";
            secondaryAltDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SECONDARY_ALT_DESCRIPTION";
            secondaryAltDef.stockToConsume = 0;    //Toggling doesn't consume stocks
            secondaryAltDef.autoHandleLuminousShot = false;
            Skills.FixSkillName(secondaryAltDef);
            MoffeinPilot.Modules.Content.AddSkillDef(secondaryAltDef);
            SkillDefs.Secondaries.ColdWar = secondaryAltDef;

            SkillDef secondaryAltOverrideDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryAltOverrideDef.activationState = new SerializableEntityStateType(typeof(FireColdWar));
            secondaryAltOverrideDef.activationStateMachineName = "Weapon";
            secondaryAltOverrideDef.baseMaxStock = 1;
            secondaryAltOverrideDef.baseRechargeInterval = 0f;
            secondaryAltOverrideDef.beginSkillCooldownOnSkillEnd = false;
            secondaryAltOverrideDef.canceledFromSprinting = false;
            secondaryAltOverrideDef.dontAllowPastMaxStocks = false;
            secondaryAltOverrideDef.forceSprintDuringState = false;
            secondaryAltOverrideDef.fullRestockOnAssign = true;
            secondaryAltOverrideDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("sPilotSkills_1");
            secondaryAltOverrideDef.interruptPriority = InterruptPriority.Skill;
            secondaryAltOverrideDef.isCombatSkill = true;
            secondaryAltOverrideDef.keywordTokens = new string[] { };
            secondaryAltOverrideDef.mustKeyPress = false;
            secondaryAltOverrideDef.cancelSprintingOnActivation = true;
            secondaryAltOverrideDef.rechargeStock = 0;
            secondaryAltOverrideDef.requiredStock = 1;
            secondaryAltOverrideDef.skillName = "PilotSecondaryAltOverride";
            secondaryAltOverrideDef.skillNameToken = "MOFFEIN_PILOT_BODY_SECONDARY_ALT_NAME";
            secondaryAltOverrideDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SECONDARY_ALT_DESCRIPTION";
            secondaryAltOverrideDef.stockToConsume = 1;
            Skills.FixSkillName(secondaryAltOverrideDef);
            MoffeinPilot.Modules.Content.AddSkillDef(secondaryAltOverrideDef);
            SkillDefs.Secondaries.FireColdWar = secondaryAltOverrideDef;
            ColdWar.primaryOverride = secondaryAltOverrideDef;

            Modules.Skills.AddSecondarySkills(bodyPrefab, new SkillDef[] { secondaryDef, secondaryAltDef });
        }

        internal static void HandleLuminousShotServer(CharacterBody body)
        {
            if (!NetworkServer.active || !body || !body.inventory) return;
            if (body.inventory.GetItemCount(DLC2Content.Items.IncreasePrimaryDamage) <= 0) return;

            body.AddIncreasePrimaryDamageStack();
        }

        private void InitUtilities()
        {
            SkillDef utilityDef = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(DeployParachute));
            utilityDef.activationStateMachineName = "Parachute";
            utilityDef.baseMaxStock = 1;
            utilityDef.baseRechargeInterval = 12f;
            utilityDef.beginSkillCooldownOnSkillEnd = false;
            utilityDef.canceledFromSprinting = false;
            utilityDef.dontAllowPastMaxStocks = true;
            utilityDef.forceSprintDuringState = false;
            utilityDef.fullRestockOnAssign = true;
            utilityDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillRapidDeployment");
            utilityDef.interruptPriority = InterruptPriority.Any;
            utilityDef.isCombatSkill = false;
            utilityDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            utilityDef.mustKeyPress = true;
            utilityDef.cancelSprintingOnActivation = false;
            utilityDef.rechargeStock = 1;
            utilityDef.requiredStock = 1;
            utilityDef.skillName = "PilotParachute";
            utilityDef.skillNameToken = "MOFFEIN_PILOT_BODY_UTILITY_NAME";
            utilityDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_UTILITY_DESCRIPTION";
            utilityDef.stockToConsume = 1;
            Skills.FixSkillName(utilityDef);
            MoffeinPilot.Modules.Content.AddSkillDef(utilityDef);
            SkillDefs.Utilities.RapidDeployment = utilityDef;

            SkillDef utilityAltDef = ScriptableObject.CreateInstance<SkillDef>();
            utilityAltDef.activationState = new SerializableEntityStateType(typeof(AerobaticsDashEntry));
            utilityAltDef.activationStateMachineName = "Parachute";
            utilityAltDef.baseMaxStock = 1;
            utilityAltDef.baseRechargeInterval = 8f;
            utilityAltDef.beginSkillCooldownOnSkillEnd = false;
            utilityAltDef.canceledFromSprinting = false;
            utilityAltDef.dontAllowPastMaxStocks = true;
            utilityAltDef.forceSprintDuringState = true;
            utilityAltDef.fullRestockOnAssign = false;
            utilityAltDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("sPilotSkills_6");
            utilityAltDef.interruptPriority = InterruptPriority.Any;
            utilityAltDef.isCombatSkill = false;
            utilityAltDef.keywordTokens = new string[] {};
            utilityAltDef.mustKeyPress = true;
            utilityAltDef.cancelSprintingOnActivation = false;
            utilityAltDef.rechargeStock = 1;
            utilityAltDef.requiredStock = 1;
            utilityAltDef.skillName = "PilotDash";
            utilityAltDef.skillNameToken = "MOFFEIN_PILOT_BODY_UTILITY_ALT_NAME";
            utilityAltDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_UTILITY_ALT_DESCRIPTION";
            utilityAltDef.stockToConsume = 1;
            Skills.FixSkillName(utilityAltDef);
            MoffeinPilot.Modules.Content.AddSkillDef(utilityAltDef);
            SkillDefs.Utilities.Aerobatics = utilityAltDef;

            Modules.Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] { utilityDef, utilityAltDef });
        }

        private void InitSpecials()
        {
            SkillDef specialDef = ScriptableObject.CreateInstance<SkillDef>();
            specialDef.activationState = new SerializableEntityStateType(typeof(PlaceAirstrike));
            specialDef.activationStateMachineName = "Airstrike";
            specialDef.baseMaxStock = 2;
            specialDef.baseRechargeInterval = 12f;
            specialDef.beginSkillCooldownOnSkillEnd = false;
            specialDef.canceledFromSprinting = false;
            specialDef.dontAllowPastMaxStocks = true;
            specialDef.forceSprintDuringState = false;
            specialDef.fullRestockOnAssign = true;
            specialDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillAirstrike");
            specialDef.interruptPriority = InterruptPriority.Any;
            specialDef.isCombatSkill = true;
            specialDef.keywordTokens = new string[] {};
            specialDef.mustKeyPress = true;
            specialDef.cancelSprintingOnActivation = false;
            specialDef.rechargeStock = 1;
            specialDef.requiredStock = 1;
            specialDef.skillName = "PilotSpecial";
            specialDef.skillNameToken = "MOFFEIN_PILOT_BODY_SPECIAL_NAME";
            specialDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SPECIAL_DESCRIPTION";
            specialDef.stockToConsume = 1;
            Skills.FixSkillName(specialDef);
            MoffeinPilot.Modules.Content.AddSkillDef(specialDef);
            SkillDefs.Specials.Airstrike = specialDef;

            SkillDef specialScepterDef = ScriptableObject.CreateInstance<SkillDef>();
            specialScepterDef.activationState = new SerializableEntityStateType(typeof(PlaceAirstrikeScepter));
            specialScepterDef.activationStateMachineName = "Airstrike";
            specialScepterDef.baseMaxStock = 2;
            specialScepterDef.baseRechargeInterval = 12f;
            specialScepterDef.beginSkillCooldownOnSkillEnd = false;
            specialScepterDef.canceledFromSprinting = false;
            specialScepterDef.dontAllowPastMaxStocks = true;
            specialScepterDef.forceSprintDuringState = false;
            specialScepterDef.fullRestockOnAssign = true;
            specialScepterDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillAirstrikeScepter");
            specialScepterDef.interruptPriority = InterruptPriority.Any;
            specialScepterDef.isCombatSkill = true;
            specialScepterDef.keywordTokens = new string[] { };
            specialScepterDef.mustKeyPress = true;
            specialScepterDef.cancelSprintingOnActivation = false;
            specialScepterDef.rechargeStock = 1;
            specialScepterDef.requiredStock = 1;
            specialScepterDef.skillName = "PilotSpecialScepter";
            specialScepterDef.skillNameToken = "MOFFEIN_PILOT_BODY_SPECIAL_SCEPTER_NAME";
            specialScepterDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SPECIAL_SCEPTER_DESCRIPTION";
            specialScepterDef.stockToConsume = 1;
            Skills.FixSkillName(specialScepterDef);
            MoffeinPilot.Modules.Content.AddSkillDef(specialScepterDef);
            SkillDefs.Specials.AirstrikeScepter = specialScepterDef;


            SkillDef specialAltDef = ScriptableObject.CreateInstance<SkillDef>();
            specialAltDef.activationState = new SerializableEntityStateType(typeof(PlaceAirstrikeAlt));
            specialAltDef.activationStateMachineName = "Airstrike";
            specialAltDef.baseMaxStock = 1;
            specialAltDef.baseRechargeInterval = 12f;
            specialAltDef.beginSkillCooldownOnSkillEnd = false;
            specialAltDef.canceledFromSprinting = false;
            specialAltDef.dontAllowPastMaxStocks = true;
            specialAltDef.forceSprintDuringState = false;
            specialAltDef.fullRestockOnAssign = true;
            specialAltDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillAerialSupport");
            specialAltDef.interruptPriority = InterruptPriority.Any;
            specialAltDef.isCombatSkill = true;
            specialAltDef.keywordTokens = new string[] { };
            specialAltDef.mustKeyPress = true;
            specialAltDef.cancelSprintingOnActivation = true;
            specialAltDef.rechargeStock = 1;
            specialAltDef.requiredStock = 1;
            specialAltDef.skillName = "PilotSpecialAlt";
            specialAltDef.skillNameToken = "MOFFEIN_PILOT_BODY_SPECIAL_ALT_NAME";
            specialAltDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SPECIAL_ALT_DESCRIPTION";
            specialAltDef.stockToConsume = 1;
            Skills.FixSkillName(specialAltDef);
            MoffeinPilot.Modules.Content.AddSkillDef(specialAltDef);
            SkillDefs.Specials.AerialSupport = specialAltDef;

            SkillDef specialAltScepterDef = ScriptableObject.CreateInstance<SkillDef>();
            specialAltScepterDef.activationState = new SerializableEntityStateType(typeof(PlaceAirstrikeAltScepter));
            specialAltScepterDef.activationStateMachineName = "Airstrike";
            specialAltScepterDef.baseMaxStock = 1;
            specialAltScepterDef.baseRechargeInterval = 12f;
            specialAltScepterDef.beginSkillCooldownOnSkillEnd = false;
            specialAltScepterDef.canceledFromSprinting = false;
            specialAltScepterDef.dontAllowPastMaxStocks = true;
            specialAltScepterDef.forceSprintDuringState = false;
            specialAltScepterDef.fullRestockOnAssign = true;
            specialAltScepterDef.icon = Asset.mainAssetBundle.LoadAsset<Sprite>("texSkillAerialSupportScepter");
            specialAltScepterDef.interruptPriority = InterruptPriority.Any;
            specialAltScepterDef.isCombatSkill = true;
            specialAltScepterDef.keywordTokens = new string[] { };
            specialAltScepterDef.mustKeyPress = true;
            specialAltScepterDef.cancelSprintingOnActivation = true;
            specialAltScepterDef.rechargeStock = 1;
            specialAltScepterDef.requiredStock = 1;
            specialAltScepterDef.skillName = "PilotSpecialAlt";
            specialAltScepterDef.skillNameToken = "MOFFEIN_PILOT_BODY_SPECIAL_ALT_SCEPTER_NAME";
            specialAltScepterDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SPECIAL_ALT_SCEPTER_DESCRIPTION";
            specialAltScepterDef.stockToConsume = 1;
            Skills.FixSkillName(specialAltScepterDef);
            MoffeinPilot.Modules.Content.AddSkillDef(specialAltScepterDef);
            SkillDefs.Specials.AerialSupportScepter = specialAltScepterDef;

            Modules.Skills.AddSpecialSkills(bodyPrefab, new SkillDef[] { specialDef, specialAltDef });

            ModCompat.SetupScepter("MoffeinPilotBody", specialScepterDef, specialDef);
            ModCompat.SetupScepter("MoffeinPilotBody", specialAltScepterDef, specialAltDef);
        }

        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("DEFAULT_SKIN",
                Asset.mainAssetBundle.LoadAsset<Sprite>("texIconSkinPilotDefault"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);

            //these are your Mesh Replacements. The order here is based on your CustomRendererInfos from earlier
            //pass in meshes as they are named in your assetbundle
            //defaultSkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
            //    "meshHenrySword",
            //    "meshHenryGun",
            //    "meshHenry");
            
            //add new skindef to our list of skindefs. this is what we'll be passing to the SkinController
            skins.Add(defaultSkin);
            #endregion
            
            //uncomment this when you have a mastery skin
            #region MasterySkin
            
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(BODY_PREFIX + " scrawny fuck",
                Asset.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject/*,
                masterySkinUnlockableDef*/);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "PilotPistol",
                "PilotWeapon",
                "PilotBody",
                "PilotBody.001",
                "PilotBreather",
                "PilotEyes",
                "Pilotfur",
                "PilotHead",
                "PilotMetal",
                "PilotPouches");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            //masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            //masterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            //masterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");

            //here's a barebones example of using gameobjectactivations that could probably be streamlined or rewritten entirely, truthfully, but it works
            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = childLocator.FindChildGameObject("GunModel"),
                    shouldActivate = false,
                }
            };
            //simply find an object on your child locator you want to activate/deactivate and set if you want to activate/deacitvate it with this skin

            //skins.Add(masterySkin);
            
            #endregion

            skinController.skins = skins.ToArray();
        }

        //For easy access
        public static class SkillDefs
        {
            public static class Primaries
            {
                public static SkillDef RapidFire;
                public static SteppedSkillDef ClusterFire;
                public static SkillDef Silencer;
            }
            public static class Secondaries
            {
                public static SkillDef TargetAcquired, FireTargetAcquired, ColdWar, FireColdWar;

            }
            public static class Utilities
            {
                public static SkillDef RapidDeployment, Aerobatics, Aerobatics2;
            }
            public static class Specials
            {
                public static SkillDef Airstrike, AirstrikeScepter, AerialSupport, AerialSupportScepter;

            }
        }
    }
}