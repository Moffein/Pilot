using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Pilot.Airstrike
{
    public class DashAir : DashGround
    {
        public override float GetBlinkSpeed()
        {
            return 5.4375f; //Half of DashGround
        }

        public override void DashPhysics()
        {
            if (base.characterMotor && base.characterDirection)
            {
                if (base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y = 0;
                base.characterMotor.rootMotion.y += (this.moveSpeedStat * GetBlinkSpeed() * Time.fixedDeltaTime);
                //base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * GetBlinkSpeed() * Time.fixedDeltaTime);
            }
        }

        public override void CreateBlinkEffect(Vector3 origin) { }
    }
}
