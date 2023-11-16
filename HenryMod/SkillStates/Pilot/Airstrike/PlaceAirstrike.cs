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
        public static float damageCoefficient = 4f; //Damage per explosion.
        public static string attackSoundString = "Play_huntress_shift_mini_blink";
        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainAirstrikeProjectile1.prefab").WaitForCompletion();
        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/CaptainDefenseMatrix/TracerCaptainDefenseMatrix.prefab").WaitForCompletion();
        public static string muzzleName = "";   //Where the laser effect originates from.


        //Scepter uses upgraded projectile
        public virtual GameObject GetProjectilePrefab()
        {
            return PlaceAirstrike.projectilePrefab;
        }

        //Scepter uses higher damage
        public virtual float GetDamageCoefficient()
        {
            return PlaceAirstrike.damageCoefficient;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(PlaceAirstrike.attackSoundString, base.gameObject);

            if (base.isAuthority)
            {
                PlaceProjectile();
            }

            bool shouldBlink = isGrounded && characterMotor.velocity != Vector3.zero;
            if (shouldBlink)
            {
                this.outer.SetNextState(new DashGround());
                return;
            }
            else if (!isGrounded)
            {
                this.outer.SetNextState(new DashAir());
                return;
            }
            else
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void PlaceProjectile()
        {
            GameObject proj = GetProjectilePrefab();
            float damageCoeff = GetDamageCoefficient();
            if (!proj) return;

            Ray aimRay = base.GetAimRay();


            //Need to do this to prevent self-hits
            RaycastHit[] raycastHits = Physics.RaycastAll(aimRay, 2000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask);
            List<RaycastHit> hitList = raycastHits.Where(hit => !(hit.collider && hit.collider.gameObject == base.gameObject)).ToList();
            hitList.Sort(delegate(RaycastHit hit1, RaycastHit hit2)
            {
                float distSqr1 = hit1.point != null ? (hit1.point - aimRay.origin).sqrMagnitude : Mathf.Infinity;
                float distSqr2 = hit2.point != null ? (hit2.point - aimRay.origin).sqrMagnitude : Mathf.Infinity;

                if (distSqr1 == distSqr2) return 0;
                if (distSqr1 < distSqr2) return -1;
                return 1;
            });

            /*RaycastHit raycastHit;
            bool successfulRaycast = Physics.Raycast(aimRay, out raycastHit, 2000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask);*/

            if (hitList.Count > 0)
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
            }

            new BulletAttack
            {
                tracerEffectPrefab = PlaceAirstrike.tracerEffectPrefab,
                damage = 0f,
                procCoefficient = 0f,
                damageType = DamageType.Silent | DamageType.NonLethal,
                stopperMask = LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask,
                owner = base.gameObject,
                aimVector = aimRay.direction,
                isCrit = false,
                minSpread = 0f,
                maxSpread = 0f,
                origin = aimRay.origin,
                maxDistance = 2000f,
                muzzleName = PlaceAirstrike.muzzleName
            }.Fire();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
