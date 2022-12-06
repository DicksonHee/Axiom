using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogTriggerEvent : MonoBehaviour
{
    public string dialogName;
    public UnityEvent2 unityEvent;

    private void OnEnable()
    {
        DialogueTrigger.OnDialogListComplete += TriggerEvent;
    }

    private void OnDisable()
    {
        DialogueTrigger.OnDialogListComplete -= TriggerEvent;
    }

    public void TriggerEvent(string name)
    {
        if (dialogName == name)
        {
            unityEvent.Invoke();
            Destroy(this);
        }
    }
}
