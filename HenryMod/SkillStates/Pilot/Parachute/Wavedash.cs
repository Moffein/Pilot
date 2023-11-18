using UnityEngine;
using RoR2;
using Pilot.Content.Components;

namespace EntityStates.Pilot.Parachute
{
    public class Wavedash : BaseState
    {
        public float initialSpeed;
        public Vector3 entryDirection;

        private PilotController pilotController;

        public override void OnEnter()
        {
            base.OnEnter();

            pilotController = base.GetComponent<PilotController>();
            if (pilotController) pilotController.isWavedashing = true;

            if (base.characterMotor)
            {
                base.characterMotor.Jump(1f, 1f);

                //Get current velocity direction
                Vector3 currentDirection = entryDirection;
                currentDirection.y = 0;
                currentDirection.Normalize();

                //Get move input
                Vector3 inputDirection = Vector3.zero;
                if (base.inputBank) inputDirection = base.inputBank.moveVector;
                if (inputDirection.x == 0f && inputDirection.z == 0f)
                {
                    inputDirection = currentDirection;
                }
                else
                {
                    if (inputDirection.y != 0f) inputDirection.y = 0f;
                    inputDirection.Normalize();
                }

                base.characterMotor.velocity.x = inputDirection.x * initialSpeed;
                base.characterMotor.velocity.z = inputDirection.z * initialSpeed;
            }
        }

        public override void FixedUpdate()
        {
            if (base.isAuthority)
            {
                bool validWavedash = false;
                if (base.characterMotor && base.characterBody)
                {
                    //Get current velocity
                    Vector3 currentDirection = base.characterMotor.velocity;
                    currentDirection.y = 0;
                    float targetSpeed = currentDirection.magnitude; //Get targetSpeed before normalization
                    currentDirection.Normalize();

                    //Get move input
                    Vector3 inputDirection = Vector3.zero;
                    if (base.inputBank)
                    {
                        inputDirection = base.inputBank.moveVector;
                    }
                    if (inputDirection.x == 0f && inputDirection.z == 0f)
                    {
                        inputDirection = currentDirection;
                    }
                    else
                    {
                        if (inputDirection.y != 0f) inputDirection.y = 0f;
                        inputDirection.Normalize();
                    }

                    //If midair, maintain currentSpeed as long as they input in the direction of their velocity.
                    if (!base.characterMotor.isGrounded)
                    {
                        Vector2 currentDirection2d = new Vector2(currentDirection.x, currentDirection.z);

                        Vector2 inputDirection2d = new Vector2(inputDirection.x, inputDirection.z);
                        float maxTurnAngle = 45f;   //180 degrees total
                        float angle = Vector2.Angle(currentDirection2d, inputDirection2d);
                        float lerp = (angle <= maxTurnAngle) ? 1f : 1f - ((angle - maxTurnAngle) / (180f - maxTurnAngle)); //Allow for gradual turning without losing speed.

                        if (targetSpeed > this.moveSpeedStat)
                        {
                            targetSpeed = Mathf.Lerp(this.moveSpeedStat, targetSpeed, lerp);
                        }
                        else
                        {
                            targetSpeed = this.moveSpeedStat;
                        }

                        Vector3 newVelocity = new Vector3(targetSpeed * inputDirection2d.x, base.characterMotor.velocity.y, targetSpeed * inputDirection2d.y);

                        base.characterMotor.velocity = newVelocity;
                    }
                    

                    //Check if still wavedashing
                    Vector3 checkVelocity = base.characterMotor.velocity;
                    checkVelocity.y = 0f;
                    if (checkVelocity.magnitude > this.moveSpeedStat)
                    {
                        validWavedash = true;
                    }
                }

                if (!validWavedash)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (pilotController) pilotController.isWavedashing = false;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
