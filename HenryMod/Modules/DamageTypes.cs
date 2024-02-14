using EntityStates.MoffeinPilot.Airstrike;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MoffeinPilot.Modules
{
    public static class DamageTypes
    {
        public static R2API.DamageAPI.ModdedDamageType SlayerExceptItActuallyWorks;
        public static R2API.DamageAPI.ModdedDamageType AirstrikeKnockup;
        public static R2API.DamageAPI.ModdedDamageType KeepAirborne;

        internal static void RegisterDamageTypes()
        {
            SlayerExceptItActuallyWorks = DamageAPI.ReserveDamageType();
            AirstrikeKnockup = DamageAPI.ReserveDamageType();
            KeepAirborne = DamageAPI.ReserveDamageType();
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        //Velocity modification will only work on server-side things.
        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.HasModdedDamageType(SlayerExceptItActuallyWorks))
            {
                damageInfo.RemoveModdedDamageType(SlayerExceptItActuallyWorks);
                damageInfo.damage *= Mathf.Lerp(3f, 1f, self.combinedHealthFraction);
            }

            //This check is a bandaid fix for clientside blast attacks being bugged with damageAPI
            if (damageInfo.HasModdedDamageType(AirstrikeKnockup) && damageInfo.attacker != damageInfo.inflictor)
            {
                bool isAir = self.body.isFlying || !(self.body.characterMotor && self.body.characterMotor.isGrounded);
                float forceMult = 1f;

                if (self.body.rigidbody) forceMult = Mathf.Max(self.body.rigidbody.mass / 100f, 1f);

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
