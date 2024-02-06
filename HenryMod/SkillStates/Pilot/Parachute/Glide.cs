using BepInEx.Configuration;
using MoffeinPilot.Content.Components;
using RoR2;
using UnityEngine;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class Glide : BaseState
    {
        public static float maxFallVelocity = -6f;
        public static float exitHopVelocity = 17f;
        public static GameObject jumpEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact");

        private PilotController pilotController;
        private int origJumpCount;
        private bool jumpReleased = false;
        internal GameObject parachute;
        private Renderer[] parachuteRenderers;

        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;

        private CharacterCameraParamsData cameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 3.5f, //how far up should the camera go?
            idealLocalCameraPos = zoomCameraPosition,
            wallCushion = 0.1f
        };
        private static Vector3 zoomCameraPosition = new Vector3(0f, 0f, -10f); // how far back should the camera go?

        public override void OnEnter()
        {
            base.OnEnter();

            if (parachute)
            {
                DestroyOnTimer dt = parachute.GetComponent<DestroyOnTimer>();
                if (dt) Destroy(dt);
            }

            SetupParachuteFade();
            pilotController = base.GetComponent<PilotController>();
            if (pilotController)
            {
                pilotController.isParachuting = true;
            }
            if (base.characterMotor)
            {
                origJumpCount = base.characterMotor.jumpCount;
                base.characterMotor.jumpCount = base.characterBody ? base.characterBody.maxJumpCount : 1;
            }

            if (cameraTargetParams)
            {
                cameraTargetParams.RemoveParamsOverride(camOverrideHandle);
                CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams,
                    priority = 0f
                };
                camOverrideHandle = cameraTargetParams.AddParamsOverride(request, 0f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode();
            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    if (base.characterMotor.velocity.y <= Glide.maxFallVelocity) base.characterMotor.velocity.y = Glide.maxFallVelocity;
                }

                bool isGrounded = base.characterMotor && base.characterMotor.isGrounded;
                bool jumped = false;
                if (base.inputBank)
                {
                    if (!jumpReleased) jumpReleased = !base.inputBank.jump.down;
                    jumped = jumpReleased && base.inputBank.jump.down;
                }
                

                if (isGrounded || jumped)
                {
                    if (jumped)
                    {
                        base.SmallHop(base.characterMotor, Glide.exitHopVelocity);
                        if (base.characterBody) EffectManager.SpawnEffect(jumpEffect, new EffectData
                        {
                            origin = base.characterBody.footPosition,
                            scale = base.characterBody.radius
                        }, true);

                        //TODO: play jump anim
                    }
                    this.outer.SetNextStateToMain();
                    return;
                }

            }
        }
        
        public override void OnExit()
        {
            if (base.characterMotor)
            {
                if (base.characterMotor.isGrounded)
                    base.characterMotor.jumpCount = 0;
                else
                    base.characterMotor.jumpCount = Mathf.Max(origJumpCount, 1);
            }
            if (pilotController)
            {
                pilotController.isParachuting = false;
            }

            if (cameraTargetParams) cameraTargetParams.RemoveParamsOverride(camOverrideHandle, 0.5f);
            Destroy(parachute);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        //Duplicated code.
        //Fade parachute if looking downwards.
        private MaterialPropertyBlock propertyStorage;
        /*public override void Update()
        {
            base.Update();

            if (!base.isAuthority || !DeployParachute.enableParachuteFade.Value || !parachute || parachuteRenderers == null|| parachuteRenderers.Length <= 0) return;
            Ray aimRay = base.GetAimRay();

            //float fadeLerp = Mathf.Lerp(1f, fadeAmount, aimRay.direction.y / (-1f - fadeLookAngle));
            float fadeLerp = aimRay.direction.y < DeployParachute.fadeLookAngle ? DeployParachute.fadeAmount : 1f;
            for (int i = 0; i < parachuteRenderers.Length; i++)
            {
                parachuteRenderers[i].GetPropertyBlock(propertyStorage);
                propertyStorage.SetFloat("_Fade", fadeLerp);
                parachuteRenderers[i].SetPropertyBlock(propertyStorage);
            }
        }*/

        private void SetupParachuteFade()
        {
            propertyStorage = new MaterialPropertyBlock();
            if (parachute) parachuteRenderers = parachute.GetComponentsInChildren<SkinnedMeshRenderer>();   //Just include SkinnedMeshRenderer for now to leave out the linerenderers.

            if (parachuteRenderers == null || !base.isAuthority || !DeployParachute.enableParachuteFade.Value) return;
            for (int i = 0; i < parachuteRenderers.Length; i++)
            {
                parachuteRenderers[i].GetPropertyBlock(propertyStorage);
                propertyStorage.SetFloat("_Fade", DeployParachute.fadeAmount);
                parachuteRenderers[i].SetPropertyBlock(propertyStorage);
            }
        }
    }
}