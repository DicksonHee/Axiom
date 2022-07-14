using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Walking : State
    {
        private float toRunCounter;
        
        public Walking(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Walking;
        }

        public override void EnterState()
        {
            base.EnterState();

            MovementSystem.SetAnimatorBool("Walking", true);
            MovementSystem.SetTargetSpeed(MovementSystem.walkSpeed);

            if (MovementSystem.GetCurrentSpeed() >= MovementSystem.currentTargetSpeed) MovementSystem.ChangeState(MovementSystem._runningState);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem.inputDetection.movementInput.magnitude <= 0 || MovementSystem.inputDetection.movementInput.z < 0) MovementSystem.ChangeState(MovementSystem._idleState);
            else if (Mathf.Abs(MovementSystem.inputDetection.movementInput.x) > 0f) MovementSystem.ChangeState(MovementSystem._strafingState);
            else if (MovementSystem.inputDetection.movementInput.z > 0 && Time.time - stateStartTime > 0.75f)MovementSystem.ChangeState(MovementSystem._runningState);
            else if (MovementSystem.inputDetection.crouchInput && MovementSystem._rb.velocity.y <= 0.1f) MovementSystem.ChangeState(MovementSystem._slidingState);

            CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            MovementSystem.SetAnimatorBool("Walking", false);
        }
    }
}