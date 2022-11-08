using Axiom.Player.Movement.StateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootstepData", menuName = "Footsteps/FootstepData")]
public class FootstepData_SO : ScriptableObject
{
    public List<FootstepData> footstepDatas;

    public Dictionary<string, FootstepData> GenerateDict()
    {
        Dictionary<string, FootstepData> dict = new();

        foreach(FootstepData data in footstepDatas)
        {
            dict.Add(data.currentState, data);
        }

        return dict;
    }
}

[Serializable]
public class FootstepData
{
    public string currentState;
    [Range(0f, 1f)] public float footstepVolume;
}

