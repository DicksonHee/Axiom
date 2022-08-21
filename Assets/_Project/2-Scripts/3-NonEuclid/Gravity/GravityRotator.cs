using System;
using UnityEngine;
using Axiom.Core;

namespace Axiom.NonEuclidean
{
    public class GravityRotator : MonoBehaviour
    {
        private bool isRotating;

        private void Awake()
        {
            InvokeRepeating(nameof(CheckPerfectRotation), 0f, 0.5f);
        }

        void LateUpdate()
        {
            CheckRotation();
        }

        private void CheckRotation()
        {
            isRotating = false;
            Vector3 rotateFrom = -transform.up.normalized;
            Vector3 rotateTo = Physics.gravity.normalized;
            if (Vector3.Dot(rotateFrom, rotateTo) > 0.99f) return;
            
            Debug.Log("ASd");
            isRotating = true;
            Vector3 cross = Vector3.Cross(rotateFrom, rotateTo);
            float angle = Vector3.SignedAngle(rotateFrom, rotateTo, cross);
            transform.Rotate(cross, angle, Space.World);
        }

        private void CheckPerfectRotation()
        {
            if (isRotating) return;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.RoundToSpecificNumber(90));
        }
    }
}
