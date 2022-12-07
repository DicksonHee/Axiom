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
        DialogueTrigger.OnDialogInvokeEvent += TriggerEvent;
    }

    private void OnDisable()
    {
        DialogueTrigger.OnDialogInvokeEvent -= TriggerEvent;
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