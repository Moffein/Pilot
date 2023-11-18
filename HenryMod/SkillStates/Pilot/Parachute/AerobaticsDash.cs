using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Parachute
{
    public class AerobaticsDash : BaseState
    {
        public static float baseDuration = 0.3f;    //Phase Blink is 0.1
        public static GameObject blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();
        public static string blinkSoundString = "Play_huntress_shift_mini_blink";

        public static float minWavedashSpeedMult = 0.375f;

        public Vector3 blinkVector;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;
        private float wavedashSpeedMult;
        private bool startedAirborne;

        public override void OnEnter()
        {
            base.OnEnter();

            wavedashSpeedMult = 1f;
            Util.PlaySound(AerobaticsDash.blinkSoundString, base.gameObject);
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));

            SetBlinkVector();

            startedAirborne = false;
            if (base.characterMotor) startedAirborne = !base.characterMotor.isGrounded;

            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();

                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.6f + AerobaticsDash.baseDuration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay2.duration = 0.7f + AerobaticsDash.baseDuration;
                temporaryOverlay2.animateShaderAlpha = true;
                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay2.destroyComponentOnEnd = true;
                temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }
            /*if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }*/
        }

        public virtual void SetBlinkVector()
        {
            blinkVector = base.GetAimRay().direction;
        }

        public virtual float GetBlinkSpeed()
        {
            return 12f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            DashPhysics();

            if (base.isAuthority)
            {
                if (startedAirborne && base.characterMotor && base.characterMotor.isGrounded)
                {
                    if (base.inputBank && base.inputBank.jump.justPressed)
                    {
                        EntityStateMachine parachuteMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Parachute");
                        if (parachuteMachine)
                        {
                            this.outer.SetNextState(new EntityStates.Pilot.Parachute.Wavedash()
                            {
                                initialSpeed = this.moveSpeedStat * GetBlinkSpeed() * Mathf.Max(wavedashSpeedMult, AerobaticsDash.minWavedashSpeedMult),
                                entryDirection = blinkVector
                            });
                            return;
                        }
                    }
                    wavedashSpeedMult -= Time.fixedDeltaTime / AerobaticsDash.baseDuration;
                }

                if (base.fixedAge >= AerobaticsDash.baseDuration)
                {
                    CheckWallBounceExit();
                    return;
                }
            }
        }

        //Not the cleanest way to do this
        private void CheckWallBounceExit()
        {
            if (base.characterBody && Physics.OverlapSphere(base.characterBody.corePosition, 1.1f * base.characterBody.radius, LayerIndex.world.mask.value).Length > 0)
            {
                this.outer.SetNextStateToMain();
                //this.outer.SetNextState(new WallBounce);
                return;
            }

            this.outer.SetNextStateToMain();
            return;
        }

        public virtual void DashPhysics()
        {
            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * GetBlinkSpeed() * Time.fixedDeltaTime);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!this.outer.destroying)
            {
                this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                /*this.modelTransform = base.GetModelTransform();
                if (this.modelTransform)
                {
                    TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 0.6f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                    TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 0.7f;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                    temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                }*/
            }
            /*if (this.characterModel)
            {
                this.characterModel.invisibilityCount--;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }*/
        }

        public virtual void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
            effectData.origin = origin;
            EffectManager.SpawnEffect(AerobaticsDash.blinkPrefab, effectData, false);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
