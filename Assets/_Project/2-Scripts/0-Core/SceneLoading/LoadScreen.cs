using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    public static LoadScreen current;

    public GameObject loadScreenCamera;
    public Animator loadScreenAnimator;
    public Image loadScreen;
    private bool isWhite;
    
    private void Awake()
    {
        if (current != null && current != this) Destroy(this);
        else current = this;
    }

    public void SetClear()
    {
        if (!isWhite) return;
        
        isWhite = false;
        loadScreenAnimator.SetTrigger("SetClear");
    }
    
    public void SetOpaque()
    {
        if (isWhite) return;
        
        isWhite = true;
        loadScreenAnimator.SetTrigger("SetWhite");
    }

    public void SetBlack() => loadScreen.color = new Color(0, 0, 0, loadScreen.color.a);
    public void SetWhite() => loadScreen.color = new Color(1, 1, 1, loadScreen.color.a);

    public void SetCameraActive() => loadScreenCamera.SetActive(true);
    public void SetCameraInactive() => loadScreenCamera.SetActive(false);
}