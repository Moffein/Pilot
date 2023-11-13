using BepInEx.Configuration;
using EntityStates;
using EntityStates.Pilot.FireSelect;
using EntityStates.Pilot.Parachute;
using EntityStates.Pilot.Weapon;
using Pilot.Modules.Characters;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Pilot.Modules.Survivors
{
    internal class PilotSurvivor : SurvivorBase
    {
        //used when building your character using the prefabs you set up in unity
        //don't upload to thunderstore without changing this
        public override string prefabBodyName => "Henry";

        public const string BODY_PREFIX = PilotPlugin.DEVELOPER_PREFIX + "_PILOT_BODY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => BODY_PREFIX;

        public override BodyInfo bodyInfo { get; set; } = new BodyInfo
        {
            bodyName = "PilotBody",
            bodyNameToken = BODY_PREFIX + "NAME",
            subtitleNameToken = BODY_PREFIX + "SUBTITLE",

            characterPortrait = Assets.addAssetBundle.LoadAsset<Texture>("sPilotPortrait_0"),
            bodyColor = new Color32(56, 148, 77, 255),

            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),

            maxHealth = 110f,
            healthRegen = 1f,
            healthGrowth = 0.2f,
            damage = 12f,
            damageGrowth = 2.4f,
            armor = 0f,

            jumpCount = 1,
        };

        public override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] 
        {
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = Materials.CreateHopooMaterial("matHenry"),
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                },
                new CustomRendererInfo
                {
                    childName = "Model",
                }
        };

        public override UnlockableDef characterUnlockableDef => null;

        public override Type characterMainState => typeof(EntityStates.GenericCharacterMain);

        public override ItemDisplaysBase itemDisplays => new PilotItemDisplays();

                                                                          //if you have more than one character, easily create a config to enable/disable them like this
        public override ConfigEntry<bool> characterEnabledConfig => null; //Modules.Config.CharacterEnableConfig(bodyName);

        private static UnlockableDef masterySkinUnlockableDef;

        public override void InitializeCharacter()
        {
            base.InitializeCharacter();

            NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();

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
            string prefix = PilotPlugin.DEVELOPER_PREFIX;

            SkillDef placeholder = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Heretic/HereticDefaultAbility.asset").WaitForCompletion();

            InitPrimaries();
            InitSecondaries();
            InitUtilities();
            Modules.Skills.AddSpecialSkills(bodyPrefab, new SkillDef[] { placeholder });

            //Default Air Strike
            //12 cooldown
            //Start with 2
            //Marker lasts 15s
            //Seems to stagger
            //2s to recharge explosion
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
            primaryDef.icon = Assets.addAssetBundle.LoadAsset<Sprite>("sPilotSkills_0");
            primaryDef.interruptPriority = InterruptPriority.Any;
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
            Pilot.Modules.Content.AddSkillDef(primaryDef);
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
            primaryAltDef.icon = Assets.addAssetBundle.LoadAsset<Sprite>("sPilotSkills_5");
            primaryAltDef.interruptPriority = InterruptPriority.Any;
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
            Skills.FixSkillName(primaryAltDef);
            Pilot.Modules.Content.AddSkillDef(primaryAltDef);
            SkillDefs.Primaries.RapidFire = primaryAltDef;

            Modules.Skills.AddPrimarySkills(bodyPrefab, new SkillDef[] { primaryDef, primaryAltDef });
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
            secondaryDef.icon = Assets.addAssetBundle.LoadAsset<Sprite>("sPilotSkills_1");
            secondaryDef.interruptPriority = InterruptPriority.Any;
            secondaryDef.isCombatSkill = true;
            secondaryDef.keywordTokens = new string[] { };
            secondaryDef.mustKeyPress = false;
            secondaryDef.cancelSprintingOnActivation = true;
            secondaryDef.rechargeStock = 1;
            secondaryDef.requiredStock = 1;
            secondaryDef.skillName = "PilotSecondary";
            secondaryDef.skillNameToken = "MOFFEIN_PILOT_BODY_SECONDARY_NAME";
            secondaryDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_SECONDARY_DESCRIPTION";
            secondaryDef.stockToConsume = 0;    //Toggling doesn't consume stocks
            Skills.FixSkillName(secondaryDef);
            Pilot.Modules.Content.AddSkillDef(secondaryDef);
            SkillDefs.Secondaries.TargetAcquired = secondaryDef;

            Modules.Skills.AddSecondarySkills(bodyPrefab, new SkillDef[] { secondaryDef });
        }

        private void InitUtilities()
        {
            SteppedSkillDef utilityDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(DeployParachute));
            utilityDef.activationStateMachineName = "Parachute";
            utilityDef.baseMaxStock = 1;
            utilityDef.baseRechargeInterval = 8f;
            utilityDef.beginSkillCooldownOnSkillEnd = false;
            utilityDef.canceledFromSprinting = false;
            utilityDef.dontAllowPastMaxStocks = true;
            utilityDef.forceSprintDuringState = false;
            utilityDef.fullRestockOnAssign = true;
            utilityDef.icon = Assets.addAssetBundle.LoadAsset<Sprite>("sPilotSkills_2");
            utilityDef.interruptPriority = InterruptPriority.Any;
            utilityDef.isCombatSkill = false;
            utilityDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            utilityDef.mustKeyPress = false;
            utilityDef.cancelSprintingOnActivation = true;
            utilityDef.rechargeStock = 1;
            utilityDef.requiredStock = 1;
            utilityDef.skillName = "PilotPrimary";
            utilityDef.skillNameToken = "MOFFEIN_PILOT_BODY_UTILITY_NAME";
            utilityDef.skillDescriptionToken = "MOFFEIN_PILOT_BODY_UTILITY_DESCRIPTION";
            utilityDef.stockToConsume = 1;
            utilityDef.stepCount = 3;
            Skills.FixSkillName(utilityDef);
            Pilot.Modules.Content.AddSkillDef(utilityDef);
            SkillDefs.Utilities.RapidDeployment = utilityDef;

            Modules.Skills.AddUtilitySkills(bodyPrefab, new SkillDef[] {utilityDef });
        }
        
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            //this creates a SkinDef with all default fields
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(BODY_PREFIX + "DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
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
            /*
            //creating a new skindef as we did before
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(HenryPlugin.DEVELOPER_PREFIX + "_HENRY_BODY_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMasteryAchievement"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                masterySkinUnlockableDef);

            //adding the mesh replacements as above. 
            //if you don't want to replace the mesh (for example, you only want to replace the material), pass in null so the order is preserved
            masterySkin.meshReplacements = Modules.Skins.getMeshReplacements(defaultRendererinfos,
                "meshHenrySwordAlt",
                null,//no gun mesh replacement. use same gun mesh
                "meshHenryAlt");

            //masterySkin has a new set of RendererInfos (based on default rendererinfos)
            //you can simply access the RendererInfos defaultMaterials and set them to the new materials for your skin.
            masterySkin.rendererInfos[0].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[1].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");
            masterySkin.rendererInfos[2].defaultMaterial = Modules.Materials.CreateHopooMaterial("matHenryAlt");

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

            skins.Add(masterySkin);
            */
            #endregion

            skinController.skins = skins.ToArray();
        }

        //For easy access
        public static class SkillDefs
        {
            public static class Primaries
            {
                public static SkillDef ClusterFire, RapidFire;
            }
            public static class Secondaries
            {
                public static SkillDef TargetAcquired;
            }
            public static class Utilities
            {
                public static SkillDef RapidDeployment;
            }
        }
    }
}