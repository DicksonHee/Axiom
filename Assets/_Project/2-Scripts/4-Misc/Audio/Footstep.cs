using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Serialization;
using Axiom.Player.Movement.StateMachine;

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

    [Header("Footstep Audio")]
    public MovementSystem movementSystem;
    public FootstepData_SO footstepDataSO;
    public LayerMask groundLayer;
    private FootstepData currentFootstepData;
    private Dictionary<string, FootstepData> footstepData;

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

        eventEmmitter.EventReference = footstepEvent;
        footstepData = footstepDataSO.GenerateDict();
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
        if(footstepData.TryGetValue(newState, out FootstepData data))
        {
            currentFootstepData = data;
            eventEmmitter.EventInstance.setVolume(data.footstepVolume);
        }
    }

    public void PlayFootstep(int value)
    {
        Value = value;
        RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, Value);
        eventEmmitter.Play();
    }
}

public enum FootstepTypeValue
{
    Default = 0,
    Dirt = 1,
    CarpetJ = 2,
    CarpetH = 3,
    Water = 4,
    WoodJ = 5,
    WoodH = 6,
    Metal = 7
}