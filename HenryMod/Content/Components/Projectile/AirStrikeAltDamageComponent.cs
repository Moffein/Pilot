using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Pilot.Content.Components.Projectile
{
    public class AirStrikeAltDamageComponent : MonoBehaviour
    {
        public int explosionCount = 3;
        public GameObject blastEffectPrefab;
        public GameObject impactEffectPrefab;
        public float blastRadius = 12f;

        public float initialDelay = 1.5f;
        public float delayBetweenExplosions = 0.3f;

        private float stopwatch;
        private ProjectileDamage projectileDamage;
        private TeamFilter teamFilter;
        private ProjectileController projectileController;
        private DamageAPI.ModdedDamageTypeHolderComponent moddedDamageType;

        private void Awake()
        {
            stopwatch = initialDelay;
            projectileDamage = base.GetComponent<ProjectileDamage>();
            teamFilter = base.GetComponent<TeamFilter>();
            projectileController = base.GetComponent<ProjectileController>();
            moddedDamageType = base.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;
            stopwatch -= Time.fixedDeltaTime;
            if (stopwatch <= 0) Explode();
        }

        public void Explode()
        {

            explosionCount--;
            stopwatch = delayBetweenExplosions;

            if (blastEffectPrefab) EffectManager.SpawnEffect(blastEffectPrefab, new EffectData { origin = base.transform.position, scale = blastRadius }, true);
            BlastAttack ba = new BlastAttack()
            {
                attacker = projectileController ? projectileController.owner : null,
                attackerFiltering = AttackerFiltering.Default,
                baseDamage = projectileDamage.damage,
                baseForce = 0f,
                bonusForce = Vector3.zero,
                canRejectForce = true,
                crit = projectileDamage.crit,
                damageColorIndex = DamageColorIndex.Default,
                damageType = projectileDamage.damageType,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = base.gameObject,
                position = base.transform.position,
                procChainMask = default,
                procCoefficient = 1f,
                radius = blastRadius,
                teamIndex = teamFilter ? teamFilter.teamIndex : TeamIndex.None
            };

            if (explosionCount <= 0) moddedDamageType.CopyTo(ba);

            ba.Fire();

            if (explosionCount <= 0)
            {
                Destroy(base.gameObject);
                return;
            }
        }
    }
}
