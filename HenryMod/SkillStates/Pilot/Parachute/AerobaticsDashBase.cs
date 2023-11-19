using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Parachute
{
    public class AerobaticsDashBase : BaseState
    {
        public static float baseDuration = 0.3f;    //Phase Blink is 0.1
        public static GameObject blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();
        public static string blinkSoundString = "Play_huntress_shift_mini_blink";

        public Vector3 blinkVector;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;
        private bool startedAirborne;
        public static float minWavedashSpeedMult = 0.375f;
        private float wavedashSpeedMult;
        private EntityStateMachine parachuteMachine;

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

                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.6f + AerobaticsDashBase.baseDuration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay2.duration = 0.7f + AerobaticsDashBase.baseDuration;
                temporaryOverlay2.animateShaderAlpha = true;
                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay2.destroyComponentOnEnd = true;
                temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }

            parachuteMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Parachute");
            wavedashSpeedMult = 1f;
            startedAirborne = false;
            if (base.characterMotor) startedAirborne = !base.characterMotor.isGrounded;
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
                if (startedAirborne && base.characterMotor && base.characterMotor.isGrounded)
                {
                    if (base.inputBank && base.inputBank.jump.justPressed && parachuteMachine)
                    {
                        this.outer.SetNextState(new EntityStates.Pilot.Parachute.Wavedash()
                        {
                            initialSpeed = this.moveSpeedStat * GetBlinkSpeed() * Mathf.Max(wavedashSpeedMult, AerobaticsDashEntry.minWavedashSpeedMult),
                            entryDirection = blinkVector
                        });
                        return;
                    }
                    wavedashSpeedMult -= Time.fixedDeltaTime / AerobaticsDashBase.baseDuration;
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
                base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * GetBlinkSpeed() * Time.fixedDeltaTime);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!this.outer.destroying)
            {
                this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
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
