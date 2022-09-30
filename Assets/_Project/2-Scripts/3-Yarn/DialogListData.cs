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
    public bool playAudio;
    public string audioFileName;
    public int currentDialogLine;
    public List<TimeStamps> timestamps;
}

[Serializable]
public class DialogLine
{
    [Header("Text")]
    public string textToShow;
    public bool showText = true;
    [Header("Start from 0")]
    public List<int> _hiddenWordIndex;
    
    public string RedactDialog()
    {
        //init modded text
        string ModdedText = new string(textToShow);

        //sort list to accending order, because in for loop requires it
        _hiddenWordIndex.Sort();

        //regex
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
              ModdedText = ModdedText.Replace(s,"▇▇▇▇");
            }
            
            ModdedText = ModdedText.Replace("[","");
            ModdedText = ModdedText.Replace("]","");

            return ModdedText;
        }
        else
        {
            //if no redacted words and no array of words need to be hidden
            ModdedText = ModdedText.Replace("[","");
            ModdedText = ModdedText.Replace("]","");

            return ModdedText;
        }
    }
    
}
[Serializable]
public class TimeStamps
{
    public float timeStamp;
    public Commands command;
    public DialogLine dialogLine;

    public enum Commands
    {
        ShowText,
        NextDialogLine,
        Mute,
        Unmute,
    }
}
