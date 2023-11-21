using EntityStates.Pilot.Airstrike;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Pilot.Modules
{
    public static class DamageTypes
    {
        public static R2API.DamageAPI.ModdedDamageType AirstrikeKnockup;
        public static R2API.DamageAPI.ModdedDamageType KeepAirborne;
        public static R2API.DamageAPI.ModdedDamageType PlaceAirstrikeImpact;
        public static R2API.DamageAPI.ModdedDamageType PlaceAirstrikeScepterImpact;

        internal static void RegisterDamageTypes()
        {
            AirstrikeKnockup = DamageAPI.ReserveDamageType();
            KeepAirborne = DamageAPI.ReserveDamageType();
            PlaceAirstrikeImpact = DamageAPI.ReserveDamageType();
            PlaceAirstrikeScepterImpact = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
        }

        private static void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            orig(self, damageInfo, hitObject);
            if (damageInfo.HasModdedDamageType(PlaceAirstrikeImpact))
            {
                PlaceAirstrike.PlaceProjectile(PlaceAirstrike.projectilePrefab, PlaceAirstrike.damageCoefficient, damageInfo.attacker, damageInfo.crit, damageInfo.position);
            }
            if (damageInfo.HasModdedDamageType(PlaceAirstrikeScepterImpact))
            {
                PlaceAirstrike.PlaceProjectile(PlaceAirstrikeScepter.scepterProjectilePrefab, PlaceAirstrikeScepter.scepterDamageCoefficient, damageInfo.attacker, damageInfo.crit, damageInfo.position);
            }
        }

        //Velocity modification will only work on server-side things.
        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.HasModdedDamageType(AirstrikeKnockup))
            {
                if (!self.body.isFlying)
                {
                    damageInfo.force.y = 2700f;

                    if (self.body.rigidbody)
                    {
                        float forceMult = Mathf.Max(self.body.rigidbody.mass / 100f, 1f);
                        damageInfo.force *= forceMult;
                    }

                    if (self.body.characterMotor)
                    {
                        //self.body.characterMotor.velocity.y = 0f;
                        self.body.characterMotor.velocity = Vector3.zero;
                    }
                }
                else
                {
                    damageInfo.force.y = -1350f;
                }
            }

            if (damageInfo.HasModdedDamageType(KeepAirborne))
            {
                if (!self.body.isFlying && self.body.characterMotor && !self.body.characterMotor.isGrounded)
                {
                    if (self.body.characterMotor.velocity.y < 6f)
                    {
                        self.body.characterMotor.velocity.y = 6f;
                    }
                }
            }

            orig(self, damageInfo);
        }
    }
}
