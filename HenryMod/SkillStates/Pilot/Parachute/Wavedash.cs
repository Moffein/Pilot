using UnityEngine;
using RoR2;
using Pilot.Content.Components;
using System;

namespace EntityStates.Pilot.Parachute
{
    public class Wavedash : BaseState
    {
        public float initialSpeed;
        public Vector3 entryDirection;

        private PilotController pilotController;

        public static float airSpeedLoss = 7.5f; //Speed lost per second when midair
        public static float groundSpeedLoss = 300f; //Speed lost per second when grounded
        public static float groundGracePeriod = 0.15f; //Time before you start losing speed when grounded
        private float speed;
        private float groundStopwatch;

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

                groundStopwatch = 0f;
                speed = initialSpeed;
                base.characterMotor.velocity.x = inputDirection.x * speed;
                base.characterMotor.velocity.z = inputDirection.z * speed;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.StartAimMode(base.GetAimRay(), 3f, false);

            //Be lenient with movespeed so you can shoot while flying without ending the state
            if (base.characterBody && base.moveSpeedStat > base.characterBody.moveSpeed) base.moveSpeedStat = base.characterBody.moveSpeed;

            if (base.isAuthority)
            {
                bool validWavedash = false;
                if (base.characterMotor && base.characterBody)
                {
                    //Slow down wavedash on the ground
                    if (base.characterMotor.isGrounded)
                    {
                        groundStopwatch += Time.fixedDeltaTime;
                        if (groundStopwatch >= Wavedash.groundGracePeriod)
                        {
                            speed -= Time.fixedDeltaTime * Wavedash.groundSpeedLoss;
                        }
                    }
                    else
                    {
                        groundStopwatch = 0f;
                        speed -= Time.fixedDeltaTime * Wavedash.airSpeedLoss;
                    }

                    //Get current velocity
                    float currentSpeedIfAirborne = 0f;
                    Vector3 currentDirection = base.characterMotor.velocity;
                    currentDirection.y = 0;
                    currentSpeedIfAirborne = currentDirection.magnitude;    //Get speed before normalization 
                    currentDirection.Normalize();

                    float targetSpeed = base.characterMotor.isGrounded ? speed : currentSpeedIfAirborne; //Get targetSpeed before normalization

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

                    //Maintain currentSpeed as long as they input in the direction of their velocity.
                    Vector2 currentDirection2d = new Vector2(currentDirection.x, currentDirection.z);
                    Vector2 inputDirection2d = new Vector2(inputDirection.x, inputDirection.z);
                    float maxTurnAngle = 2.5f;
                    float angle = Vector2.Angle(currentDirection2d, inputDirection2d);
                    //Debug.Log("Angle: " + angle + (angle > maxTurnAngle ? ", Losing Speed" : string.Empty));
                    float lerp = (angle <= maxTurnAngle) ? 1f : 1f - ((angle - maxTurnAngle) / (180f - maxTurnAngle)); //Allow for gradual turning without losing speed.

                    if (targetSpeed > this.moveSpeedStat)
                    {
                        targetSpeed = Mathf.Lerp(this.moveSpeedStat, targetSpeed, lerp);
                    }
                    else
                    {
                        targetSpeed = this.moveSpeedStat;
                    }
                    speed = targetSpeed;

                    Vector3 newVelocity = new Vector3(targetSpeed * inputDirection2d.x, base.characterMotor.velocity.y, targetSpeed * inputDirection2d.y);

                    base.characterMotor.velocity = newVelocity;

                    //Check if still wavedashing
                    if (speed > this.moveSpeedStat)
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
