using Axiom.Player.Movement.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class GroundTypeDetector : MonoBehaviour
{
    public Footstep footsteps;
    public MovementSystem movementSystem;

    public LayerMask groundLayer;
    public GroundType_SO groundType_SO;

    public Transform leftFootPosition;
    public Transform rightFootPosition;

    private Dictionary<string, FootstepTypeValue> groundTypeDict;
    private Vector3 currentCheckDirection_L;
    private Vector3 currentCheckDirection_R;

    private void Awake()
    {
        groundTypeDict = groundType_SO.GenerateDict();
        currentCheckDirection_L = -leftFootPosition.up;
        currentCheckDirection_R = -rightFootPosition.up;
    }

    private void OnEnable()
    {
        movementSystem.OnStateChanged += StateChanged;
    }

    private void OnDisable()
    {
        movementSystem.OnStateChanged -= StateChanged;
    }

    //private void StateChanged(string newState)
    //{
    //    switch(newState)
    //    {
    //        case "WallRunning":
    //            if(movementSystem.GetIsOnRightWall())
    //            {
    //                currentCheckDirection_L = leftFootPosition.right;
    //                currentCheckDirection_R = rightFootPosition.right;
    //            }
    //            else
    //            {
    //                currentCheckDirection_L = -leftFootPosition.right;
    //                currentCheckDirection_R = -rightFootPosition.right;
    //            }
    //            break;
    //        case "Climbing":
    //            currentCheckDirection_L = leftFootPosition.forward;
    //            currentCheckDirection_R = rightFootPosition.forward;
    //            break;
    //        default:
    //            currentCheckDirection_L = -leftFootPosition.up;
    //            currentCheckDirection_R = -rightFootPosition.up;
    //            break;
    //    }
    //}

    private void StateChanged(string newState)
    {
        switch (newState)
        {
            case "WallRunning":
                if (movementSystem.GetIsOnRightWall())
                {
                    currentCheckDirection_L = transform.right;
                }
                else
                {
                    currentCheckDirection_L = -transform.right;
                }
                break;
            case "Climbing":
                currentCheckDirection_L = transform.forward;
                break;
            default:
                currentCheckDirection_L = -transform.up;
                break;
        }
    }

    public void DetectGround()
    {
        if (Physics.Raycast(transform.position, currentCheckDirection_L, out RaycastHit hitInfo, 2.5f, groundLayer))
        {
            if (hitInfo.collider.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                if (groundTypeDict.TryGetValue(meshRenderer.material.name.ToString().Replace("(Instance)", "").Replace(" ", ""), out FootstepTypeValue value))
                {
                    footsteps.PlayFootstep((int)value);
                }
            }
        }
    }
    
    public void DetectGroundType_L()
    {
        DetectGround();

        //if (Physics.Raycast(leftFootPosition.position, currentCheckDirection_L, out RaycastHit hitInfo, 1f, groundLayer))
        //{
        //    Debug.Log(hitInfo.collider.name);
        //    if (hitInfo.collider.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        //    {
        //        if (groundTypeDict.TryGetValue(meshRenderer.material.name.ToString().Replace("(Instance)", "").Replace(" ", ""), out FootstepTypeValue value))
        //        {
        //            footsteps.PlayFootstep((int)value);
        //        }
        //    }
        //}
    }

    public void DetectGroundType_R()
    {
        DetectGround();

        //if (Physics.Raycast(rightFootPosition.position, currentCheckDirection_R, out RaycastHit hitInfo, 1f, groundLayer))
        //{
        //    Debug.Log(hitInfo.collider.name);
        //    if (hitInfo.collider.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
        //    {
        //        if (groundTypeDict.TryGetValue(meshRenderer.material.name.ToString().Replace(" (Instance)", "").Replace(" ", ""), out FootstepTypeValue value))
        //        {
        //            footsteps.PlayFootstep((int)value);
        //        }
        //    }
        //}
    }
}
