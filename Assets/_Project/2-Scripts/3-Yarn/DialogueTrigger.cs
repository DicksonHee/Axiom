
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

public class DialogueTrigger : MonoBehaviour
{
    public DialogListData dialogListData;
    [SerializeField] private ProgrammerSounds fmodScript;
    public float dialogueVolume;

    [ParamRef]
    [FormerlySerializedAs("parameter")]
    public string DialogToStatic;

     [SerializeField]
    private PARAMETER_DESCRIPTION parameterDescription;
    public PARAMETER_DESCRIPTION ParameterDesctription { get { return parameterDescription; } }

    [SerializeField]
    float currentValuef;

    
    ///////////////////////////////////////////////////////////////////////////////////////////
    DialogLine dialogToShow;
    Coroutine dialogCoroutine;
    void Awake()
    {
        //if parameter reference is null, look up
        if (string.IsNullOrEmpty(parameterDescription.name))
        {
            Lookup();
        }
    }
    void Start()
    {
        //  dr = FindObjectOfType<DialogueRunner>();

        //for testing
        FlagSystem.SetBoolValue("Flag1", true);
    }
    void Update()
    {
        //get dialog to static value
        RuntimeManager.StudioSystem.getParameterByID(parameterDescription.id, out currentValuef);

        if (Input.GetKeyDown(KeyCode.Space) && dialogCoroutine == null) //for testing
        {
            //dr.StartDialogue(yarnNodeToStartFrom);

            bool start = false;
            //reset dialogue to show after running
            for (int x = 0; x < dialogListData.dialogLists.Count; x++)
            {
                dialogListData.dialogLists[x].currentDialogLine = 0;
                start = true;
            }
            if (start)
            {
                dialogCoroutine = StartCoroutine(DialogueToShow());
            }
        }

        // if (Input.GetKeyDown(KeyCode.K) && dialogCoroutine != null)
        // {
        //     fmodScript.dialogueInstance.setVolume(0); //mute
        // }
        // if (Input.GetKeyUp(KeyCode.K) && dialogCoroutine != null)
        // {
        //     fmodScript.dialogueInstance.setVolume(dialogueVolume); //unmute
        // }
    }

    private IEnumerator DialogueToShow()
    {
        foreach (Dialog dialog in dialogListData.dialogLists) // Loop over each dialog in dialog list
        {
            // Play the audio file and set the appropriate volume
            //fmodScript.PlayDialog(dialog.audioFileName, dialog.playAudio ? dialogueVolume : 0);
            fmodScript.PlayDialog(dialog.audioFileName, dialogueVolume);
            
            float audioFileLength = fmodScript.dialogueLength; // Get the length of the currently playing audio file
            float elapsedTime = 0f;                            // Reset the elapsed time of each new dialog list entry
            int timestampIndex = 0;                            // Reset the timestamp index

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
                            case TimeStamp.Commands.NextDialogLine:
                                NextDialogLine(dialog.timestamps[timestampIndex].dialogLine);
                                //Debug.Log("next");
                                break;
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
                
                yield return null;
            }
        }
    }

    #region  Commands
    private void ShowText(DialogLine dialogToShow)
    {
        //if (dialogToShow.showText)
        //{
            if (DialogUI.current != null) DialogUI.current.UpdateText(dialogToShow.textToShow);

            try
            {
                Debug.Log(dialogToShow.RedactDialog());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e);
            }
        //}
        //else //if dialogue line is hidden
        //{
            //Debug.Log("text hidden");

        //}
    }
    private void NextDialogLine(DialogLine dialog)
    {
        dialogToShow = dialog;
    }
    private void Mute(string flagToCheck = null)
    {
        if(FlagSystem.GetBoolValue(flagToCheck))
        {
                
            FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, 1);//1 is full static
            //fmodScript.dialogueInstance.setVolume(0);
             
        }
            
    }
    private void Unmute()
    {
        //fmodScript.dialogueInstance.setVolume(dialogueVolume);
        FMOD.RESULT result = RuntimeManager.StudioSystem.setParameterByID(parameterDescription.id, 0); //0 is no static
    }
    private void Stop()
    {
        fmodScript.StopDialog(false);
    }
    #endregion
    private FMOD.RESULT Lookup()
    {
        FMOD.RESULT result = RuntimeManager.StudioSystem.getParameterDescriptionByName(DialogToStatic, out parameterDescription);
        return result;
    }
}

