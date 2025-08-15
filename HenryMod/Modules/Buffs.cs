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
        public static BuffDef ParachuteSpeed;

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
            On.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;

            ParachuteSpeed = AddNewBuff("MoffeinPilotParachuteSpeed",
                Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texMovespeedBuffIcon.tif").WaitForCompletion(),
                Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Bandit2/bdCloakSpeed.asset").WaitForCompletion().buffColor,
                false,
                false);
            ParachuteSpeed.isHidden = true;
        }

        private static void GlobalEventManager_OnCharacterHitGroundServer(On.RoR2.GlobalEventManager.orig_OnCharacterHitGroundServer orig, GlobalEventManager self, CharacterBody characterBody, CharacterMotor.HitGroundInfo hitGroundInfo)
        {
            orig(self, characterBody, hitGroundInfo);
            if (NetworkServer.active && characterBody && characterBody.HasBuff(WallclingBonusAirborne)) characterBody.RemoveBuff(WallclingBonusAirborne);
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(WallclingBonus) || sender.HasBuff(WallclingBonusAirborne))
            {
                args.attackSpeedMultAdd += 0.3f;
            }

            if (sender.HasBuff(ParachuteSpeed)) {
                args.moveSpeedMultAdd += 0.2f;
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