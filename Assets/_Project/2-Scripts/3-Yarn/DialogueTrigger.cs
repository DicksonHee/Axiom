using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueTrigger : MonoBehaviour
{
    private DialogueRunner dr;
    // Start is called before the first frame update
    void Awake()
    {
        dr = FindObjectOfType<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space)) //change it to on trigger enter
        {
            dr.StartDialogue("Start");
        }
    }
}
