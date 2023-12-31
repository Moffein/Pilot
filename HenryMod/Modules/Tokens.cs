﻿using R2API;
using System;

namespace MoffeinPilot.Modules
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
            LanguageAPI.Add(prefix + "PRIMARY_DESCRIPTION", "Fire your weapon for <style=cIsDamage>160% damage</style>. Every third hit <style=cIsDamage>explodes</style> and deals <style=cIsDamage>320% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_ALT_NAME", "Rapidfire");
            LanguageAPI.Add(prefix + "PRIMARY_ALT_DESCRIPTION", "Rapidly fire your weapon for <style=cIsDamage>140% damage</style>.");// Reload every 30 shots.

            LanguageAPI.Add(prefix + "PRIMARY_SILENCER_NAME", "Suppresedfire");
            LanguageAPI.Add(prefix + "PRIMARY_SILENCER_DESCRIPTION", "<style=cIsDamage>Slayer</style>. Fire your silenced pistol for <style=cIsDamage>100% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_NAME", "Target Acquired!");
            LanguageAPI.Add(prefix + "SECONDARY_DESCRIPTION", "Transform your weapon into a <style=cIsDamage>piercing smartgun</style> that deals <style=cIsDamage>3x190% damage</style>. Hold up to 2.");

            LanguageAPI.Add(prefix + "SECONDARY_ALT_NAME", "Bombs Away!");
            LanguageAPI.Add(prefix + "SECONDARY_ALT_DESCRIPTION", "Transform your weapon into a <style=cIsDamage>energy grenade launcher</style> that deals <style=cIsDamage>3x190% damage</style>. Hold up to 2.");

            LanguageAPI.Add(prefix + "UTILITY_NAME", "Rapid Deployment");
            LanguageAPI.Add(prefix + "UTILITY_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Launch into the air and <style=cIsUtility>activate your parachute</style>.");

            LanguageAPI.Add(prefix + "UTILITY_ALT_NAME", "Aerobatics");
            LanguageAPI.Add(prefix + "UTILITY_ALT_DESCRIPTION", "<style=cIsUtility>Dash forwards</style> and <style=cIsUtility>cling to a wall</style>. Boost forwards when <style=cIsUtility>jumping off the wall</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_NAME", "Air Strike");
            LanguageAPI.Add(prefix + "SPECIAL_DESCRIPTION", "<style=cIsUtility>Dash</style> and place a bomb <style=cIsDamage>at your feet</style> that <style=cIsDamage>knocks enemies into the air</style> for <style=cIsDamage>600% damage</style>. Can trigger <style=cIsDamage>3</style> times. Hold up to 2.");

            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_NAME", "Air Raid");
            LanguageAPI.Add(prefix + "SPECIAL_SCEPTER_DESCRIPTION", "<style=cIsUtility>Dash</style> and place a bomb <style=cIsDamage>at your feet</style> that <style=cIsDamage>knocks enemies into the air</style> for <style=cIsDamage>800% damage</style>. Can trigger <style=cIsDamage>6</style> times. Hold up to 2.");

            LanguageAPI.Add(prefix + "SPECIAL_ALT_NAME", "Aerial Support");
            LanguageAPI.Add(prefix + "SPECIAL_ALT_DESCRIPTION", "Bombard an area for <style=cIsDamage>4x400% damage</style>, <style=cIsDamage>knocking enemies into the air</style> on the final hit.");

            LanguageAPI.Add(prefix + "SPECIAL_ALT_SCEPTER_NAME", "Aerial Barrage");
            LanguageAPI.Add(prefix + "SPECIAL_ALT_SCEPTER_DESCRIPTION", "Bombard an area for <style=cIsDamage>8x400% damage</style>, <style=cIsDamage>knocking enemies into the air</style> on the final hit.");
        }
    }
}