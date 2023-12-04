using MoffeinPilot.Content.Components;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.MoffeinPilot.Weapon
{
    public class ReloadRapidFire : BaseState
    {
        public static string startSoundString = "Play_commando_M2_grenade_throw";
        public static string endSoundString = "Play_captain_m1_reload";
        public static float baseDuration = 1.4f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = ReloadRapidFire.baseDuration / this.attackSpeedStat;

            //Util.PlaySound(ReloadRapidFire.startSoundString, base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                if (base.skillLocator && base.skillLocator.primary)
                {
                    base.skillLocator.primary.stock = base.skillLocator.primary.maxStock;
                }
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            Util.PlaySound(ReloadRapidFire.endSoundString, base.gameObject);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
