
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
         if(Input.GetKeyDown(KeyCode.Space) && dialogCoroutine == null) //for testing
        {
            bool start = false;
             //dr.StartDialogue(yarnNodeToStartFrom);
           
            //reset dialogue to show after running
            for(int x = 0; x < dialogListData.dialogLists.Count; x++)
            {
                dialogListData.dialogLists[x].currentDialogLine = 0;
                start = true;
            }
            if(start)
            {
                dialogCoroutine = StartCoroutine(DialogueToShow());
            }
        }
    }
    
    private IEnumerator DialogueToShow()
    {
        foreach (DialogList dialog in dialogListData.dialogLists) // Loop over each dialog in dialog list
        {
            // Get the start time and play the audio file
            if(dialog.playAudio)
            {
                fmodScript.PlayDialog(dialog.audioFileName, dialogueVolume);
            }
            else
            {
                //if not audio is hidden set volume = 0
                fmodScript.PlayDialog(dialog.audioFileName, 0);
            }
            float audioFileLength = fmodScript.dialogueLength;
            float startTime = Time.time; //the time from the start of foreach loop

            // Get next dialog line to show
            dialogToShow = dialog.GetNextLineToShow();
            
            // While the audio clip is still playing
            while (Time.time < startTime + audioFileLength / 1000)
            {
                // If there is more dialog to show on screen and the current audio clip time is more than the timestamp
                // Show the next line, otherwise do nothing and wait until the audio clip finishes
                if (dialogToShow != null && Time.time - startTime > dialogToShow.timeStamp)
                {
                    if(dialogToShow.showText)
                    {
                        if(DialogUI.current!=null)
                        DialogUI.current.UpdateText(dialogToShow.textToShow);

                        try
                        {
                            Debug.Log(dialogToShow.GetPart());
                        }
                        catch(ArgumentOutOfRangeException e)
                        {
                            Debug.Log(e);
                        }

                        dialogToShow = dialog.GetNextLineToShow();
                    }
                    else //if dialogue line is hidden
                    {
                        Debug.Log("text hidden");
                        dialogToShow = dialog.GetNextLineToShow();
                    }
                    
                }
                
              //  Debug.Log(Time.time.ToString() + "//" +(startTime + audioFileLength / 1000).ToString());
                yield return null;
            }
        }

        yield return new WaitForSeconds(2f);
        dialogCoroutine = null;

        if(DialogUI.current!=null)
        DialogUI.current.HideText();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // if(other.tag == "Player")
        // {
        //      dr.StartDialogue(yarnNodeToStartFrom);
        // }
    }
}

