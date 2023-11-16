using Pilot.Content.Components.Projectile;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Pilot.Modules
{
    internal static class Projectiles
    {

        internal static void RegisterProjectiles()
        {
            NetworkSoundEventDef detSound = Assets.CreateNetworkSoundEventDef("Play_engi_M2_arm");
            GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("PilotAirstrikeBlastEffect", false);
            EffectComponent ec = blastEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_captain_shift_impact";
            Content.AddEffectDef(new EffectDef(blastEffectPrefab));

            GameObject ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierPreBombGhost.prefab").WaitForCompletion();
            EntityStates.Pilot.Airstrike.PlaceAirstrike.projectilePrefab = CreatePilotAirstrike("PilotAirstrikeProjectile", ghostPrefab, blastEffectPrefab, detSound, 7, 1.5f);
            EntityStates.Pilot.Airstrike.PlaceAirstrikeScepter.scepterProjectilePrefab = CreatePilotAirstrike("PilotAirstrikeScepterProjectile", ghostPrefab, blastEffectPrefab, detSound, 10, 1f);
        }

        private static GameObject CreatePilotAirstrike(string projectileName, GameObject ghostPrefab, GameObject blastEffectPrefab, NetworkSoundEventDef armSound, int maxTriggers, float rearmTime)
        {
            GameObject proj = Assets.pilotAssetBundle.LoadAsset<GameObject>("EmptyGameObject").InstantiateClone(projectileName, false); //Load from AssetBundle so it stays in memory. Is there a better way to do this?
            proj.AddComponent<NetworkIdentity>();

            ProjectileController pc = proj.AddComponent<ProjectileController>();
            pc.ghostPrefab = ghostPrefab;
            pc.allowPrediction = false;
            pc.procCoefficient = 1f;

            proj.AddComponent<ProjectileNetworkTransform>();
            proj.AddComponent<ProjectileDamage>();
            proj.AddComponent<TeamFilter>();

            DamageAPI.ModdedDamageTypeHolderComponent mdc = proj.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypes.AirstrikeKnockup);

            AirStrikeDamageComponent asdc = proj.AddComponent<AirStrikeDamageComponent>();
            asdc.armSound = armSound;
            asdc.blastEffectPrefab = blastEffectPrefab;
            asdc.maxTriggers = maxTriggers;
            asdc.rearmDuration = rearmTime;

            AddProjectile(proj);

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