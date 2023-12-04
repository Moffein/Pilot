using MoffeinPilot.SkillStates;
using MoffeinPilot.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace MoffeinPilot.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Weapon.RapidFire));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Weapon.ReloadRapidFire));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Weapon.ClusterFire));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.FireSelect.TargetAcquired));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Weapon.FireTargetAcquired));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.FireSelect.ColdWar));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Weapon.FireColdWar));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Weapon.FireSilencedPistol));

            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Parachute.DeployParachute));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Parachute.Glide));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Parachute.AerobaticsDashBase));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Parachute.AerobaticsDashEntry));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Parachute.Wallbounce));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Parachute.Wallcling));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Parachute.Wavedash));

            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Airstrike.PlaceAirstrike));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Airstrike.PlaceAirstrikeAlt));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Airstrike.PlaceAirstrikeScepter));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Airstrike.PlaceAirstrikeAltScepter));
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Airstrike.DashGround));    //unused
            Modules.Content.AddEntityState(typeof(EntityStates.MoffeinPilot.Airstrike.DashAir));   //unused
        }
    }
}