
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
            // Get the start time and play the audio file
            if (dialog.playAudio)
            {
                fmodScript.PlayDialog(dialog.audioFileName, dialogueVolume);
            }
            else
            {
                //if not audio is hidden set volume = 0
                fmodScript.PlayDialog(dialog.audioFileName, 0);
            }
            float audioFileLength = fmodScript.dialogueLength;
            //the time from the start of foreach loop

            float elapsedTime = 0f;
            int timestampIndex = 0;

           // dialogToShow = dialog.GetNextLineToShow();

            while (elapsedTime < audioFileLength / 1000)
            {
                elapsedTime += Time.deltaTime;

                bool hasExecutedCommand = true;

                while(hasExecutedCommand == true)
                {
                    hasExecutedCommand = false;

                    if (timestampIndex < dialog.timestamps.Count && elapsedTime > dialog.timestamps[timestampIndex].timeStamp)
                    {
                        switch (dialog.timestamps[timestampIndex].command)
                        {
                            case TimeStamps.Commands.ShowText:
                                ShowText(dialog.);
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

        //     // While the audio clip is still playing
        //     while (elapsedTime < audioFileLength / 1000)
        //     {
        //         elapsedTime += Time.deltaTime;
        //         //Debug.Log(elapsedTime); 
        //         try
        //         {
        //             if (elapsedTime > dialog.timeStamps[i].timeStamp && i < dialog.timeStamps.Count)
        //             {
        //                 switch (dialog.timeStamps[i].command)
        //                 {
        //                     case TimeStamps.Commands.ShowText:

        //                         ShowText(dialog);
        //                         //Debug.Log("show"+i);
        //                         i++;

        //                         break;
        //                     case TimeStamps.Commands.NextDialogLine:

        //                         NextDialogLine(dialog);
        //                         //Debug.Log("next"+i);
        //                         i++;

        //                         break;
        //                     case TimeStamps.Commands.Mute:

        //                         Mute();
        //                         //Debug.Log("muted"+i);
        //                         i++;

        //                         break;
        //                     case TimeStamps.Commands.Unmute:

        //                         Unmute();
        //                         //Debug.Log("unmuted"+i);
        //                         i++;

        //                         break;
        //                     default:

        //                         Debug.Log("command not found" + i);
        //                         //yield return null;
        //                         i++;

        //                         break;
        //                 }//end switch case
        //             }//end if time
        //             else if (i >= dialog.timeStamps.Count)
        //             {
        //                 Debug.Log("break");
        //                 continue;
        //             }
        //         }
        //         catch
        //         {
        //             //Debug.Log("here");
        //             //continue;
        //         }
        //         yield return null;
        //     }// end while
        //     yield return null;
        // }
    
    //command
    #region  Commands
    private void ShowText(DialogLine dialogToShow)
    {
        if (dialogToShow.showText)
        {
            if (DialogUI.current != null)
                DialogUI.current.UpdateText(dialogToShow.textToShow);

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

