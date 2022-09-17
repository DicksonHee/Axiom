
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
         if(Input.GetKeyDown(KeyCode.Space)) //for testing
        {
             //dr.StartDialogue(yarnNodeToStartFrom);
            StartCoroutine(DialogueToShow());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // if(other.tag == "Player")
        // {
        //      dr.StartDialogue(yarnNodeToStartFrom);
        // }
    }
    public IEnumerator DialogueToShow()
    {
        foreach (DialogList dialog in dialogListData.dialogLists) // Loop over each dialog in dialog list
        {
            // Get the start time and play the audio file
            fmodScript.PlayDialog(dialog.audioFileName, dialogueVolume);
            float audioFileLength = fmodScript.dialogueLength;
            float startTime = Time.time; //the time from the start of foreach loop

            // Get next dialog line to show
            DialogLine dialogToShow = dialog.GetNextLineToShow();
            
            // While the audio clip is still playing
            while (Time.time < startTime + audioFileLength / 1000)
            {
                // If there is more dialog to show on screen and the current audio clip time is more than the timestamp
                // Show the next line, otherwise do nothing and wait until the audio clip finishes
                if (dialogToShow != null && Time.time - startTime > dialogToShow.timeStamp)
                {
                    Debug.Log(dialogToShow.textToShow);
                    dialogToShow = dialog.GetNextLineToShow();
                }

                yield return null;
            }
        }
    }
}

