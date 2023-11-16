using Pilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.Pilot.Airstrike
{
    public class PlaceAirstrike : BaseState
    {
        public static float damageCoefficient = 3.2f; //Damage per explosion.
        public static string attackSoundString = "Play_huntress_shift_mini_blink";
        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainAirstrikeProjectile1.prefab").WaitForCompletion();
        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/CaptainDefenseMatrix/TracerCaptainDefenseMatrix.prefab").WaitForCompletion();
        public static string muzzleName = "";   //Where the laser effect originates from.

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(PlaceAirstrike.attackSoundString, base.gameObject);

            if (base.isAuthority)
            {
                PlaceProjectile();

                bool shouldBlink = isGrounded && characterMotor.velocity != Vector3.zero;
                if (shouldBlink)
                {
                    this.outer.SetNextState(new DashGround());
                    return;
                }
                else if (!isGrounded)
                {
                    //this.outer.SetNextState(new DashAir());
                    if (base.characterMotor) base.SmallHop(base.characterMotor, 24f);
                    this.outer.SetNextStateToMain();
                    return;
                }
                else
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        private void PlaceProjectile()
        {
            Ray aimRay = base.GetAimRay();
            //Even with all this, the air strike still sometimes spawns on top of you.
            /*RaycastHit[] raycastHits = Physics.RaycastAll(aimRay, 2000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask);
            List<RaycastHit> hitList = raycastHits.Where(hit => !(hit.collider && hit.collider.gameObject == base.gameObject)).ToList();
            hitList.Sort(delegate(RaycastHit hit1, RaycastHit hit2)
            {
                float distSqr1 = hit1.point != null ? (hit1.point - aimRay.origin).sqrMagnitude : Mathf.Infinity;
                float distSqr2 = hit2.point != null ? (hit2.point - aimRay.origin).sqrMagnitude : Mathf.Infinity;

                if (distSqr1 == distSqr2) return 0;
                if (distSqr1 < distSqr2) return -1;
                return 1;
            });*/

            /*RaycastHit raycastHit;
            bool successfulRaycast = Physics.Raycast(aimRay, out raycastHit, 2000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask);*/

            /*if (hitList.Count > 0)
            {

                RaycastHit firstRaycast = hitList.FirstOrDefault();
                FireProjectileInfo projInfo = new FireProjectileInfo
                {
                    projectilePrefab = proj,
                    crit = base.RollCrit(),
                    damage = base.damageStat * damageCoeff,
                    damageColorIndex = DamageColorIndex.Default,
                    force = 0f,
                    owner = base.gameObject,
                    position = firstRaycast.point,
                    procChainMask = default,
                    rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f),
                    speedOverride = 0f
                };

                ProjectileManager.instance.FireProjectile(projInfo);
            }*/

            BulletAttack ba = new BulletAttack
            {
                tracerEffectPrefab = PlaceAirstrike.tracerEffectPrefab,
                damage = 0f,
                procCoefficient = 0.1f,
                damageType = DamageType.Silent | DamageType.NonLethal,
                owner = base.gameObject,
                aimVector = aimRay.direction,
                isCrit = base.RollCrit(),
                minSpread = 0f,
                maxSpread = 0f,
                origin = aimRay.origin,
                maxDistance = 2000f,
                muzzleName = PlaceAirstrike.muzzleName,
                radius = 0.2f
            };
            ba.AddModdedDamageType(DamageTypes.PlaceAirstrike);
            ba.Fire();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static void PlaceProjectile(GameObject attacker, bool crit, Vector3 position)
        {
            CharacterBody attackerBody = attacker.GetComponent<CharacterBody>();
            if (!attackerBody) return;

            FireProjectileInfo projInfo = new FireProjectileInfo
            {
                projectilePrefab = PlaceAirstrike.projectilePrefab,
                crit = crit,
                damage = attackerBody.damage * PlaceAirstrike.damageCoefficient,
                damageColorIndex = DamageColorIndex.Default,
                force = 0f,
                owner = attacker,
                position = position,
                procChainMask = default,
                rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f),
                speedOverride = 0f
            };

            ProjectileManager.instance.FireProjectile(projInfo);
        }
    }
}
