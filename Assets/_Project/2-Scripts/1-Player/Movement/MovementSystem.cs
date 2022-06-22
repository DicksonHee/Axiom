using System;
using UnityEngine;

namespace Axiom.Player.Movement
{
    [RequireComponent(typeof(RigidbodyDetection))]
    public class MovementSystem : StateMachine.StateMachine
    {
        public RigidbodyDetection rbInfo;
    }
}
