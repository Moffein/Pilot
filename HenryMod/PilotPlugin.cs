using BepInEx;
using MoffeinPilot.Modules.Survivors;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace MoffeinPilot
{
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "UnlockableAPI"
    })]

    public class PilotPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.EnforcerGang.Pilot";
        public const string MODNAME = "Pilot";
        public const string MODVERSION = "0.0.6";

        public const string DEVELOPER_PREFIX = "MOFFEIN";

        public static PilotPlugin instance;

        private void Awake()
        {
            instance = this;
            Log.Init(Logger);

            ModCompat.Init();
            Modules.DamageTypes.RegisterDamageTypes();
            Modules.Assets.Initialize(); // load assets and read config
            Modules.Config.ReadConfig();
            Modules.Buffs.RegisterBuffs(); // add and register custom buffs/debuffs
            Modules.Projectiles.RegisterProjectiles(); // add and register custom projectiles
            Modules.Tokens.AddTokens(); // register name tokens
            Modules.ItemDisplays.PopulateDisplays(); // collect item display prefabs for use in our display rules

            // survivor initialization
            new Modules.Survivors.PilotSurvivor().Initialize();

            // now make a content pack and add it- this part will change with the next update
            new Modules.ContentPacks().Initialize();
        }
    }
}