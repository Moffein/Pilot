using MoffeinPilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPilot.Airstrike
{
    public class PlaceAirstrikeAltScepter : PlaceAirstrikeAlt
    {
        public static new float damageCoefficient = 3.6f;
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
