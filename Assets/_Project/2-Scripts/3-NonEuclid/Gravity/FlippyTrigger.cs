using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlippyTrigger : MonoBehaviour
{
    public delegate void EnterEvent(Collider other, FlippyTrigger trigger);
    public EnterEvent OnEnter;

    public delegate void ExitEvent(Collider other, FlippyTrigger trigger);
    public ExitEvent OnExit;

    private void OnTriggerEnter(Collider other) => OnEnter?.Invoke(other, this);
    private void OnTriggerExit(Collider other) => OnExit?.Invoke(other, this);
}
