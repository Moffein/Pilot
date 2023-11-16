using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Pilot.Modules
{
    public static class DamageTypes
    {
        public static R2API.DamageAPI.ModdedDamageType AirstrikeKnockup;

        internal static void RegisterDamageTypes()
        {
            AirstrikeKnockup = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.HasModdedDamageType(AirstrikeKnockup))
            {
                if (!self.body.isFlying)
                {
                    damageInfo.force.y = 2400f;

                    if (self.body.rigidbody)
                    {
                        float forceMult = Mathf.Max(self.body.rigidbody.mass / 100f, 1f);
                        damageInfo.force *= forceMult;
                    }

                    if (self.body.characterMotor)
                    {
                        self.body.characterMotor.velocity.y = 0f;
                    }
                }
                else
                {
                    damageInfo.force.y = -1200f;
                }
            }
            orig(self, damageInfo);
        }
    }
}
