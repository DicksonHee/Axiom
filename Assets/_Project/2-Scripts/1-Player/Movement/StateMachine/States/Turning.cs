using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Turning : State
    {
        public Turning(MovementSystem movementSystem) : base(movementSystem)
        {
        }
    }
}

