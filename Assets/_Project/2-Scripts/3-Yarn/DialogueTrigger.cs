
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Axiom.Dialogue;

public class DialogueTrigger : MonoBehaviour
{
    public List<DialogList> dialogLists;
    [SerializeField]
    ProgrammerSounds fmodScript;

    public float dialogueVolume;
    float waitTime;

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
        for(int x = 0; x < dialogLists.Count;)
        {
           
            fmodScript.PlayDialogue(dialogLists[x].audioFileName, dialogueVolume);
            //show text
            print(dialogLists[x].textToShow);

            waitTime = fmodScript.dialogueLength;
            WaitForSecondsRealtime wait = new WaitForSecondsRealtime(waitTime/1000);
            Debug.Log("wait time: " + waitTime);
            yield return wait;
            x++;
        }
    }
}

