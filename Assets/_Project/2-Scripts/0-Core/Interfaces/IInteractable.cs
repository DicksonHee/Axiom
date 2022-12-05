using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Core
{
    public interface IInteractable
    {
        public void StartInteraction();
        public void StopInteraction();
        public void Hover();
        public void EndHover();
    }
}