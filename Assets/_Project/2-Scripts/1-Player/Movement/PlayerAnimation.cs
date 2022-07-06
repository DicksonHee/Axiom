using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
            transform.DOLocalRotateQuaternion(orientation.rotation, 0.1f);
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

        public void SetTrigger(string param)
        {
            _playerAnimator.SetTrigger(param);
        }

        public void ResetTrigger(string param)
        {
            _playerAnimator.ResetTrigger(param);
        }

        public void SetJumpParam(float val)
        {
            _playerAnimator.SetFloat("JumpType", val);
        }
    }
}