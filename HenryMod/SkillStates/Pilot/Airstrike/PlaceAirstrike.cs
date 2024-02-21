using BepInEx.Configuration;
using EntityStates.MoffeinPilot.Parachute;
using MoffeinPilot.Modules;
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
        public static string muzzleName = "Muzzle";   //Where the laser effect originates from.
        public static float damageCoefficient = 6f;   //damage per explosion
        public static float baseDuration = 0.21f;   //Slightly longer than DashGround duration so you dont whiff dashes

        private bool placedProjectile;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(PlaceAirstrike.attackSoundString, base.gameObject);
            placedProjectile = false;

            if (base.isAuthority)
            {
                PlaceProjectile();
                DoPhysics();
            }
            DoAnim();
        }

        protected virtual void DoPhysics()
        {
            bool shouldBlink = isGrounded && (characterMotor.velocity != Vector3.zero || DashGround.onlyDashBackwards.Value);
            if (shouldBlink)
            {
                EntityStateMachine parachuteMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Parachute");
                if (parachuteMachine && parachuteMachine.state.GetMinimumInterruptPriority() <= InterruptPriority.Any)
                {
                    parachuteMachine.SetNextState(new EntityStates.MoffeinPilot.Parachute.DashGround());
                }
            }
            else if (!isGrounded)
            {
                if (base.characterMotor) base.SmallHop(base.characterMotor, 24f);
            }
        }

        protected virtual void DoAnim()
        {
            if (!isGrounded) PlayAnimation("Gesture, Override", "PointDown", "Point.playbackRate", 0.4f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= PlaceAirstrike.baseDuration)
            {
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

        public virtual void PlaceProjectile()
        {
            Ray aimRay = base.GetAimRay();

            BulletAttack ba = new BulletAttack
            {
                tracerEffectPrefab = PlaceAirstrike.tracerEffectPrefab,
                damage = 0f,
                procCoefficient = 0f,
                damageType = DamageType.Silent | DamageType.NonLethal,
                owner = base.gameObject,
                aimVector = Vector3.down,//aimRay.direction,
                isCrit = false,
                minSpread = 0f,
                maxSpread = 0f,
                origin = aimRay.origin,
                maxDistance = 2000f,
                muzzleName = PlaceAirstrike.muzzleName,
                radius = 0.2f,
                hitCallback = AirstrikeHitCallback,
                stopperMask = LayerIndex.world.mask
            };
            ba.Fire();
        }

        protected bool AirstrikeHitCallback(BulletAttack bulletRef, ref BulletHit hitInfo)
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
