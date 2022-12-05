using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Axiom.UI.MainMenu;

public class MindTransition : MonoBehaviour
{
    public Camera overlayCam;
    public Animator overlayFadeAnim;
    public MainMenuAnim[] animObjects;
    public PerspectiveSwitcher perspectiveSwitcher;

    public void Destroy(float delay = 0f) => Destroy(gameObject, delay);
}
