using EntityStates.MoffeinPilot.Airstrike;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace MoffeinPilot.Modules
{
    public static class DamageTypes
    {
        public static R2API.DamageAPI.ModdedDamageType AirstrikeKnockup;
        public static R2API.DamageAPI.ModdedDamageType KeepAirborne;
        public static R2API.DamageAPI.ModdedDamageType BonusDamageToAirborne;

        internal static void RegisterDamageTypes()
        {
            AirstrikeKnockup = DamageAPI.ReserveDamageType();
            KeepAirborne = DamageAPI.ReserveDamageType();
            BonusDamageToAirborne = DamageAPI.ReserveDamageType();
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamage;
        }

        //Velocity modification will only work on server-side things.
        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
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

            if (damageInfo.HasModdedDamageType(BonusDamageToAirborne))
            {
                if (self.body.isFlying || (self.body.characterMotor && !self.body.characterMotor.isGrounded))
                {
                    damageInfo.damage *= 1.5f;
                    if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                    {
                        damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                    }
                }
            }

            orig(self, damageInfo);
        }
    }
}
