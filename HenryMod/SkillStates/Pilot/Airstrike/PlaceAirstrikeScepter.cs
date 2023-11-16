using Pilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Airstrike
{
    public class PlaceAirstrikeScepter : PlaceAirstrike
    {
        public static float scepterDamageCoefficient = 3.9f;
        public static GameObject scepterProjectilePrefab;

        protected override void ModifyBulletAttack(BulletAttack bulletAttack)
        {
            bulletAttack.AddModdedDamageType(DamageTypes.PlaceAirstrikeScepterImpact);
        }
    }
}
