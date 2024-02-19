using BepInEx.Configuration;
using MoffeinPilot.Modules.Survivors;
using UnityEngine;

namespace MoffeinPilot.Modules
{
    public static class Config
    {
        public static void ReadConfig()
        {
            EntityStates.MoffeinPilot.Weapon.ClusterFire.useLaser = PilotPlugin.instance.Config.Bind("Settings", "Cluster Fire - Piercing Shot", false, "Cluster Fire pierces on the 3rd shot like in Returns.");
            EntityStates.MoffeinPilot.Weapon.ClusterFire.useLaser.SettingChanged += UseLaser_SettingChanged;

            EntityStates.MoffeinPilot.Parachute.DeployParachute.enableParachuteFade = PilotPlugin.instance.Config.Bind("Settings", "Parachute Fade", true, "Pilot's parachute becomes transparent when looking downwards.");
            EntityStates.MoffeinPilot.Parachute.DeployParachute.holdToAscend = PilotPlugin.instance.Config.Bind("Controls", "Rapid Deployment - Hold to Ascend", false, "Rapid Deployment requires you to hold the Utility button to ascend, and ends early if you let go.");

            EntityStates.MoffeinPilot.Parachute.DashGround.onlyDashBackwards = PilotPlugin.instance.Config.Bind("Controls", "Air Strike - Only Dash Backwards", false, "Air Strike ground dash only moves backwards like in Returns.");
            EntityStates.MoffeinPilot.Airstrike.PlaceAirstrikeAlt.useShorthop = PilotPlugin.instance.Config.Bind("Controls", "Aerial Support - Shorthop", true, "Using this skill midair gives a vertical boost.");

            ModCompat.SetupOptions();
        }

        private static void UseLaser_SettingChanged(object sender, System.EventArgs e)
        {
            if (!PilotSurvivor.SkillDefs.Primaries.ClusterFire) return;
            PilotSurvivor.SkillDefs.Primaries.ClusterFire.skillDescriptionToken = EntityStates.MoffeinPilot.Weapon.ClusterFire.useLaser.Value ? "MOFFEIN_PILOT_BODY_PRIMARY_RETURNS_DESCRIPTION" : "MOFFEIN_PILOT_BODY_PRIMARY_DESCRIPTION";
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true)
        {

            return PilotPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}