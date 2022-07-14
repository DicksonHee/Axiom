using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class InAir : State
    {
        private Vector3 initialDir;
        private float initialSpeed;
        private float initialHeight;
        private float wallClimbTimer;

        public InAir(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.InAir;
        }

        public override void EnterState()
        {
            base.EnterState();

            initialDir = MovementSystem._rb.velocity;
            initialSpeed = MovementSystem._rb.velocity.magnitude;
            initialHeight = MovementSystem.transform.position.y;
            wallClimbTimer = 0f;

            MovementSystem.cameraLook.ApplyCameraXAxisMultiplier(0.5f);
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
            MovementSystem.SetLRMultiplier(0.25f);
            MovementSystem.SetAnimatorBool("InAir", true);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            
            if (MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (MovementSystem.inputDetection.movementInput.z > 0f)
            {
                if (MovementSystem._rb.velocity.y >= 0f && MovementSystem.rbInfo.canWallClimb && !MovementSystem.isExitingClimb) // Check for wall climb
                {
                    wallClimbTimer += Time.deltaTime;
                    if (ShouldWallClimb()) MovementSystem.ChangeState(MovementSystem._climbingState);
                }
                else if (((MovementSystem.rbInfo.IsLeftWallDetected() && MovementSystem.previousWall != MovementSystem.rbInfo.GetLeftWall()) ||
                          (MovementSystem.rbInfo.IsRightWallDetected() && MovementSystem.previousWall != MovementSystem.rbInfo.GetRightWall()))) // Check for wall run
                {
                    MovementSystem.ChangeState(MovementSystem._wallRunningState);
                }
            }
            
            MovementSystem.SetMaxHeight(initialHeight - MovementSystem.transform.position.y);
            MovementSystem.playerAnimation.SetFloatParam("LandHeight", MovementSystem._maxHeight);
            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            MovementSystem.cameraLook.ResetCameraXSens();
            MovementSystem.SetLRMultiplier(1f);
            MovementSystem.SetAnimatorBool("InAir", false);
        }

        private bool ShouldWallClimb()
        {
            return wallClimbTimer > 0.25f;
        }

        private void CalculateInAirSpeed()
        {
            float velDiff = initialSpeed - MovementSystem.idleSpeed;
            float currentSpeed = Mathf.Clamp(initialSpeed - velDiff * MovementSystem.inAirCurve.Evaluate(Time.time - stateStartTime), 0, float.MaxValue);
            Vector3 movementInput = MovementSystem.moveDirection;

            Vector3 moveVel = (initialDir + movementInput).normalized * currentSpeed;
            moveVel.y = MovementSystem._rb.velocity.y;
            MovementSystem._rb.velocity = moveVel;
        }

        public void InAirJump(Vector3 jumpVel)
        {
            initialDir = jumpVel;
            initialSpeed = jumpVel.magnitude;
        }
    }
}

