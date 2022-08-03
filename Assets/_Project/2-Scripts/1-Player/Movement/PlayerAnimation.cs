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
        
        private Vector3 currentModelRotation;
        private bool rotationFinished;
        
        private void Awake()
        {
            _playerAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            if(isRotationEnabled) transform.localRotation = orientation.localRotation * Quaternion.Euler(currentModelRotation);
        }

        private IEnumerator LerpRotation_CO(Vector3 rot, float duration)
        {
            rotationFinished = false;
            float startTime = Time.time;
            Vector3 initialModelRotation = transform.localRotation.eulerAngles;
            initialModelRotation.y = 0;
            while (Time.time - startTime < duration)
            {
                currentModelRotation = Vector3.Lerp(initialModelRotation, rot, (Time.time - startTime) / duration);
                yield return null;
            }

            currentModelRotation = rot;
            rotationFinished = true;
        }
        
        public void SetMovementDir(Vector3 movementDir)
        {
            _playerAnimator.SetFloat(XVel, movementDir.x, 0.1f, Time.deltaTime);
            _playerAnimator.SetFloat(ZVel, movementDir.z, 0.1f, Time.deltaTime);
        }

        public void SetClimbHandPositions()
        {
            if (!rbInfo.CanClimbLedge()) return;

            leftHandClimbPositionTarget.position = rbInfo.GetLeftHandPosition();
            rightHandClimbPositionTarget.position = rbInfo.GetRightHandPosition();
        }

        public void SetVaultHandPositions()
        {
            leftHandClimbPositionTarget.position = rbInfo.GetLeftHandPosition();
            rightHandClimbPositionTarget.position = rbInfo.GetRightHandPosition();
        }

        public void SetRotation(Vector3 rot, float duration)
        {
            if(!rotationFinished || Vector3.Dot(rot, currentModelRotation) > 0.8f) return;
            
            rot.y = 0;
            StartCoroutine(LerpRotation_CO(rot, duration));
        }

        public void ResetRotation()
        {
            StartCoroutine(LerpRotation_CO(Vector3.zero, 1f));
        }
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