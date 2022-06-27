using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class InAir : State
    {
        private Vector3 startPos;
        
        public InAir(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.InAir;
        }

        public override void EnterState(StateName prevState)
        {
            base.EnterState(prevState);

            startPos = MovementSystem.transform.position;
            MovementSystem.SetDrag(MovementSystem.stoppingDrag);
            MovementSystem.SetGravity(MovementSystem.inAirGravity);
            MovementSystem.SetTargetSpeed(MovementSystem.inAirSpeed);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (MovementSystem.rbInfo.isGrounded) MovementSystem.ChangeState(MovementSystem._idleState);

                CalculateMovementSpeed();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void ExitState()
        {
            base.ExitState();
            
            Debug.Log(Vector3.Distance(startPos, MovementSystem.transform.position));
        }

        protected override void SelectMovementCurve()
        {
            base.SelectMovementCurve();
            
            switch (previousState)
            {
                case StateName.Idle:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
                case StateName.Walking:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.Running:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.Strafing:
                    movementCurve = MovementSystem.decelerationCurve;
                    break;
                case StateName.InAir:
                    break;
                case StateName.Climbing:
                    break;
                case StateName.Sliding:
                    break;
                case StateName.WallRunning:
                    break;
                case StateName.BackRunning:
                    movementCurve = MovementSystem.accelerationCurve;
                    break;
            }
        }
    }
}

