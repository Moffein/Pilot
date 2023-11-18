using Pilot.Content.Components;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.Pilot.Parachute
{
    public class DeployParachute : BaseState
    {
        public static float liftDuration = 0f;
        public static float baseDuration = 0.8f;
        public static float liftVelocity = 72f; //This linearly lowers to 0 over the course of the skill.
        //public static float hopVelocity = 36f;

        public static float boostVelocity = 18f;
        public static float stopAscentVelocity = 6f;


        public static float stunRadius = 12f;
        public static string deploySoundString = "Play_bandit2_shift_exit";
        public static GameObject stunEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();

        private PilotController pilotController;
        private bool stopAscent;

        public override void OnEnter()
        {
            base.OnEnter();

            stopAscent = false;

            Util.PlaySound(DeployParachute.deploySoundString, base.gameObject);
            if (NetworkServer.active && stunRadius > 0f) StunEnemies(base.transform.position);

            if (base.isAuthority && base.characterMotor)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
                //base.SmallHop(base.characterMotor, DeployParachute.hopVelocity);

                Ray aimRay = base.GetAimRay();

                //Speed scaling.
                float velocityMult = base.moveSpeedStat / (base.characterBody && base.characterBody.baseMoveSpeed != 0f ? base.characterBody.baseMoveSpeed : 7f);

                Vector3 directionFlat = aimRay.direction;
                directionFlat.y = 0f;
                directionFlat.Normalize();

                base.characterMotor.velocity += directionFlat * boostVelocity * velocityMult;
            }

            pilotController = base.GetComponent<PilotController>();
            if (pilotController)
            {
                pilotController.isParachuting = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!stopAscent&& base.characterMotor)
            {
                if (base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y = 0;

                float velocity = DeployParachute.liftVelocity;
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
                }
            }

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
                    this.outer.SetNextState(new Glide());
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (pilotController)
            {
                pilotController.isParachuting = false;
            }
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
    }
}
