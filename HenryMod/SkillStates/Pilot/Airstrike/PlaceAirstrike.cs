using Pilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.BulletAttack;

namespace EntityStates.MoffeinPilot.Airstrike
{
    public class PlaceAirstrike : BaseState
    {
        public static string attackSoundString = "Play_huntress_shift_mini_blink";
        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainAirstrikeProjectile1.prefab").WaitForCompletion();
        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/CaptainDefenseMatrix/TracerCaptainDefenseMatrix.prefab").WaitForCompletion();
        public static string muzzleName = "";   //Where the laser effect originates from.
        public static float damageCoefficient = 3.2f;   //damage per explosion

        private bool placedProjectile;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(PlaceAirstrike.attackSoundString, base.gameObject);
            placedProjectile = false;

            if (base.isAuthority)
            {
                PlaceProjectile();

                /*bool shouldBlink = isGrounded && characterMotor.velocity != Vector3.zero;
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
                }*/
                if (base.characterMotor && !base.characterMotor.isGrounded) base.SmallHop(base.characterMotor, 24f);
                this.outer.SetNextStateToMain();
                return;
            }
        }

        protected virtual float GetDamageCoefficient()
        {
            return PlaceAirstrike.damageCoefficient;
        }

        protected virtual GameObject GetProjectile()
        {
            return PlaceAirstrike.projectilePrefab;
        }

        private void PlaceProjectile()
        {
            Ray aimRay = base.GetAimRay();

            BulletAttack ba = new BulletAttack
            {
                tracerEffectPrefab = PlaceAirstrike.tracerEffectPrefab,
                damage = 0f,
                procCoefficient = 0f,
                damageType = DamageType.Silent | DamageType.NonLethal,
                owner = base.gameObject,
                aimVector = aimRay.direction,
                isCrit = false,
                minSpread = 0f,
                maxSpread = 0f,
                origin = aimRay.origin,
                maxDistance = 2000f,
                muzzleName = PlaceAirstrike.muzzleName,
                radius = 0.2f,
                hitCallback = AirstrikeHitCallback
            };
            ba.Fire();
        }

        private bool AirstrikeHitCallback(BulletAttack bulletRef, ref BulletHit hitInfo)
        {
            if (hitInfo.point != null && !placedProjectile)
            {
                placedProjectile = true;
                PlaceAirstrike.PlaceProjectile(GetProjectile(), this.damageStat * GetDamageCoefficient(), base.gameObject, base.RollCrit(), hitInfo.point);
            }

            return false;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static void PlaceProjectile(GameObject projectilePrefab, float damage, GameObject attacker, bool crit, Vector3 position)
        {
            FireProjectileInfo projInfo = new FireProjectileInfo
            {
                projectilePrefab = projectilePrefab,
                crit = crit,
                damage = damage,
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
