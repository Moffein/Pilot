using RiskOfOptions;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace MoffeinPilot
{
    public static class ModCompat
    {
        public static bool ScepterLoaded, RiskOfOptionsLoaded, EmoteAPILoaded, InfernoLoaded;

        internal static void Init()
        {
            RiskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            ScepterLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            EmoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            InfernoLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
        }

        public static void SetupScepter(string bodyName, SkillDef scepterSkill, SkillSlot skillSlot, int skillIndex)
        {
            if (ScepterLoaded) SetupScepterInternal(bodyName, scepterSkill, skillSlot, skillIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupScepterInternal(string bodyName, SkillDef scepterSkill, SkillSlot skillSlot, int skillIndex)
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSkill, bodyName, skillSlot, skillIndex);
        }

        public static void SetupOptions()
        {
            if (RiskOfOptionsLoaded) SetupOptionsInternal();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetupOptionsInternal()
        {
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.MoffeinPilot.Parachute.DeployParachute.holdToAscend));
            ModSettingsManager.SetModIcon(Modules.Assets.pilotAssetBundle.LoadAsset<Sprite>("sPilotPortrait_0"));
        }

    }
}
