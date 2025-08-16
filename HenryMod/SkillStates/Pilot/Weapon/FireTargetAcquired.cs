﻿using EntityStates;
using MoffeinPilot.Content.Components;
using MoffeinPilot.Modules;
using MoffeinPilot.Modules.Survivors;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPilot.Weapon
{
    public class FireTargetAcquired : BaseState
    {
        public static float selfKnockbackForce = 450f;

        public static float damageCoefficient = 1.6f;
        public static float force = 450f;
        public static float baseDuration = 0.3f;
        public static float baseShotDuration = 0.1f;
        public static string attackSoundString = "Play_MoffeinPilot_Secondary_FireBurst";
        public static int baseShotCount = 3;
        public static string muzzleString = "Muzzle";
        public static float spreadBloom = 0f;
        public static float recoil = 1f;

        public static GameObject tracerEffectPrefab;
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXFMJ.prefab").WaitForCompletion();

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
                pilotController.BeginAutoAim();
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
            duration = FireTargetAcquired.baseDuration / this.attackSpeedStat;
            shotDuration = FireTargetAcquired.baseShotDuration / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
                if (isAuthority && !characterBody.HasBuff(Buffs.ParachuteSpeed))
                {
                    characterBody.isSprinting = false;
                }
            }

            PilotSurvivor.HandleLuminousShotServer(base.characterBody);
            FireBullet();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.characterMotor && base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y = 0;
            if (shotCount < FireTargetAcquired.baseShotCount)
            {
                shotStopwatch += Time.fixedDeltaTime;
                if (shotStopwatch >= shotDuration)
                {
                    FireBullet();
                }
            }
            else
            {
                if (base.isAuthority && base.fixedAge >= this.duration && shotCount >= FireTargetAcquired.baseShotCount)
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
            if (pilotController)
            {
                pilotController.EndAutoAim();
            }
            base.OnExit();
        }

        private void FireBullet()
        {
            shotStopwatch = 0f;
            shotCount++;
            Util.PlaySound(FireTargetAcquired.attackSoundString, base.gameObject);
            //EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, muzzleString, false);
            //base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Additive", "Shoot1", "ShootGun.playbackRate", shotDuration * 2f);
            if (base.isAuthority)
            {
                if (characterBody && characterBody.HasBuff(Buffs.ParachuteSpeed))
                {
                    characterBody.isSprinting = false;
                }

                Ray aimRay = base.GetAimRay();
                if (pilotController)
                {
                    pilotController.UpdateAutoAim();
                    pilotController.UpdateIndicator();
                }

                BulletAttack ba = new BulletAttack
                {
                    aimVector = AutoTarget(aimRay),
                    origin = aimRay.origin,
                    damage = damageCoefficient * damageStat,
                    damageType = DamageTypeCombo.GenericSecondary,
                    damageColorIndex = DamageColorIndex.Default,
                    minSpread = 0f,
                    maxSpread = 0f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = FireTargetAcquired.force,
                    isCrit = this.crit,
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 1f,
                    radius = 2f,
                    weapon = base.gameObject,
                    tracerEffectPrefab = tracerEffectPrefab,
                    hitEffectPrefab = hitEffectPrefab,
                    stopperMask = LayerIndex.world.mask
                };
                ba.damageType.AddModdedDamageType(DamageTypes.KeepAirborne);
                ba.damageType.AddModdedDamageType(DamageTypes.BonusDamageToAirborne);
                ba.Fire();
                if (applySelfForce && FireTargetAcquired.selfKnockbackForce != 0f //pilotController && pilotController.isParachuting && 
                    && base.characterMotor && !base.characterMotor.isGrounded && base.characterMotor.velocity != Vector3.zero)
                    base.characterMotor.ApplyForce(-FireTargetAcquired.selfKnockbackForce * aimRay.direction, false, false);
            }
            base.AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            if (base.characterBody) base.characterBody.AddSpreadBloom(spreadBloom); //Spread is cosmetic. Skill is always perfectly accurate.
        }

        private Vector3 AutoTarget(Ray aimRay)
        {
            Vector3 aimDirection = aimRay.direction;

            HurtBox bestTarget = pilotController ? pilotController.GetAutoaimHurtbox() : null;
            if (bestTarget)
            {
                aimDirection = (bestTarget.transform.position - aimRay.origin);
                aimDirection.Normalize();
            }

            return aimDirection;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
