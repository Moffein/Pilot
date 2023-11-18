using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Parachute
{
    public class AerobaticsDashEntry : AerobaticsDashBase
    {

        protected override void CheckStateExit()
        {
            if (!base.isAuthority) return;
            if (base.fixedAge >= AerobaticsDashBase.baseDuration)
            {
                CheckWallBounceExit();
                return;
            }
        }

        public override void FixedUpdate()
        {
            if (base.isAuthority) CheckWallBounce();
            base.FixedUpdate();
        }

        private void CheckWallBounceExit()
        {
            if (!CheckWallBounce()) this.outer.SetNextStateToMain();
            return;
        }

        //Not the cleanest way to do this
        private bool CheckWallBounce()
        {
            if (base.characterBody && Physics.OverlapSphere(base.characterBody.corePosition, 4f * base.characterBody.radius, LayerIndex.world.mask.value).Length > 0)
            {
                this.outer.SetNextState(new Wallbounce());
                return true;
            }
            return false;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
