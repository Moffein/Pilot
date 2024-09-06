using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class AerobaticsDashBase : BaseState
    {
        public static float baseDuration = 0.3f;
        public static GameObject blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();
        public static string blinkSoundString = "Play_huntress_shift_mini_blink";

        public Vector3 blinkVector;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;
        private bool startedAirborne;
        public static float minWavedashSpeedMult = 0.5f;
        public static float maxWavedashSpeedMult = 0.5f;
        private float wavedashSpeedMult;
        private EntityStateMachine parachuteMachine;

        private int origJumpCount;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(AerobaticsDashBase.blinkSoundString, base.gameObject);
            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));

            SetBlinkVector();


            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();

                CharacterModel cm = this.modelTransform.GetComponent<CharacterModel>();
                if (cm)
                {
                    TemporaryOverlayInstance temporaryOverlay = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                    temporaryOverlay.duration = 0.6f + AerobaticsDashBase.baseDuration;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacterModel(cm);
                    temporaryOverlay.Start();


                    TemporaryOverlayInstance temporaryOverlay2 = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                    temporaryOverlay2.duration = 0.7f + AerobaticsDashBase.baseDuration;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                    temporaryOverlay2.AddToCharacterModel(cm);
                    temporaryOverlay2.Start();
                }
            }

            parachuteMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Parachute");
            wavedashSpeedMult = AerobaticsDashBase.maxWavedashSpeedMult;
            startedAirborne = false;
            if (base.characterMotor)
            {
                startedAirborne = !base.characterMotor.isGrounded;

                origJumpCount = base.characterMotor.jumpCount;
                base.characterMotor.jumpCount = base.characterBody ? base.characterBody.maxJumpCount : 1;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
        }

        public virtual void SetBlinkVector()
        {
            blinkVector = base.GetAimRay().direction;
        }

        public virtual float GetBlinkSpeed()
        {
            return 14f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            DashPhysics();

            if (base.isAuthority)
            {
                if (base.characterMotor && base.characterMotor.isGrounded)//startedAirborne && 
                {
                    if (base.inputBank && base.inputBank.jump.down && parachuteMachine)//base.inputBank.jump.justPressed
                    {
                        this.outer.SetNextState(new EntityStates.MoffeinPilot.Parachute.Wavedash()
                        {
                            initialSpeed = this.moveSpeedStat * GetBlinkSpeed() * Mathf.Max(wavedashSpeedMult, AerobaticsDashEntry.minWavedashSpeedMult),
                            entryDirection = blinkVector
                        });
                        return;
                    }
                    wavedashSpeedMult -= Time.deltaTime / AerobaticsDashBase.baseDuration;
                }
            }

            CheckStateExit();
        }

        protected virtual void CheckStateExit()
        {
            if (!base.isAuthority) return;
            if (base.fixedAge >= AerobaticsDashBase.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public virtual void DashPhysics()
        {
            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * GetBlinkSpeed() * Time.deltaTime);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (base.characterMotor)
            {
                if (base.characterMotor.isGrounded)
                    base.characterMotor.jumpCount = 0;
                else
                    base.characterMotor.jumpCount = Mathf.Max(origJumpCount, 1);
            }
            if (!this.outer.destroying)
            {
                this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
        }

        public virtual void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
            effectData.origin = origin;
            EffectManager.SpawnEffect(AerobaticsDashBase.blinkPrefab, effectData, false);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
