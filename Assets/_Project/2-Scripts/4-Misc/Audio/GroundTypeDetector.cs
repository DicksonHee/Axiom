using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeDetector : MonoBehaviour
{
    public Footstep footsteps;
    public LayerMask groundLayer;
    public GroundType_SO groundType_SO;

    public Transform leftFootPosition;
    public Transform rightFootPosition;

    private Dictionary<string, FootstepTypeValue> groundTypeDict;

    private void Awake()
    {
        groundTypeDict = groundType_SO.GenerateDict();
    }
    public void DetectGroundType_L()
    {
        if(Physics.SphereCast(leftFootPosition.position, 0.1f, -transform.up, out RaycastHit hitInfo, 1f, groundLayer))
        {            
            if(hitInfo.collider.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                if (groundTypeDict.TryGetValue(meshRenderer.material.name.ToString().Replace("(Instance)", "").Replace(" ", ""), out FootstepTypeValue value))
                {
                    footsteps.PlayFootstep((int) value);
                }
            }
        }
    }

    public void DetectGroundType_R()
    {
        if (Physics.SphereCast(rightFootPosition.position, 0.1f, -transform.up, out RaycastHit hitInfo, 1f, groundLayer))
        {
            if (hitInfo.collider.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                if (groundTypeDict.TryGetValue(meshRenderer.material.name.ToString().Replace(" (Instance)", "").Replace(" ", ""), out FootstepTypeValue value))
                {
                    footsteps.PlayFootstep((int) value);
                }
            }
        }
    }
}
