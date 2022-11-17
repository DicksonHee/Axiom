using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerUnityEvent : MonoBehaviour
{
    public UnityEvent2 triggerEvent;

    public List<UnityEvent2> triggerEventList;

    public void InvokeUnityEventIndex(int index)
    {
        triggerEventList[index].Invoke();
    }
    
    public void InvokeUnityEvent()
    {
        triggerEvent.Invoke();
        Destroy(this);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggerEvent.Invoke();
            Destroy(this);
        }
    }
}