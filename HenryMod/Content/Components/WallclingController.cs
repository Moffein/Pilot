using RoR2;
using System;
using UnityEngine;

namespace Pilot.Content.Components
{
    public class WallclingController : MonoBehaviour
    {
        private EntityStateMachine parachuteMachine;
        private CharacterMotor characterMotor;

        private void Awake()
        {
            parachuteMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Parachute");
            characterMotor = base.GetComponent<CharacterMotor>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if ((collision.gameObject.layer & LayerIndex.world.mask) == 0 || !characterMotor || characterMotor.isGrounded || !parachuteMachine) return;
            Type parachuteState = parachuteMachine.state.GetType();
            if (parachuteState == typeof(EntityStates.Pilot.Parachute.AerobaticsDash))
            {
                (parachuteMachine.state as EntityStates.Pilot.Parachute.AerobaticsDash).TriggerWallcling();
            }
        }
    }
}
