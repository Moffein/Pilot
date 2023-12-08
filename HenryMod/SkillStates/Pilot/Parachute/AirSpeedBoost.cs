using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.MoffeinPilot.Parachute
{
    public class AirSpeedBoost : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active && base.characterBody)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.CloakSpeed);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.isGrounded)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {

            if (NetworkServer.active && base.characterBody)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.CloakSpeed);
            }
            base.OnExit();
        }
    }
}
