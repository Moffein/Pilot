using MoffeinPilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.MoffeinPilot.Airstrike
{
    public class PlaceAirstrikeAlt : PlaceAirstrike
    {
        public static new float damageCoefficient = 4f;
        public static new GameObject projectilePrefab;

        protected override float GetDamageCoefficient()
        {
            return PlaceAirstrikeAlt.damageCoefficient;
        }

        protected override GameObject GetProjectile()
        {
            return PlaceAirstrikeAlt.projectilePrefab;
        }
    }
}
