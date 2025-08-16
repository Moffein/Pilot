﻿using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;
using System;
using UnityEngine.AddressableAssets;
using MoffeinPilot.Modules;
using MoffeinPilot.Content.Components.SkyboxDrone;
using EntityStates.MoffeinPilot.Parachute;

namespace MoffeinPilot.Modules
{
    internal static class Asset
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;
        public static GameObject EngiMissileIndicatorButRed;
        public static GameObject TempParachute;
        public static GameObject ExplosionGolemButScale;
        public static GameObject SkyboxDronePrefab;

        // CHANGE THIS
        private const string assetbundleName = "pilotbundle";
        //change this to your project's name if/when you've renamed it
        private const string csProjName = "Pilot";
        
        internal static void Initialize()
        {
            //todo fix
            /*if (assetbundleName == "myassetbundle")
            {
                Log.Error("AssetBundle name hasn't been changed. not loading any assets to avoid conflicts");
                return;
            }*/

            LoadAssetBundle();
            PopulateAssets();
        }

        internal static void LoadAssetBundle()
        {
            try
            {
                if (mainAssetBundle == null)
                {
                    mainAssetBundle = AssetBundle.LoadFromFile(Files.GetPathToFile("AssetBundles", "pilotbundle"));

                    //Async causes this to not work properly
                    ShaderSwapper.ShaderSwapper.UpgradeStubbedShaders(mainAssetBundle);
                }
            }
            catch (Exception e)
            {
                Log.Error("Failed to load assetbundle. Make sure your assetbundle name is setup correctly\n" + e);
                return;
            }
        }

        internal static void PopulateAssets()
        {
            if (!mainAssetBundle)
            {
                Log.Error("There is no AssetBundle to load assets from.");
                return;
            }

            EngiMissileIndicatorButRed = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMissileTrackingIndicator.prefab").WaitForCompletion(), "PilotMissileTrackingIndicator", false);
            EngiMissileIndicatorButRed.transform.Find("Base Container/Base Core").GetComponent<SpriteRenderer>().color = new Color32(174, 26, 64, 255);

            ExplosionGolemButScale = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion(), "PilotComboExplosion", false);
            ExplosionGolemButScale.GetComponent<EffectComponent>().applyScale = true;

            ShakeEmitter[] shakeEmitters = ExplosionGolemButScale.GetComponentsInChildren<ShakeEmitter>();
            for (int i = 0; i < shakeEmitters.Length; i++)
            {
                UnityEngine.Object.Destroy(shakeEmitters[i]);
            }

            AddNewEffectDef(ExplosionGolemButScale);
            EntityStates.MoffeinPilot.Weapon.ClusterFire.comboExplosionEffectPrefab = ExplosionGolemButScale;

            TempParachute = mainAssetBundle.LoadAsset<GameObject>("PilotParachuteTempParachute");
            foreach(var r in TempParachute.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                r.sharedMaterial.SetHopooMaterial();
            }

            GameObject redLaserTracer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/CaptainDefenseMatrix/TracerCaptainDefenseMatrix.prefab").WaitForCompletion().InstantiateClone("PilotComboTracer", false);
            EffectComponent ec = redLaserTracer.GetComponent<EffectComponent>();
            ec.soundName = "";
            AddNewEffectDef(redLaserTracer);
            EntityStates.MoffeinPilot.Weapon.ClusterFire.comboTracerEffectPrefab = redLaserTracer;

            CreateSkyboxDronePrefab();

            DeployParachute.matParachuteAlpha = mainAssetBundle.LoadAsset<Material>("matParachuteStandardAlpha");
        }

        private static void CreateSkyboxDronePrefab()
        {
            SkyboxDronePrefab = mainAssetBundle.LoadAsset<GameObject>("PilotDronePrefab");
            SkyboxDronePrefab.AddComponent<SkyboxDroneController>();
            RoR2.CharacterBody.onBodyStartGlobal += SkyboxDroneController.CharacterBody_onBodyStartGlobal;
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            Modules.Content.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                i?.sharedMaterial?.SetHopooMaterial();
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }


        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- check to see if the BodyName in your code matches the prefab name of the object in Unity\nFor Example, if your prefab in unity is 'mdlHenry', then your BodyName must be 'Henry'");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            if (!newEffect)
            {
                Log.Error("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        private static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        private static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            Modules.Content.AddEffectDef(newEffectDef);
        }
    }
}