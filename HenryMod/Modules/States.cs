using Pilot.SkillStates;
using Pilot.SkillStates.BaseStates;
using System.Collections.Generic;
using System;

namespace Pilot.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Weapon.RapidFire));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Weapon.ClusterFire));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.FireSelect.TargetAcquired));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Weapon.FireTargetAcquired));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.FireSelect.ColdWar));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Weapon.FireColdWar));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.DeployParachute));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.Glide));

            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.AerobaticsDashBase));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.AerobaticsDashEntry));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.Wallbounce));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.Wavedash));

            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Airstrike.PlaceAirstrike));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Airstrike.DashGround));    //unused
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Airstrike.DashAir));   //unused
        }
    }
}