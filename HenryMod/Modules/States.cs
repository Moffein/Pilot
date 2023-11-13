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
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.DeployParachute));
            Modules.Content.AddEntityState(typeof(EntityStates.Pilot.Parachute.Glide));
        }
    }
}