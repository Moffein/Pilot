using MoffeinPilot.Content.Components.Projectile;
using Pilot.Content.Components.ProjectileGhost;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace MoffeinPilot.Modules
{
    internal static class Projectiles
    {

        internal static void RegisterProjectiles()
        {
            NetworkSoundEventDef detSound = Assets.CreateNetworkSoundEventDef("Play_engi_M2_arm");
            
            GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("PilotAirstrikeBlastEffect", false);
            {
                EffectComponent ec = blastEffectPrefab.GetComponent<EffectComponent>();
                ec.soundName = "Play_captain_shift_impact";
            }
            Content.AddEffectDef(new EffectDef(blastEffectPrefab));

            GameObject ghostPrefab = CreateAirStrikeGhost("AirStrikeVisualPrefab");
            GameObject ghostAltPrefab = CreateAirStrikeGhost("AirStrikeAltVisualPrefab");
            GameObject ghostScepterPrefab = CreateAirStrikeGhost("AirStrikeScepterVisualPrefab");
            GameObject ghostAltScepterPrefab = CreateAirStrikeGhost("AirStrikeAltScepterVisualPrefab");
            EntityStates.MoffeinPilot.Airstrike.PlaceAirstrike.projectilePrefab = CreatePilotAirstrike("PilotAirstrikeProjectile", ghostPrefab, blastEffectPrefab, detSound, 3, 1.5f);
            EntityStates.MoffeinPilot.Airstrike.PlaceAirstrikeScepter.projectilePrefab = CreatePilotAirstrike("PilotAirstrikeScepterProjectile", ghostScepterPrefab, blastEffectPrefab, detSound, 6, 1.5f);
            EntityStates.MoffeinPilot.Airstrike.PlaceAirstrikeAlt.projectilePrefab = CreatePilotAirstrikeAlt("PilotAirstrikeAltProjectile", ghostAltPrefab, blastEffectPrefab, detSound, 4, 0.25f);
            EntityStates.MoffeinPilot.Airstrike.PlaceAirstrikeAltScepter.projectilePrefab = CreatePilotAirstrikeAlt("PilotAirstrikeAltScepterProjectile", ghostScepterPrefab, blastEffectPrefab, detSound, 8, 0.15f);


            EntityStates.MoffeinPilot.Weapon.FireColdWar.projectilePrefab = CreatePilotColdWarProjectile("PilotColdWarProjectile", Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/LightningStakeNova.prefab").WaitForCompletion());
        }

        private static GameObject CreateAirStrikeGhost(string prefabName)
        {
            GameObject toReturn = Assets.mainAssetBundle.LoadAsset<GameObject>(prefabName).InstantiateClone(prefabName + "Ghost", false);

            toReturn.AddComponent<ProjectileGhostController>();
            toReturn.AddComponent<RotationVisuals>();

            VFXAttributes vfx = toReturn.AddComponent<VFXAttributes>();
            vfx.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfx.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            ChildLocator cl = toReturn.GetComponent<ChildLocator>();
            if (cl)
            {
                Transform sphere = cl.FindChild("Sphere");
                if (sphere)
                {
                    MeshRenderer m = sphere.GetComponent<MeshRenderer>();
                    if (m)
                    {
                        m.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/NearbyDamageBonus/matNearbyDamageBonusRangeIndicator.mat").WaitForCompletion();
                    }
                }
            }

            return toReturn;
        }

        private static GameObject CreatePilotAirstrike(string projectileName, GameObject ghostPrefab, GameObject blastEffectPrefab, NetworkSoundEventDef armSound, int maxTriggers, float rearmTime)
        {
            GameObject proj = Assets.mainAssetBundle.LoadAsset<GameObject>("EmptyGameObject").InstantiateClone(projectileName, false); //Load from AssetBundle so it stays in memory. Is there a better way to do this?
            proj.layer = LayerIndex.noCollision.intVal;
            proj.AddComponent<NetworkIdentity>();

            ProjectileController pc = proj.AddComponent<ProjectileController>();
            pc.ghostPrefab = ghostPrefab;
            pc.allowPrediction = false;
            pc.procCoefficient = 1f;

            proj.AddComponent<ProjectileNetworkTransform>();
            proj.AddComponent<ProjectileDamage>();
            proj.AddComponent<TeamFilter>();

            AirStrikeDamageComponent asdc = proj.AddComponent<AirStrikeDamageComponent>();
            asdc.armSound = armSound;
            asdc.blastEffectPrefab = blastEffectPrefab;
            asdc.maxTriggers = maxTriggers;
            asdc.rearmDuration = rearmTime;
            asdc.blastRadius = 10f;
            asdc.triggerRadius = 6f;
            asdc.initialArmDuration = rearmTime;

            proj.RegisterNetworkPrefab();
            AddProjectile(proj);

            return proj;
        }

        private static GameObject CreatePilotAirstrikeAlt(string projectileName, GameObject ghostPrefab, GameObject blastEffectPrefab, NetworkSoundEventDef armSound, int explosionCount, float rearmTime)
        {
            GameObject proj = Assets.mainAssetBundle.LoadAsset<GameObject>("EmptyGameObject").InstantiateClone(projectileName, false); //Load from AssetBundle so it stays in memory. Is there a better way to do this?
            proj.layer = LayerIndex.noCollision.intVal;
            proj.AddComponent<NetworkIdentity>();

            ProjectileController pc = proj.AddComponent<ProjectileController>();
            pc.ghostPrefab = ghostPrefab;
            pc.allowPrediction = false;
            pc.procCoefficient = 1f;

            proj.AddComponent<ProjectileNetworkTransform>();
            proj.AddComponent<ProjectileDamage>();
            proj.AddComponent<TeamFilter>();

            AirStrikeAltDamageComponent asdca = proj.AddComponent<AirStrikeAltDamageComponent>();
            asdca.blastEffectPrefab = blastEffectPrefab;
            asdca.delayBetweenExplosions = rearmTime;
            asdca.explosionCount = explosionCount;
            asdca.blastRadius = 10f;

            proj.RegisterNetworkPrefab();
            AddProjectile(proj);

            return proj;
        }

        private static GameObject CreatePilotColdWarProjectile(string projectileName, GameObject explosionEffect)
        {
            GameObject proj = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningboltBasic.prefab").WaitForCompletion().InstantiateClone(projectileName, true);

            Rigidbody rb = proj.GetComponent<Rigidbody>();
            rb.useGravity = true;

            ProjectileSimple ps = proj.GetComponent<ProjectileSimple>();
            ps.lifetime = 5f;
            ps.desiredForwardSpeed = 120f;

            ProjectileImpactExplosion pie = proj.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = 6f;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            if (explosionEffect) pie.explosionEffect = explosionEffect;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = proj.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            if (mdc) UnityEngine.Object.Destroy(mdc);
            mdc = proj.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.KeepAirborne);

            /*AntiGravityForce agf = proj.AddComponent<AntiGravityForce>();
            agf.antiGravityCoefficient = 0.5f;
            agf.rb = rb;*/

            ProjectileDamage pd = proj.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            Content.AddProjectilePrefab(proj);

            return proj;
        }

        internal static void AddProjectile(GameObject projectileToAdd)
        {
            Modules.Content.AddProjectilePrefab(projectileToAdd);
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName);
            if (!ghostPrefab.GetComponent<NetworkIdentity>()) ghostPrefab.AddComponent<NetworkIdentity>();
            if (!ghostPrefab.GetComponent<ProjectileGhostController>()) ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}