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
    private int currentDialogLine;
    
    public string audioFileName;
    public List<DialogLine> dialogLines;

    public DialogLine GetNextLineToShow()
    {
        if (currentDialogLine >= dialogLines.Count) return null;
        
        DialogLine retVal = dialogLines[currentDialogLine];
        currentDialogLine++;

        return retVal;
    }
}

[Serializable]
public class DialogLine
{
    public float timeStamp;
    public string textToShow;
}
