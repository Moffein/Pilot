using Pilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EntityStates.Pilot.Airstrike
{
    public class PlaceAirstrikeAlt : PlaceAirstrike
    {
        public static new float damageCoefficient = 3.2f; //Damage per explosion.
        public static new GameObject projectilePrefab;

        //Jank way of placing the projectile due to issues with accidental self targeting when doing a raycast.
        protected override void ModifyBulletAttack(BulletAttack bulletAttack)
        {
            bulletAttack.AddModdedDamageType(DamageTypes.PlaceAirstrikeAltImpact);
        }
    }
}
