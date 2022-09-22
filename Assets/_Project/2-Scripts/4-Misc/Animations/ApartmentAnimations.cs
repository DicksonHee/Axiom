using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentAnimations : MonoBehaviour
{
    public GameObject cutsceneModel;
    public GameObject playerGO;

    public float blackBarAmount;
    public float blurAmount;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            cutsceneModel.SetActive(false);
            playerGO.SetActive(true);
        }
    }
}
