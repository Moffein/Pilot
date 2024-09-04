using BepInEx.Configuration;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class DashGround : BaseState
    {
        public static ConfigEntry<bool> onlyDashBackwards;
        public static float baseDuration = 0.2f;    //Phase Blink is 0.1
        public static GameObject blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();

        public Vector3 blinkVector;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;

        public override void OnEnter()
        {
            base.OnEnter();

            CreateBlinkEffect(Util.GetCorePosition(gameObject));

            SetBlinkVector();
            StartAimMode(new Ray(transform.position, -blinkVector), 0.5f);
            PlayAnimation("Gesture, Override", "PointGround", "Point.playbackRate", 0.5f);

            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();

                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.6f + baseDuration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                TemporaryOverlay temporaryOverlay2 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay2.duration = 0.7f + baseDuration;
                temporaryOverlay2.animateShaderAlpha = true;
                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay2.destroyComponentOnEnd = true;
                temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                temporaryOverlay2.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }
            /*if (this.characterModel)
            {
                this.characterModel.invisibilityCount++;
            }*/
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
        }

        public virtual void SetBlinkVector()
        {
            if (!onlyDashBackwards.Value)
            {
                blinkVector = characterDirection ? characterDirection.forward.normalized : Vector3.zero;
                if (inputBank && inputBank.moveVector != Vector3.zero) blinkVector = inputBank.moveVector.normalized;
            }
            else
            {
                blinkVector = characterDirection ? -characterDirection.forward.normalized : Vector3.zero;
            }
        }

        public virtual float GetBlinkSpeed()
        {
            return 10.875f;  //Same as phase blink, scaled to duration. Includes sprint multiplier baked-in since this skill disables sprint.
        }

        public override void Update() {
            base.Update();

            characterDirection.forward = -blinkVector.normalized;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            DashPhysics();

            if (isAuthority && fixedAge >= baseDuration)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public virtual void DashPhysics()
        {
            if (characterMotor && characterDirection)
            {
                characterMotor.velocity = Vector3.zero;
                characterMotor.rootMotion += blinkVector * (moveSpeedStat * GetBlinkSpeed() * Time.deltaTime);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!outer.destroying)
            {
                CreateBlinkEffect(Util.GetCorePosition(gameObject));
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
            }*/
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
            effectData.rotation = Util.QuaternionSafeLookRotation(blinkVector);
            effectData.origin = origin;
            EffectManager.SpawnEffect(blinkPrefab, effectData, false);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
