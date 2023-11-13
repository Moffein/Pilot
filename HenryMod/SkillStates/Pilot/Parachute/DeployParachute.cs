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
        public static float minDuration = 0.1f;
        public static float baseDuration = 0.5f;
        public static float boostVelocity = 15f;
        public static float hopVelocity = 40f;
        public static float stunRadius = 12f;
        public static string deploySoundString = "Play_bandit2_shift_exit";
        public static GameObject stunEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2SmokeBomb.prefab").WaitForCompletion();

        private PilotController pilotController;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound(DeployParachute.deploySoundString, base.gameObject);
            if (NetworkServer.active && stunRadius > 0f) StunEnemies(base.transform.position);

            if (base.isAuthority && base.characterMotor)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
                base.SmallHop(base.characterMotor, DeployParachute.hopVelocity);

                Ray aimRay = base.GetAimRay();

                //Speed scaling.
                float velocityMult = base.moveSpeedStat / (base.characterBody ? base.characterBody.baseMoveSpeed : 7f);

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

            if (base.isAuthority)
            {
                bool isFalling = base.fixedAge >= DeployParachute.minDuration && base.characterMotor && base.characterMotor.velocity.y <= 0f;
                if (isFalling || base.fixedAge >= DeployParachute.baseDuration)
                {
                    this.outer.SetNextState(new Glide());
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

                List<HealthComponent> hcList = new List<HealthComponent>();
                Collider[] array = Physics.OverlapSphere(stunPosition, DeployParachute.stunRadius, LayerIndex.entityPrecise.mask);
                for (int i = 0; i < array.Length; i++)
                {
                    HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                    if (hurtBox && hurtBox.healthComponent && !hcList.Contains(hurtBox.healthComponent))
                    {
                        hcList.Add(hurtBox.healthComponent);
                        if (hurtBox.healthComponent.body.teamComponent && hurtBox.healthComponent.body.teamComponent.teamIndex != base.GetTeam())
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
