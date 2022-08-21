using System;
using UnityEngine;
using Axiom.Core;
using DG.Tweening;

namespace Axiom.NonEuclidean
{
    public class GravityRotator : MonoBehaviour
    {
        void Update()
        {
            CheckRotation();
        }

        private void CheckRotation()
        {
            Vector3 rotateFrom = -transform.up;
            Vector3 rotateTo = Physics.gravity;
            if (Vector3.Dot(rotateFrom, rotateTo) > 0.99f) return;


            Vector3 cross = Vector3.Cross(rotateFrom, rotateTo);
            float angle = Vector3.SignedAngle(rotateFrom, rotateTo, cross);
            transform.Rotate(cross, angle, Space.World);
        }
    }
}
