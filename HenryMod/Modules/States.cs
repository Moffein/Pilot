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
        }
    }
}