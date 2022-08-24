using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class DialogueTrigger : MonoBehaviour
{
    private DialogueRunner dr;
    public string yarnNodeToStartFrom;
    // Start is called before the first frame update
    void Awake()
    {
        dr = FindObjectOfType<DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space)) //for testing
        {
            dr.StartDialogue(yarnNodeToStartFrom);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
             dr.StartDialogue(yarnNodeToStartFrom);
        }
    }
}
