using RiskOfOptions;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace MoffeinPilot
{
    public static class ModCompat
    {
        public static bool ScepterLoaded, RiskOfOptionsLoaded, EmoteAPILoaded, InfernoLoaded;

        internal static void CheckDependencies()
        {
            RiskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            ScepterLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            EmoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            InfernoLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
        }

        internal static void Init()
        {
            if (EmoteAPILoaded) SetupEmotes();
        }

        public static void SetupScepter(string bodyName, SkillDef scepterSkill, SkillDef skillToReplace)
        {
            if (ScepterLoaded) SetupScepterInternal(bodyName, scepterSkill, skillToReplace);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupScepterInternal(string bodyName, SkillDef scepterSkill, SkillDef skillToReplace)
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSkill, bodyName, skillToReplace);
        }

        public static void SetupOptions()
        {
            if (RiskOfOptionsLoaded) SetupOptionsInternal();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupOptionsInternal()
        {
            ModSettingsManager.SetModIcon(Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIconPilot"));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.MoffeinPilot.Weapon.ClusterFire.useLaser));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.MoffeinPilot.Parachute.DeployParachute.enableParachuteFade));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.MoffeinPilot.Parachute.DeployParachute.holdToAscend));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupEmotes()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();
                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "MoffeinPilotBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("PilotEmotePrefab.prefab");
                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                    }
                }
            };
        }
    }
}
