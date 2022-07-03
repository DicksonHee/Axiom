using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Axiom.Player.Movement
{
    public class PlayerAnimation : MonoBehaviour
    {
        public Transform orientation;

        private Animator _playerAnimator;

        private void Awake()
        {
            _playerAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            transform.rotation = orientation.rotation;
        }

        public void SetMovementDir(Vector3 movementDir)
        {
            _playerAnimator.SetFloat("XVel", movementDir.x);
            _playerAnimator.SetFloat("ZVel", movementDir.z);
        }

        public void SetRotationDir(float movementDelta)
        {
            _playerAnimator.SetFloat("YDelta", movementDelta);
        }

        public void SetBool(string param, bool val)
        {
            _playerAnimator.SetBool(param, val);
        }
    }
}