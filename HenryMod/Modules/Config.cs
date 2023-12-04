using BepInEx.Configuration;
using UnityEngine;

namespace Pilot.Modules
{
    public static class Config
    {
        public static void ReadConfig()
        {
            EntityStates.MoffeinPilot.Parachute.DeployParachute.holdToAscend = PilotPlugin.instance.Config.Bind("Controls", "Rapid Deployment - Hold to Ascend", false, "Rapid Deployment requires you to hold the Utility button to ascend, and ends early if you let go.");

            ModCompat.SetupOptions();
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return PilotPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }
    }
}