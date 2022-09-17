using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName = "Dialog/Data", fileName = "DialogData")]
public class DialogListData : ScriptableObject
{
    public List<DialogList> dialogLists;
}

[Serializable]
public class DialogList
{
    public string audioFileName;
   // public float timeStamp;
    public string textToShow;
}
