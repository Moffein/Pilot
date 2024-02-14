using MoffeinPilot.Modules;
using R2API;
using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace MoffeinPilot.Content.Components.Projectile
{
    public class AirStrikeDamageComponent : MonoBehaviour
    {
        public int maxTriggers = 7;

        //public float lifetime = 120f;
        public float rearmDuration = 1.5f;
        public float detonationDelay = 0.5f;
        public float initialArmDuration = 1f;
        public float blastRadius = 12f;
        public float triggerRadius = 8f;
        public float enemyCheckFrequency = 10f;

        public NetworkSoundEventDef armSound;
        public GameObject blastEffectPrefab;
        public GameObject impactEffectPrefab;

        private int triggerCount;
        private float detectionTimer;
        private float detonationTimer;
        private ProjectileDamage projectileDamage;
        private TeamFilter teamFilter;
        private ProjectileController projectileController;

        private void Awake()
        {
            triggerCount = 0;
            detonationTimer = 0f;
            detectionTimer = initialArmDuration;
            projectileDamage = base.GetComponent<ProjectileDamage>();
            teamFilter = base.GetComponent<TeamFilter>();
            projectileController = base.GetComponent<ProjectileController>();

            if (!projectileDamage || !projectileController) Destroy(this);
        }

        private void Start()
        {
            if (projectileController.owner)
            {
                PilotController pc = projectileController.owner.GetComponent<PilotController>();
                if (pc)
                {
                    pc.RegisterAirstrike(base.gameObject);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            if (detonationTimer > 0f)
            {
                detonationTimer -= Time.fixedDeltaTime;
                if (detonationTimer <= 0f) Explode();
            }
            else
            {
                detectionTimer -= Time.fixedDeltaTime;
                if (detectionTimer <= 0)
                {
                    if (IsEnemyInSphere(triggerRadius))
                    {
                        detonationTimer = detonationDelay;
                        detectionTimer = rearmDuration;

                        if (armSound) EffectManager.SimpleSoundEffect(armSound.index, base.transform.position, true);
                    }
                    else
                    {
                        detectionTimer = 1f / enemyCheckFrequency;
                    }
                }
            }
        }

        private bool IsEnemyInSphere(float radius)
        {
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(base.transform.position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent && !hcList.Contains(healthComponent))
                    {
                        hcList.Add(healthComponent);
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != teamFilter.teamIndex) return true;
                    }
                }
            }
            return false;
        }

        public void Explode()
        {
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
            ba.AddModdedDamageType(DamageTypes.AirstrikeKnockup);
            ba.Fire();

            triggerCount++;
            if (triggerCount > maxTriggers)
            {
                Destroy(base.gameObject);
            }
        }
    }
}
