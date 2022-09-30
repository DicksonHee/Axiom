
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Axiom.Dialogue;

public class DialogueTrigger : MonoBehaviour
{
    public DialogListData dialogListData;
    [SerializeField] private ProgrammerSounds fmodScript;
    public float dialogueVolume;
    DialogLine dialogToShow;
    Coroutine dialogCoroutine;
    /////////////////////////////////////


    // private DialogueRunner dr;
    // public string yarnNodeToStartFrom;
    // Start is called before the first frame update
    void Start()
    {
        //  dr = FindObjectOfType<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
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

        if (Input.GetKeyDown(KeyCode.K) && dialogCoroutine != null)
        {
            fmodScript.dialogueInstance.setVolume(0); //mute
        }
        if (Input.GetKeyUp(KeyCode.K) && dialogCoroutine != null)
        {
            fmodScript.dialogueInstance.setVolume(dialogueVolume); //unmute
        }
    }

    private IEnumerator DialogueToShow()
    {
        foreach (DialogList dialog in dialogListData.dialogLists) // Loop over each dialog in dialog list
        {
            // Play the audio file and set the appropriate volume
            fmodScript.PlayDialog(dialog.audioFileName, dialog.playAudio ? dialogueVolume : 0);
            
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
                            case TimeStamps.Commands.ShowText:
                                ShowText(dialog.timestamps[timestampIndex].dialogLine);
                                break;
                            case TimeStamps.Commands.NextDialogLine:
                                NextDialogLine(dialog.timestamps[timestampIndex].dialogLine);
                                break;
                            case TimeStamps.Commands.Mute:
                                Mute();
                                break;
                            case TimeStamps.Commands.Unmute:
                                Unmute();
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
        if (dialogToShow.showText)
        {
            if (DialogUI.current != null) DialogUI.current.UpdateText(dialogToShow.textToShow);

            try
            {
                Debug.Log(dialogToShow.RedactDialog());
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.Log(e);
            }
        }
        else //if dialogue line is hidden
        {
            Debug.Log("text hidden");

        }
    }
    private void NextDialogLine(DialogLine dialog)
    {
        dialogToShow = dialog;
    }
    private void Mute()
    {
        fmodScript.dialogueInstance.setVolume(0);
    }
    private void Unmute()
    {
        fmodScript.dialogueInstance.setVolume(dialogueVolume);
    }
    #endregion
}

