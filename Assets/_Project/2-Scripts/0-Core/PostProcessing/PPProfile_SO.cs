using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "ProfileSettings", fileName = "ProfileSettings")]
public class PPProfile_SO : ScriptableObject
{
   public string scene;
   public AreaName areaName;
   public VolumeProfile volumeProfile;
}

public enum AreaName
{
   MainMenu,
   Dreamscape,
   Memory,
   Nightmare,
   Apartment,
   RealWorld,
   Mindscape
}