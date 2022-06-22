using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Climbing : State
    {
        public Climbing(MovementSystem movementSystem) : base(movementSystem)
        {
        }
    }
}