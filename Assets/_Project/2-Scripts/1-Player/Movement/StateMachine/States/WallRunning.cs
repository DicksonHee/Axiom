using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class WallRunning : State
    {
        public WallRunning(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.WallRunning;
        }
    }
}

