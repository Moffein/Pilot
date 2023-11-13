using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Pilot.Airstrike
{
    public class PlaceAirstrike : BaseState
    {
        public static float baseDuration = 0.3f;
        public static string attackSoundString = "";
        public static GameObject projectilePrefab;
        public static float smallHopVelocity = 17f;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(PlaceAirstrike.attackSoundString, base.gameObject);
            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    if (base.characterMotor.isGrounded)
                    {
                        //Dash
                    }
                    else
                    {
                        base.SmallHop(base.characterMotor, PlaceAirstrike.smallHopVelocity);
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (base.fixedAge >= PlaceAirstrike.baseDuration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
