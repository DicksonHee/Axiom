using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerUnityEvent : MonoBehaviour
{
    public UnityEvent2 triggerEvent;

    public List<UnityEvent2> triggerEventList;

    [Header("In View Options")]
    public bool triggerInViewOnly;
    public float detectionRange;
    public LayerMask PoiAndGroundMask;

    private Camera playerSight;
   

    private void Awake()
    {
        if(triggerInViewOnly)
        {
            playerSight = Camera.main;
        }
    }

    private void Update()
    {
        if (!triggerInViewOnly || !InView()) return;
        
        InvokeUnityEvent();
    }

    public void InvokeUnityEventIndex(int index)
    {
        triggerEventList[index].Invoke();
    }
    
    public void InvokeUnityEvent()
    {
        triggerEvent.Invoke();
        Destroy(this);
    }

    private bool InView()
    {
        if (!triggerInViewOnly) return false;

        //needed for calculating fov
        Vector3 displacement = transform.position - playerSight.transform.position;
        float LookAngleThreshold = 50;

        //make sure has los
        RaycastHit hit;
        Physics.Linecast(playerSight.transform.position, transform.position, out hit, PoiAndGroundMask, QueryTriggerInteraction.Ignore);

        //if player has focused on the poi, and has los
        if (hit.collider != null && displacement.magnitude <= detectionRange)
            return (Vector3.Angle(displacement, playerSight.transform.forward) <= LookAngleThreshold && hit.collider.gameObject == this.gameObject);
        else
            return false;

        //return false;
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