using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvents : MonoBehaviour
{
    public SerialisableStringEventKeyValuePair[] serialisedEvents;
    private Dictionary<string, UnityEvent> events;

    private void Awake()
    {
        events = new Dictionary<string, UnityEvent>(serialisedEvents.Length);
        for (int i = 0; i < serialisedEvents.Length; i++)
            events.Add(serialisedEvents[i].name, serialisedEvents[i].unityEvent);
    }

    public void InvokeEvent(string name) => events[name]?.Invoke();

    [System.Serializable]
    public class SerialisableStringEventKeyValuePair
    {
        public string name;
        public UnityEvent unityEvent;
    }
}
