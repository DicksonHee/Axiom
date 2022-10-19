using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.NonEuclidean
{
    public class FlippyCollider : MonoBehaviour
    {
        public delegate void EnterEvent(Collision collision, FlippyCollider trigger);
        public EnterEvent OnEnter;

        public delegate void ExitEvent(Collision collision, FlippyCollider trigger);
        public ExitEvent OnExit;

        private void OnCollisionEnter(Collision collision) => OnEnter?.Invoke(collision, this);
        private void OnCollisionExit(Collision collision) => OnExit?.Invoke(collision, this);
    }
}