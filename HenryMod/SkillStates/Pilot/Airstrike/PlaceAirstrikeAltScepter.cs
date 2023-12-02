using Pilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Airstrike
{
    public class PlaceAirstrikeAltScepter : PlaceAirstrikeAlt
    {
        public static new float damageCoefficient = 3.9f;
        public static new GameObject projectilePrefab;

        protected override float GetDamageCoefficient()
        {
            return PlaceAirstrikeAltScepter.damageCoefficient;
        }

        protected override GameObject GetProjectile()
        {
            return PlaceAirstrikeAltScepter.projectilePrefab;
        }
    }
}
