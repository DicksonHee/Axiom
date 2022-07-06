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
        
        public InAir(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.InAir;
        }

        public override void EnterState()
        {
            base.EnterState();

            initialDir = MovementSystem._rb.velocity;
            initialSpeed = MovementSystem._rb.velocity.magnitude;
            
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
            MovementSystem.SetLRMultiplier(0.3f);
            MovementSystem.SetAnimatorBool("InAir", true);
            MovementSystem.DisableMovement();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            Debug.Log(MovementSystem.isExitingWallRun);

            if (MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (MovementSystem._rb.velocity.y > 0 && !MovementSystem.isExitingWallRun)
            {
                if(MovementSystem.inputDetection.movementInput.x < 0 && MovementSystem.rbInfo.leftWallDetected) MovementSystem.ChangeState(MovementSystem._wallRunningState);
                else if(MovementSystem.inputDetection.movementInput.x > 0 && MovementSystem.rbInfo.rightWallDetected) MovementSystem.ChangeState(MovementSystem._wallRunningState);
            }

            CalculateInAirSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            MovementSystem.SetLRMultiplier(1f);
            MovementSystem.SetAnimatorBool("InAir", false);
            MovementSystem.EnableMovement();
        }

        private void CalculateInAirSpeed()
        {
            float velDiff = initialSpeed - MovementSystem.idleSpeed;
            float currentSpeed = Mathf.Clamp(initialSpeed - velDiff * MovementSystem.inAirCurve.Evaluate(Time.time - stateStartTime), 0, float.MaxValue);
            Vector3 movementInput = MovementSystem.moveDirection.normalized;
            movementInput.z = 0;

            Vector3 moveVel = (initialDir.normalized + movementInput * 0.3f) * currentSpeed;
            moveVel.y = MovementSystem._rb.velocity.y;
            MovementSystem._rb.velocity = moveVel;
        }
    }
}

