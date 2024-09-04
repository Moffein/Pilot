using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace MoffeinPilot.Modules
{
    public static class Buffs
    {
        public static BuffDef WallclingBonus;
        public static BuffDef WallclingBonusAirborne;

        internal static void RegisterBuffs()
        {
            Color pilotColor = new Color32(65, 247, 197, 255);
            WallclingBonus = AddNewBuff("MoffeinPilotWallclingBonus",
                Addressables.LoadAssetAsync<Sprite>("RoR2/Base/CritOnUse/texBuffFullCritIcon.tif").WaitForCompletion(),
                pilotColor,
                false,
                false);

            WallclingBonusAirborne = AddNewBuff("MoffeinPilotWallclingBonusAirborne",
                Addressables.LoadAssetAsync<Sprite>("RoR2/Base/CritOnUse/texBuffFullCritIcon.tif").WaitForCompletion(),
                pilotColor,
                false,
                false);
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            Log.Error("CharacterMotor.OnHitGroundServer DOESN'T EXIST FIX THE HOOK");
            //On.RoR2.CharacterMotor.OnHitGroundServer += CharacterMotor_OnHitGroundServer;
        }

        //private static void CharacterMotor_OnHitGroundServer(On.RoR2.CharacterMotor.orig_OnHitGroundServer orig, CharacterMotor self, CharacterMotor.HitGroundInfo hitGroundInfo)
        //{
        //    orig(self, hitGroundInfo);
        //    if (NetworkServer.active && self.body && self.body.HasBuff(WallclingBonusAirborne)) self.body.RemoveBuff(WallclingBonusAirborne);
        //}

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(WallclingBonus) || sender.HasBuff(WallclingBonusAirborne))
            {
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