using BepInEx.Configuration;
using MoffeinPilot.Content.Components;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using MoffeinPilot.Modules;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class DeployParachute : BaseState
    {
        public static float liftDuration = 0f;
        public static float baseDuration = 1.2f;    //This just acts as a hard-cap to the duration in case anything weird happens, so you don't get softlocked.
        public static float liftVelocity = 54f;     //This linearly lowers to 0 over the course of the skill.

        public static float boostVelocity = 18f;
        public static float stopAscentVelocity = 12f;

        public static float airBoostMult = 1.4f;
        public static float airLiftMult = 0.6f;

        public static float minDuration = 0.3f;
        public static ConfigEntry<bool> holdToAscend;

        public static float stunRadius = 12f;
        public static string deploySoundString = "Play_bandit2_shift_exit";
        public static GameObject stunEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();

        private PilotController pilotController;
        private bool stopAscent;
        private bool startedAirborne;

        private GameObject parachute;
        private Renderer[] parachuteRenderers;
        private bool uninterrupted;

        private int origJumpCount;


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

            stopAscent = false;

            Util.PlaySound(DeployParachute.deploySoundString, base.gameObject);
            if (NetworkServer.active && stunRadius > 0f) StunEnemies(base.transform.position);

            startedAirborne = false;

            CreateParachute();

            StartAimMode(1);
            if (base.characterMotor)
            {
                startedAirborne = !base.characterMotor.isGrounded;

                origJumpCount = base.characterMotor.jumpCount;
                base.characterMotor.jumpCount = base.characterBody ? base.characterBody.maxJumpCount : 1;

                if (startedAirborne) {
                    base.PlayAnimation("FullBody, Override", "ParachuteForward");
                } else {
                    base.PlayAnimation("FullBody, Override", "ParachuteUp");
                }

                if (base.isAuthority)
                {
                    base.characterMotor.disableAirControlUntilCollision = false;
                    //base.SmallHop(base.characterMotor, DeployParachute.hopVelocity);

                    Ray aimRay = base.GetAimRay();

                    //Speed scaling.
                    float velocityMult = base.moveSpeedStat / (base.characterBody && base.characterBody.baseMoveSpeed != 0f ? base.characterBody.baseMoveSpeed : 7f);
                    if (startedAirborne) velocityMult *= DeployParachute.airBoostMult;

                    Vector3 directionFlat = aimRay.direction;
                    directionFlat.y = 0f;
                    directionFlat.Normalize();

                    base.characterMotor.velocity += directionFlat * boostVelocity * velocityMult;
                }
            }

            pilotController = base.GetComponent<PilotController>();
            if (pilotController)
            {
                pilotController.isParachuting = true;
            }


            if (cameraTargetParams)
            {
                cameraTargetParams.RemoveParamsOverride(camOverrideHandle);
                CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = cameraParams,
                    priority = 0f
                };
                camOverrideHandle = cameraTargetParams.AddParamsOverride(request, 0.5f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            HandleMotion();
            base.StartAimMode();

            if (base.isAuthority)
            {
                if (base.healthComponent && base.healthComponent.isInFrozenState)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }

                //bool isFalling = base.fixedAge >= DeployParachute.minDuration && base.characterMotor && base.characterMotor.velocity.y <= 0f;
                if (base.fixedAge >= DeployParachute.baseDuration || stopAscent)
                {
                    uninterrupted = true;
                    this.outer.SetNextState(new Glide());
                    return;
                }
            }
        }

        private void HandleMotion()
        {

            if (!stopAscent && base.characterMotor)
            {
                if (base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y = 0;

                float velocity = DeployParachute.liftVelocity;
                if (startedAirborne) velocity *= DeployParachute.airLiftMult;

                if (base.fixedAge > DeployParachute.liftDuration)
                {
                    velocity *= Mathf.Lerp(DeployParachute.baseDuration, DeployParachute.liftDuration, base.fixedAge);
                }

                if (velocity > DeployParachute.stopAscentVelocity)
                {
                    base.characterMotor.rootMotion.y += (velocity * Time.fixedDeltaTime);
                }
                else
                {
                    //This smooths out the ending of the ascent by converting the pure linear rootmotion deceleration into the game's own force handling.
                    //Could probably skipped if a VelocityCurve was set up.
                    stopAscent = true;
                    base.SmallHop(base.characterMotor, DeployParachute.stopAscentVelocity);
                    return;
                }

                if (base.isAuthority && DeployParachute.holdToAscend.Value && base.fixedAge > DeployParachute.minDuration && base.inputBank && !base.inputBank.skill3.down)
                {
                    stopAscent = true;
                    base.SmallHop(base.characterMotor, DeployParachute.stopAscentVelocity);
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
            if (base.characterMotor && !base.characterMotor.isGrounded) base.characterMotor.jumpCount = 1;

            DestroyParachute();
            if (cameraTargetParams) cameraTargetParams.RemoveParamsOverride(camOverrideHandle, 0.5f);
            base.OnExit();
        }

        private void StunEnemies(Vector3 stunPosition)
        {
            if (base.characterBody)
            {
                if (base.characterBody.coreTransform)
                {
                    EffectManager.SimpleEffect(DeployParachute.stunEffectPrefab, stunPosition, characterBody.coreTransform.rotation, true);
                }

                TeamIndex myTeam = base.GetTeam();

                List<HealthComponent> hcList = new List<HealthComponent>();
                Collider[] array = Physics.OverlapSphere(stunPosition, DeployParachute.stunRadius, LayerIndex.entityPrecise.mask);
                for (int i = 0; i < array.Length; i++)
                {
                    HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                    if (hurtBox && hurtBox.healthComponent && !hcList.Contains(hurtBox.healthComponent))
                    {
                        hcList.Add(hurtBox.healthComponent);
                        if (hurtBox.healthComponent.body.teamComponent && hurtBox.healthComponent.body.teamComponent.teamIndex != myTeam)
                        {
                            SetStateOnHurt ssoh = hurtBox.healthComponent.gameObject.GetComponent<SetStateOnHurt>();
                            if (ssoh && ssoh.canBeStunned)
                            {
                                ssoh.SetStun(1f);
                            }
                        }
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }


        //Duplicated code.
        //Fade parachute if looking downwards.
        public static ConfigEntry<bool> enableParachuteFade;
        public static float fadeAmount = 0.6f;
        public static float fadeLookAngle = -0.35f;
        private MaterialPropertyBlock propertyStorage;
        /*public override void Update()
        {
            base.Update();

            if (!base.isAuthority || !DeployParachute.enableParachuteFade.Value || !parachute || parachuteRenderers == null || parachuteRenderers.Length <= 0) return;
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

            if (parachuteRenderers == null || !base.isAuthority || !DeployParachute.enableParachuteFade.Value || !(base.characterBody && base.characterBody.isPlayerControlled)) return;
            for (int i = 0; i < parachuteRenderers.Length; i++)
            {
                parachuteRenderers[i].GetPropertyBlock(propertyStorage);
                propertyStorage.SetFloat("_Fade", DeployParachute.fadeAmount);
                parachuteRenderers[i].SetPropertyBlock(propertyStorage);
            }
        }

        private void CreateParachute()
        {
            parachute = Object.Instantiate(Assets.TempParachute, FindModelChild("ParachutePosition"), false);
            parachute.transform.localPosition = Vector3.zero;
            parachute.transform.localScale = Vector3.one;
            parachute.transform.localRotation = Quaternion.identity;

            SetupParachuteFade();
        }

        private void DestroyParachute()
        {
            if (parachute) Destroy(parachute);
        }
    }
}
