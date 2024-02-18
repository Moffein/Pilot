using BepInEx.Configuration;
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
        public static ConfigEntry<bool> useShorthop;

        protected override float GetDamageCoefficient()
        {
            return PlaceAirstrikeAlt.damageCoefficient;
        }

        protected override GameObject GetProjectile()
        {
            return PlaceAirstrikeAlt.projectilePrefab;
        }

        public override void PlaceProjectile()
        {
            Ray aimRay = base.GetAimRay();

            BulletAttack ba = new BulletAttack
            {
                tracerEffectPrefab = PlaceAirstrike.tracerEffectPrefab,
                damage = 0f,
                procCoefficient = 0f,
                damageType = DamageType.Silent | DamageType.NonLethal,
                owner = base.gameObject,
                aimVector = aimRay.direction,
                isCrit = false,
                minSpread = 0f,
                maxSpread = 0f,
                origin = aimRay.origin,
                maxDistance = 2000f,
                muzzleName = PlaceAirstrike.muzzleName,
                radius = 0.2f,
                hitCallback = base.AirstrikeHitCallback
            };
            ba.Fire();
        }

        protected override void DoPhysics()
        {
            if (useShorthop.Value && !isGrounded && base.characterMotor) base.SmallHop(base.characterMotor, 24f);
        }

        protected override void DoAnim()
        {
            base.DoAnim();
        }
    }
}
