using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPilot.Airstrike
{
    public class DashGround : BaseState
    {
        public static float baseDuration = 0.2f;    //Phase Blink is 0.1
        public static GameObject blinkPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBlinkEffect.prefab").WaitForCompletion();

        public Vector3 blinkVector;

        private Transform modelTransform;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;

        public override void OnEnter()
        {
            base.OnEnter();

            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));

            SetBlinkVector();

            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();

                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.6f + DashGround.baseDuration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay2.duration = 0.7f + DashGround.baseDuration;
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
            blinkVector = base.characterDirection ? base.characterDirection.forward.normalized : Vector3.zero;
            if (base.inputBank && base.inputBank.moveVector != Vector3.zero) blinkVector = base.inputBank.moveVector.normalized;
        }

        public virtual float GetBlinkSpeed()
        {
            return 10.875f;  //Same as phase blink, scaled to duration. Includes sprint multiplier baked-in since this skill disables sprint.
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            DashPhysics();

            if (base.isAuthority && base.fixedAge >= DashGround.baseDuration)
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
            EffectManager.SpawnEffect(DashGround.blinkPrefab, effectData, false);
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
