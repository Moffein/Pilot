using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MoffeinPilot.Modules
{
    public static class Buffs
    {
        public static BuffDef WallclingBonus;

        internal static void RegisterBuffs()
        {
            BuffDef vanillaWarbanner = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/WardOnLevel/bdWarbanner.asset").WaitForCompletion();
            WallclingBonus = AddNewBuff("MoffeinPilotWallclingBonus",
                Addressables.LoadAssetAsync<Sprite>("RoR2/Base/CritOnUse/texBuffFullCritIcon.tif").WaitForCompletion(), 
                vanillaWarbanner.buffColor,
                false,
                false);
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(WallclingBonus))
            {
                args.armorAdd += 30;
                args.attackSpeedMultAdd += 0.3f;
            }
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            Modules.Content.AddBuffDef(buffDef);

            return buffDef;
        }
    }
}