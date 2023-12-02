using EntityStates;
using Pilot.Content.Components;
using Pilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Weapon
{
    public class FireColdWar : BaseState
    {
        public static float selfKnockbackForce = 450f;

        public static float damageCoefficient = 2f;
        public static float force = 450f;
        public static float baseDuration = 0.3f;
        public static float baseShotDuration = 0.1f;
        public static string attackSoundString = "Play_Pilot_Secondary_FireBurst";
        public static int baseShotCount = 3;
        public static string muzzleString = "";
        public static float spreadBloom = 0f;
        public static float recoil = 1f;
        public static GameObject projectilePrefab;

        private int shotCount;
        private float duration;
        private float shotDuration;
        private float shotStopwatch;
        private bool crit;
        private PilotController pilotController;
        private bool applySelfForce;

        public override void OnEnter()
        {
            base.OnEnter();

            if (base.characterMotor && base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y = 0;

            applySelfForce = true;
            pilotController = base.GetComponent<PilotController>();
            if (pilotController)
            {
                pilotController.ConsumeSecondaryStock(1);
                applySelfForce = !pilotController.isWavedashing;
            }

            if (base.characterBody && base.skillLocator && base.skillLocator.secondary)
            {
                base.characterBody.OnSkillActivated(base.skillLocator.secondary);
            }

            crit = base.RollCrit();
            shotCount = 0;
            shotStopwatch = 0f;
            duration = FireColdWar.baseDuration / this.attackSpeedStat;
            shotDuration = FireColdWar.baseShotDuration / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }

            FireProjectile();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterMotor && base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y = 0;
            if (shotCount < FireColdWar.baseShotCount)
            {
                shotStopwatch += Time.fixedDeltaTime;
                if (shotStopwatch >= shotDuration)
                {
                    FireProjectile();
                }
            }
            else
            {
                if (base.isAuthority && base.fixedAge >= this.duration && shotCount >= FireColdWar.baseShotCount)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                base.characterBody.SetSpreadBloom(0f, false);
            }
            base.OnExit();
        }

        private void FireProjectile()
        {
            shotStopwatch = 0f;
            shotCount++;
            Util.PlaySound(FireColdWar.attackSoundString, base.gameObject);
            //EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, muzzleString, false);
            //base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                ProjectileManager.instance.FireProjectile(
                    FireColdWar.projectilePrefab,
                    aimRay.origin,
                    Util.QuaternionSafeLookRotation(aimRay.direction),
                    base.gameObject,
                    this.damageStat * FireColdWar.damageCoefficient,
                    FireColdWar.force,
                    this.crit);

                if (applySelfForce && FireColdWar.selfKnockbackForce != 0f //pilotController && pilotController.isParachuting && 
                    && base.characterMotor && !base.characterMotor.isGrounded && base.characterMotor.velocity != Vector3.zero)
                    base.characterMotor.ApplyForce(-FireColdWar.selfKnockbackForce * aimRay.direction, false, false);
            }
            base.AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            if (base.characterBody) base.characterBody.AddSpreadBloom(spreadBloom); //Spread is cosmetic. Skill is always perfectly accurate.
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
