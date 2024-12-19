using MoffeinPilot.Content.Components;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;
using MoffeinPilot.Modules;

namespace EntityStates.MoffeinPilot.Weapon
{
    public class FireSilencedPistol : BaseState
    {
        //Railgunner 300 for 5 shots per second
        public static float selfKnockbackForce = 0f;

        public static float damageCoefficient = 1f;
        public static float weakpointMultiplier = 1f;
        public static GameObject weakpointEffectPrefab;

        public static float force = 400f;
        public static float baseDuration = 0.25f;
        public static float spreadBloomValue = 0.75f;
        public static float recoilAmplitude = 1f;
        public static string attackSoundString = "Play_MoffeinPilot_Silencer";
        public static string muzzleName = "Muzzle";
        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoDefault.prefab").WaitForCompletion();
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommando.prefab").WaitForCompletion();
        public static GameObject muzzleEffectPrefab;

        private PilotController pilotController;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            pilotController = base.GetComponent<PilotController>();

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 3f, false);
            duration = baseDuration / this.attackSpeedStat;
            base.PlayAnimation("Gesture, Additive", "Shoot1", "ShootGun.playbackRate", duration * 2f);
            if (muzzleEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(FireSilencedPistol.muzzleEffectPrefab, base.gameObject, FireSilencedPistol.muzzleName, false);
            }
            if (base.isAuthority)
            {
                BulletAttack ba = new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0f,
                    maxSpread = 0f,
                    bulletCount = 1u,
                    damage = FireSilencedPistol.damageCoefficient * this.damageStat,
                    force = FireSilencedPistol.force,
                    tracerEffectPrefab = FireSilencedPistol.tracerEffectPrefab,
                    muzzleName = FireSilencedPistol.muzzleName,
                    hitEffectPrefab = FireSilencedPistol.hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    radius = 0.5f,
                    smartCollision = true,
                    damageType = (DamageTypeCombo) DamageType.BonusToLowHealth | DamageSource.Primary,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    procCoefficient = 1f
                };
                if (weakpointMultiplier > 1f)
                {
                    ba.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                    {
                        if (BulletAttack.IsSniperTargetHit(hitInfo))
                        {
                            damageInfo.damage *= weakpointMultiplier;
                            damageInfo.damageColorIndex = DamageColorIndex.Sniper;

                            if (weakpointEffectPrefab)
                            {
                                EffectData effectData = new EffectData
                                {
                                    origin = hitInfo.point,
                                    rotation = Quaternion.LookRotation(-hitInfo.direction)
                                };
                                effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                                EffectManager.SpawnEffect(weakpointEffectPrefab, effectData, true);
                            }

                            RoR2.Util.PlaySound("Play_MoffeinPilot_Headshot", base.gameObject);
                        }
                    };
                }
                ba.Fire();
                if (FireSilencedPistol.selfKnockbackForce != 0f//pilotController && pilotController.isParachuting && 
                    && base.characterMotor && !base.characterMotor.isGrounded && base.characterMotor.velocity != Vector3.zero)
                    base.characterMotor.ApplyForce(-FireSilencedPistol.selfKnockbackForce * aimRay.direction, false, false);
            }
            base.AddRecoil(-0.4f * FireSilencedPistol.recoilAmplitude, -0.8f * FireSilencedPistol.recoilAmplitude, -0.3f * FireSilencedPistol.recoilAmplitude, 0.3f * FireSilencedPistol.recoilAmplitude);
            Util.PlaySound(FireSilencedPistol.attackSoundString, base.gameObject);
            base.characterBody.AddSpreadBloom(FireSilencedPistol.spreadBloomValue);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
