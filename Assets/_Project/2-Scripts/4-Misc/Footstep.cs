using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Serialization;


public class Footstep : MonoBehaviour
{
    [SerializeField]
    private StudioEventEmitter eventEmmitter;
    public EventReference footstepEvent;

    [ParamRef]
    [FormerlySerializedAs("parameter")]
    public string globalSurface;

    [FormerlySerializedAs("value")]
    public int Value; //currently 0-7 changes instrument

    [SerializeField]
    private PARAMETER_DESCRIPTION parameterDescription;
    public PARAMETER_DESCRIPTION ParameterDesctription { get { return parameterDescription; } }

    FMOD.RESULT currentValue;
    float currentValuef;

    [SerializeField]
    private string labelName;

   
    private FMOD.RESULT Lookup()
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.getParameterDescriptionByName(globalSurface, out parameterDescription);
        return result;
    }

    private void Awake()
    {
        //if parameter reference is null, look up
        if (string.IsNullOrEmpty(parameterDescription.name))
        {
            Lookup();
        }

        // get the label name once in awake to set string once //
        try
        {
            RuntimeManager.StudioSystem.getParameterLabelByID(parameterDescription.id, (int)currentValuef, out labelName);
        }
        catch(System.Exception ex)
        {
            Debug.Log(ex);
        }
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0)) //press 0 to play// testing
        {
           eventEmmitter.Play();
        }
        //get current value
        currentValue  = RuntimeManager.StudioSystem.getParameterByID(parameterDescription.id, out currentValuef);
        RuntimeManager.StudioSystem.getParameterLabelByID(parameterDescription.id, (int)currentValuef, out labelName);


        //set parameter from changing value
        if(currentValuef == Value) return;
        FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, Value);
       
        if (result != FMOD.RESULT.OK)
        {
            RuntimeUtils.DebugLogError(string.Format(("[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}"), globalSurface, result));
        }
    }
}
