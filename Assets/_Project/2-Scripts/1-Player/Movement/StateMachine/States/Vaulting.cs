using System.Collections;
using System.Collections.Generic;
using Axiom.Player.Movement;
using Axiom.Player.StateMachine;
using UnityEngine;

namespace Axiom.Player.StateMachine
{
    public class Vaulting : State
    {
        public Vaulting(MovementSystem movementSystem) : base(movementSystem)
        {
            stateName = StateName.Vaulting;
        }
    }
}