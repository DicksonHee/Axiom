using Axiom.Core;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBGMTrigger : MonoBehaviour
{
    public EventReference bgmReference;
    public string parameterName;
    public float startVal;
    public float endVal;
    public float duration;

    public bool changeBGM;
    public bool lerpBGMParameter;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(changeBGM) AudioManager.current.SetNewBGM(bgmReference, duration);
            if (lerpBGMParameter) AudioManager.current.LerpBGMParameter(parameterName, startVal, endVal, duration);
        }
    }
}