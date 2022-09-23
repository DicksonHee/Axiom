using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Text.RegularExpressions;
using System.Linq;

[CreateAssetMenu(menuName = "Dialog/Data", fileName = "DialogData")]
public class DialogListData : ScriptableObject
{
    public List<DialogList> dialogLists;
}

[Serializable]
public class DialogList
{
    [Header("Audio // make sure no additional spaces")]
    public string audioFileName;
    public bool playAudio = true;
    [Header("Dialog Line")]
    public int currentDialogLine;
    public List<DialogLine> dialogLines;
   
    public void ResetCurrentDialogLine()
    {
        currentDialogLine = 0;
    }
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
    [Header("Time Stamp // when to start displaying text after audio plays")]
    public float timeStamp;
    [Header("Text")]
    public string textToShow;
    public bool showText = true;
    [Header("Choose hide index // start from 0")]
    public List<int> _hiddenWordIndex;
    
    
    public string GetPart()
    {
        string ModdedText = new string(textToShow);
        string pattern = @"\[(\S*?\s*?){0,10}?\]";
        Regex regex = new Regex(pattern, RegexOptions.None);

        //matches
        MatchCollection matches = regex.Matches(textToShow);

        //redacted list added from match in matches
        List<string> stringToRedact = new List<string>();

        if(matches!=null)
        {
            for(int x = 0; x < matches.Count;)
            {
                if(_hiddenWordIndex.Contains(x))
                {
                    stringToRedact.Add(matches[x].Value);
                }
                x++;
            }

            foreach(string s in stringToRedact)
            {
              ModdedText = ModdedText.Replace(s,"");
            }
            //if no redacted words and no array of words need to be hidden
            ModdedText = ModdedText.Replace("[","");
            ModdedText = ModdedText.Replace("]","");

            return ModdedText;
        }

        else
        {
            ModdedText = ModdedText.Replace("[","");
            ModdedText = ModdedText.Replace("]","");

            return ModdedText;
        }
    }
}
