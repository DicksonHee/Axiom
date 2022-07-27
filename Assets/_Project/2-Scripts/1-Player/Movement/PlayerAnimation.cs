using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Axiom.Player.Movement
{
    public class PlayerAnimation : MonoBehaviour
    {
        public RigidbodyDetection rbInfo;
        public Transform orientation;
        public LayerMask groundLayer;

        public Transform leftHandClimbPositionTarget;
        public Transform rightHandClimbPositionTarget;

        private bool isRotationEnabled = true;

        private Animator _playerAnimator;
        private static readonly int JumpType = Animator.StringToHash("JumpType");
        private static readonly int WallRunType = Animator.StringToHash("WallRunType");
        private static readonly int XVel = Animator.StringToHash("XVel");
        private static readonly int YDelta = Animator.StringToHash("YDelta");
        private static readonly int ZVel = Animator.StringToHash("ZVel");
        private static readonly int LandType = Animator.StringToHash("LandType");
        private static readonly int InAirType = Animator.StringToHash("InAirType");
       
        
        private void Awake()
        {
            _playerAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            if(isRotationEnabled) transform.DOLocalRotateQuaternion(orientation.localRotation, 0.1f);
        }
        public void MoveWithinCapsule()
        {
            transform.DOLocalMove(transform.forward.normalized * 0.2f, 0.1f);
        }

        //public void MoveWithinCapsule(Vector3 targetPos)
        //{
        //    transform.DOLocalMove(targetPos, 0.3f);
        //}

        public void MoveToCenter()
        {
            transform.DOLocalMove(Vector3.zero, 0.25f);
        }

        public void SetMovementDir(Vector3 movementDir)
        {
            _playerAnimator.SetFloat(XVel, movementDir.x, 0.1f, Time.deltaTime);
            _playerAnimator.SetFloat(ZVel, movementDir.z, 0.1f, Time.deltaTime);
        }

        public void SetClimbHandPositions()
        {
            if (!rbInfo.isDetectingLedge) return;

            leftHandClimbPositionTarget.position = rbInfo.GetLeftHandPosition();
            rightHandClimbPositionTarget.position = rbInfo.GetRightHandPosition();
        }

        public void SetRotation(Quaternion rot) => transform.DOLocalRotateQuaternion(rot, 0.1f);
        public void DisableRotation() => isRotationEnabled = false;
        public void EnableRotation() => isRotationEnabled = true;

        public void SetRotationDir(float movementDelta) => _playerAnimator.SetFloat(YDelta, movementDelta, 0.1f, Time.deltaTime);
        
        public void SetBool(string param, bool val) => _playerAnimator.SetBool(param, val);

        public void SetTrigger(string param) => _playerAnimator.SetTrigger(param);

        public void ResetTrigger(string param) => _playerAnimator.ResetTrigger(param);

        public void SetJumpParam(float val) => _playerAnimator.SetFloat(JumpType, val);

        public void SetLandParam(float val) => _playerAnimator.SetFloat(LandType, val);

        public void SetWallRunParam(float val) => _playerAnimator.SetFloat(WallRunType, val);

        public void SetInAirParam(float val) => _playerAnimator.SetFloat(InAirType, val);

        public void SetFloatParam(string param, float val) => _playerAnimator.SetFloat(param, val);

    }
}