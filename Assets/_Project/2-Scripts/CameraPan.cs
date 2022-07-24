using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class CameraPan : MonoBehaviour
{
    public Transform playerEyePos;
    public Camera cam;
    public Transform targetPos;
    public float speed;
    public float returnTimer;

    public UnityEvent panEvent;

    private bool going = false;
    private bool returning = false;
    public void StartPan(Transform _targetPos)
    {
        //targetPos = _targetPos;
        
        StartCoroutine(MoveCam(_targetPos));
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.Return))
        {
            panEvent?.Invoke();
           //StartCoroutine(MoveCam(targetPos));
        }
    }
    private void FixedUpdate()
    {
        // if(going)
        // {
        //     cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos.position, speed*Time.deltaTime);
        //     cam.transform.eulerAngles = Vector3.Lerp(cam.transform.eulerAngles, targetPos.eulerAngles, speed*Time.deltaTime);
        // }
        // if(returning)
        // {
        //     cam.transform.position = Vector3.Lerp(cam.transform.position, playerEyePos.position, speed*Time.deltaTime);
        //     cam.transform.eulerAngles = Vector3.Lerp(cam.transform.eulerAngles, playerEyePos.eulerAngles, speed*Time.deltaTime);
        // }
    }
    
    public IEnumerator MoveCam(Transform _targetPos)
    {
        going = true;
        if(going)
        {
            cam.transform.DOMove(_targetPos.position, 2f).SetEase(Ease.InOutCubic);
            cam.transform.DORotateQuaternion(_targetPos.rotation, 2f).SetEase(Ease.InOutCubic);
        }
       yield return new WaitForSeconds(returnTimer);
        
        //Return Camera back to player
        going = false;
        returning = true;
         if(returning)
         {
            cam.transform.DOMove(playerEyePos.position, 2f).SetEase(Ease.InOutCubic);
            cam.transform.DORotateQuaternion(playerEyePos.rotation, 2f).SetEase(Ease.InOutCubic);
         }
        yield return new WaitForSeconds(returnTimer);
       returning = false;
    }
    
    public void ResetCamera()
    {
        cam.transform.position = playerEyePos.transform.position;
        cam.transform.eulerAngles = playerEyePos.transform.eulerAngles;
    }
    
}
