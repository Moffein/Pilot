using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.Pilot.FireSelect
{
    public class TargetAcquired : BaseState
    {
        public static float baseEntryDuration = 0.2f;
        public static float baseExitDuration = 0.2f;
        public static GameObject crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerCrosshair.prefab").WaitForCompletion();

        public static SkillDef primaryOverrideSkill;
    }
}
