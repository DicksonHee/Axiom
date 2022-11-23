using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Axiom.Core;
using FMODUnity;
using UnityEngine.Events;
public class FMODParameterChanger : MonoBehaviour
{
    private StudioEventEmitter bgmEmitter;
    public UnityEvent triggerEnter;
    public UnityEvent triggerExit;
    public List<FMODParameter> parameters = new List<FMODParameter>();
    

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(AudioManager.current == null) return;

        bgmEmitter = AudioManager.current.bgmEmitter;
    }
    private void SetParamByIndex(int _paramIndex, float _paramValue)
    {
            string paramName = bgmEmitter.Params[_paramIndex].Name;
            FMOD.RESULT result = bgmEmitter.EventInstance.setParameterByName(paramName, _paramValue);
    }
    private void SetParamByName(string _paramName, float _paramValue)
    {
       bgmEmitter.EventDescription.getParameterDescriptionByName(_paramName, out FMOD.Studio.PARAMETER_DESCRIPTION _pd);
       bgmEmitter.EventInstance.setParameterByID(_pd.id, _paramValue);
    }
    public void Z_ChangeByName()
    {
        foreach(FMODParameter p in parameters)
        {
            SetParamByName(p.parameterName,p.parameterValue);
        }
    }
    public void Z_ChangeByIndex()
    {
        foreach(FMODParameter p in parameters)
        {
            SetParamByIndex((int)p.index ,p.parameterValue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnter.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        triggerExit.Invoke();
    }
}

[System.Serializable]
public struct FMODParameter
{   
    [Header("Use Index or Name")]
    public uint index;
    public string parameterName;
    [Space]
    [Range(0,1)] public float parameterValue;
}
