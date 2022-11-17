using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Serialization;
using Axiom.Dialogue;
using Axiom.Core;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    //public DialogListData dialogListData;
    public float dialogueVolume = 0.8f;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    [ParamRef]
    [FormerlySerializedAs("parameter")]
    public string DialogToStatic;

    [SerializeField] //dialog to static
    private PARAMETER_DESCRIPTION DialogToStaticDescription;
    public PARAMETER_DESCRIPTION DialogToStaticDesctription { get { return DialogToStaticDescription; } }

    [SerializeField]//dialog to static
    float currentValuef;

    [ParamRef]
    [FormerlySerializedAs("parameter")]
    public string DialogDip;
    private PARAMETER_DESCRIPTION DialogDipDescription;
    public PARAMETER_DESCRIPTION DialogDipDesctription { get { return DialogDipDescription; } }
    [SerializeField]
    float currentValuedd;
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    DialogLine dialogToShow;
    Coroutine dialogCoroutine;
    Camera playerSight;

    [Header("Used in view check")]
    public LayerMask PoiAndGroundMask;
    public float detectionRange;
    ///////////////////////////////////////////////////////////////////////////////////////////
    [Header("Events")]
    public UnityEvent onPlayerTriggerEvent; // When player enter's the trigger
    public UnityEvent onViewEvent; //if player looked at the poi the dialog event triggers

    private StudioEventEmitter bgm;
    private bool viewOnce = false;
    private bool enterOnce = false;
   
    void Awake()
    {
        //if parameter reference is null, look up, dialog to static
        if (string.IsNullOrEmpty(DialogToStaticDescription.name))
        {
            LookupDialogToStatic();
        }
        if(string.IsNullOrEmpty(DialogDipDesctription.name))
        {
            LookupDialogDip();
        }

        //get player's camera
        playerSight = Camera.main;
        bgm = Camera.main.GetComponent<StudioEventEmitter>();
    }
    void Start()
    {
        // for (int x = 0; x < dialogListData.dialogLists.Count; x++)
        // {
        //     dialogListData.dialogLists[x].currentDialogLine = 0; //make sure the dialoglist starts at the beginning
        // }

        //for testing need to be removed
        FlagSystem.SetBoolValue("Flag1", true);
    }
    void Update()
    {
        //get dialog to static value
        RuntimeManager.StudioSystem.getParameterByID(DialogToStaticDescription.id, out currentValuef);
        RuntimeManager.StudioSystem.getParameterByID(DialogDipDescription.id, out currentValuedd);

    //    if(Input.GetKeyDown(KeyCode.Alpha0))
    //    {
    //     RuntimeManager.StudioSystem.setParameterByID(DialogToStaticDescription.id, 1);
    //    }

    }
    private void FixedUpdate()
    {
        //maybe give another condition like range
        if(InView() && dialogCoroutine == null && !viewOnce)
        {
            //Invoke when in player's view
            onViewEvent.Invoke();
            viewOnce = true;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && dialogCoroutine == null)// && !enterOnce)
        {
            Debug.Log("ASD");
            //trigger dialog event
            onPlayerTriggerEvent.Invoke();
            enterOnce = true;
        }
    }
    public void Z_StartDialog(DialogListData _data)
    {
        dialogCoroutine = StartCoroutine(DialogToShow(_data));
    }
    private bool InView()
    {
        //needed for calculating fov
        Vector3 displacement = transform.position - playerSight.transform.position;
        float LookAngleThreshold = 50;

        //make sure has los
        RaycastHit hit;
        Physics.Linecast(playerSight.transform.position, transform.position, out hit, PoiAndGroundMask, QueryTriggerInteraction.Ignore);

        //if player has focused on the poi, and has los
        if(hit.collider!=null && displacement.magnitude <= detectionRange)
        return (Vector3.Angle(displacement, playerSight.transform.forward) <= LookAngleThreshold && hit.collider.gameObject == this.gameObject);
        else
        return false;

        //return false;
    }

    //New Show Dialog stuff
    //Event emmiter.play, set volume
    //Get event length

    //while playing get elapsed time
    //if elapsed time = timestamp
    //do timestamp command
    //timestamp index ++

    private IEnumerator DialogToShow(DialogListData dialogListData)
    {
        //get the last dialog
        Dialog last = dialogListData.dialogLists.Last();

        foreach (Dialog dialog in dialogListData.dialogLists) // Loop over each dialog in dialog list
        {
            // Play the audio file and set the appropriate volume
            //ProgrammerSounds.current.PlayDialog(dialog.audioFileName, dialog.playAudio ? dialogueVolume : 0);
            ProgrammerSounds.current.PlayDialog(dialog.audioFileName, SettingsData.dialogVolume / 100f);
            //dip volume
            RuntimeManager.StudioSystem.setParameterByID(DialogDipDesctription.id, 1);
            
            float audioFileLength = ProgrammerSounds.current.dialogueLength; // Get the length of the currently playing audio file
            float elapsedTime = 0f;                            // Reset the elapsed time of each new dialog list entry
            int timestampIndex = 0;                            // Reset the timestamp index

            //dialog.currentDialogLine = 0;

            // While loop for the duration of the audio file length
            while (elapsedTime < audioFileLength / 1000)
            {
                bool hasExecutedCommand = true;
                elapsedTime += Time.deltaTime;

                while(hasExecutedCommand == true)
                {
                    hasExecutedCommand = false;

                    if (timestampIndex < dialog.timestamps.Count && elapsedTime > dialog.timestamps[timestampIndex].timeStamp)
                    {
                        switch (dialog.timestamps[timestampIndex].command)
                        {
                            case TimeStamp.Commands.ShowText:
                                ShowText(dialog.timestamps[timestampIndex].dialogLine);
                                //Debug.Log("show");
                                break;
                            // case TimeStamp.Commands.NextDialogLine:
                            //     NextDialogLine(dialog.timestamps[timestampIndex].dialogLine);
                            //     //Debug.Log("next");
                            //     break;
                            case TimeStamp.Commands.Mute:
                                Mute(dialog.timestamps[timestampIndex].muteFlag);
                                //Debug.Log("mute");
                                break;
                            case TimeStamp.Commands.Unmute:
                                Unmute();
                                //Debug.Log("unmute");
                                break;
                            case TimeStamp.Commands.Stop:
                                Stop();
                                //Debug.Log("stop");
                                break;
                        }

                        timestampIndex++;
                        hasExecutedCommand = true;
                    }
                }
                if(elapsedTime >= audioFileLength/1000 && dialog==last)
                {
                    RuntimeManager.StudioSystem.setParameterByID(DialogDipDesctription.id, 0);
                }
                
                yield return null;
            }
        }
    }

    #region  Commands
    private void ShowText(DialogLine _dialogToShow)
    {
        //if (dialogToShow.showText)
        //{
            if (DialogUI.current != null) DialogUI.current.UpdateText(_dialogToShow.textToShow);

            try
            {
                Debug.Log(_dialogToShow.RedactDialog());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e);
            }
    }
    // private void NextDialogLine(DialogLine dialog)
    // {
    //     dialogToShow = dialog;
    // }
    private void Mute(string flagToCheck = null)
    {
        if(FlagSystem.GetBoolValue(flagToCheck))
        {
                
            FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByID(DialogToStaticDescription.id, 1);//1 is full static
            //ProgrammerSounds.current.dialogueInstance.setVolume(0);
             
        }
    }
    private void Unmute()
    {
        //ProgrammerSounds.current.dialogueInstance.setVolume(dialogueVolume);
        FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByID(DialogToStaticDescription.id, 0); //0 is no static
    }
    private void Stop()
    {
        ProgrammerSounds.current.StopDialog(false);
        dialogCoroutine = null;
    }
    #endregion
    private FMOD.RESULT LookupDialogToStatic()
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.getParameterDescriptionByName(DialogToStatic, out DialogToStaticDescription);
        return result;
    }
    private FMOD.RESULT LookupDialogDip()
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.getParameterDescriptionByName(DialogDip, out DialogDipDescription);
        return result;
    }
}

