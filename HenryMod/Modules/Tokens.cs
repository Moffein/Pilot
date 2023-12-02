using R2API;
using System;

namespace Pilot.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string prefix = PilotPlugin.DEVELOPER_PREFIX + "_PILOT_BODY_";
            LanguageAPI.Add(prefix + "NAME", "Pilot");
            LanguageAPI.Add(prefix + "DESCRIPTION", "The Pilot is an airborne fighter that excels at raining damage from above.");
            LanguageAPI.Add(prefix + "SUBTITLE", "Airborne Ace");
            LanguageAPI.Add(prefix + "LORE", "Pilot was born with a special power.");
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", "..and so he left, with his dreams of the sky below.");
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", "..and so he vanished, with the skies forever out of reach.");

            LanguageAPI.Add(prefix + "PRIMARY_NAME", "Clusterfire");
            LanguageAPI.Add(prefix + "PRIMARY_DESCRIPTION", "Fire your weapon for <style=cIsDamage>180% damage</style>. Every third hit <style=cIsDamage>pierces</style> and deals <style=cIsDamage>360% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_ALT_NAME", "Rapidfire");
            LanguageAPI.Add(prefix + "PRIMARY_ALT_DESCRIPTION", "Rapidly fire your weapon for <style=cIsDamage>140% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_NAME", "Target Acquired!");
            LanguageAPI.Add(prefix + "SECONDARY_DESCRIPTION", "Transform your weapon into a <style=cIsDamage>piercing smartgun</style> that deals <style=cIsDamage>3x200% damage</style>. Hold up to 2.");

            LanguageAPI.Add(prefix + "SECONDARY_ALT_NAME", "Bombs Away!");
            LanguageAPI.Add(prefix + "SECONDARY_ALT_DESCRIPTION", "Transform your weapon into a <style=cIsDamage>energy grenade launcher</style> that deals <style=cIsDamage>3x200% damage</style>. Hold up to 2.");

            LanguageAPI.Add(prefix + "UTILITY_NAME", "Rapid Deployment");
            LanguageAPI.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Launch into the air and <style=cIsUtility>activate your parachute</style>.");

            LanguageAPI.Add(prefix + "UTILITY_ALT_NAME", "Aerobatics");
            LanguageAPI.Add(prefix + "UTILITY_ALT_DESCRIPTION", "<style=cIsUtility>Dash forwards</style>. Can be retriggered if you <style=cIsUtility>hit a wall</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_NAME", "Air Strike");
            LanguageAPI.Add(prefix + "SPECIAL_DESCRIPTION", "Place leave a bomb that <style=cIsDamage>knocks enemies into the air</style> for <style=cIsDamage>320% damage</style>. Can trigger <style=cIsDamage>6</style> times. Hold up to 2.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_NAME", "Air Raid");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_DESCRIPTION", "Place a bomb that <style=cIsDamage>knocks enemies into the air</style> for <style=cIsDamage>390% damage</style>. Can trigger <style=cIsDamage>9</style> times. Hold up to 2.");

            LanguageAPI.Add(prefix + "SPECIAL_ALT_NAME", "Aerial Support");
            LanguageAPI.Add(prefix + "SPECIAL_ALT_DESCRIPTION", "Bombard an area for <style=cIsDamage>5x320% damage</style>, <style=cIsDamage>knocking enemies into the air</style> on the final hit.");

            LanguageAPI.Add(prefix + "SPECIAL_ALT_SCEPTER_NAME", "Aerial Barrage");
            LanguageAPI.Add(prefix + "SPECIAL_ALT_SCEPTER_DESCRIPTION", "Bombard an area for <style=cIsDamage>8x320% damage</style>, <style=cIsDamage>knocking enemies into the air</style> on the final hit.");
        }
    }
}