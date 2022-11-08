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
    private Vector3 currentCheckDirection;

    private void Awake()
    {
        groundTypeDict = groundType_SO.GenerateDict();
        currentCheckDirection = -transform.up;
    }

    private void OnEnable()
    {
        movementSystem.OnStateChanged += StateChanged;
    }

    private void OnDisable()
    {
        movementSystem.OnStateChanged -= StateChanged;
    }

    private void StateChanged(string newState)
    {
        switch (newState)
        {
            case "WallRunning":
                currentCheckDirection = movementSystem.GetIsOnRightWall() ? transform.right : -transform.right;
                break;
            case "Climbing":
                currentCheckDirection = transform.forward;
                break;
            default:
                currentCheckDirection = -transform.up;
                break;
        }
    }

    public void DetectGround()
    {
        if (Physics.Raycast(transform.position, currentCheckDirection, out RaycastHit hitInfo, 2.5f, groundLayer))
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
    }

    public void DetectGroundType_R()
    {
        DetectGround();
    }
}
