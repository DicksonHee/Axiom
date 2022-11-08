using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundTypeData", menuName = "Footsteps/GroundData")]
public class GroundType_SO : ScriptableObject
{
    public List<GroundType> groundTypes;

    public Dictionary<string, FootstepTypeValue> GenerateDict()
    {
        Dictionary<string, FootstepTypeValue> dict = new();

        foreach(GroundType type in groundTypes)
        {
            dict.Add(type.material.name.ToString().Replace(" ", ""), type.footstepValue);
        }

        return dict;
    }
}

[Serializable]
public class GroundType
{
    public Material material;
    public FootstepTypeValue footstepValue;
}