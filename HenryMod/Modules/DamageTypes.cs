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

                bool isAir = self.body.isFlying || !(self.body.characterMotor && self.body.characterMotor.isGrounded);
                float forceMult = 1f;

                if (self.body.rigidbody)
                {
                    forceMult = Mathf.Max(self.body.rigidbody.mass / 100f, 1f);
                    /*if (isAir)
                    {
                        forceMult = Mathf.Min(7.5f, forceMult) * -1f; ;
                    }*/
                }

                //damageInfo.force = forceMult * Vector3.up * 2700f;
                if (!isAir)
                {
                    damageInfo.force = 2700f * forceMult * Vector3.up;
                }
                else if (!self.body.isFlying)
                {
                    self.body.characterMotor.velocity.y = 17f;
                }

                if (self.body.characterMotor)
                {
                    //self.body.characterMotor.velocity.y = 0f;
                    self.body.characterMotor.velocity.x = 0f;
                    self.body.characterMotor.velocity.z = 0f;
                    if (isAir && !self.body.isFlying)
                    {
                        if (self.body.characterMotor.velocity.y < 17f) self.body.characterMotor.velocity.y = 17f;
                    }
                    else
                    {
                        self.body.characterMotor.velocity.y = 0f;
                    }
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
