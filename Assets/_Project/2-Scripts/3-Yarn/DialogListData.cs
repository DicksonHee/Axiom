using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Axiom.Core;

[CreateAssetMenu(menuName = "Dialog/Data", fileName = "DialogData")]
public class DialogListData : ScriptableObject
{
    public string name;
    public List<Dialog> dialogLists;
}

[Serializable]
public class Dialog
{
    public string audioFileName;
    //public bool playAudio;
    public int currentDialogLine;
    public List<TimeStamp> timestamps;
}

[Serializable]
public class TimeStamp
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
        Stop,
    }
    public string muteFlag;
}


[Serializable]
public class DialogLine
{
    [Header("Text")]
    public string textToShow;
    //public bool showText = true;
    //[Header("Start from 0")]

    //public List<int> _hiddenWordIndex;
    public List<HiddenWord> hiddenWords;
    public string RedactDialog(string _flag = null)
    {
        //init modded text
        string ModdedText = new string(textToShow);

        //sort list to accending order, because in for loop requires it
        //_hiddenWordIndex.Sort();

        //regex
        string pattern = @"\[(\S*?\s*?){0,10}?\]";
        Regex regex = new Regex(pattern, RegexOptions.None);

        //matches
        MatchCollection matches = regex.Matches(textToShow);

        //redacted list added from match in matches
        List<string> stringToRedact = new List<string>();

        if(matches!=null)
        {
            //if hidden Word Index contains one of the number in x for loop, add that match to string to redact
            //need to use dictionary
            /* for(int x = 0; x < matches.Count;)
            {
                if(_hiddenWordIndex.Contains(x))
                {
                    stringToRedact.Add(matches[x].Value);
                }
                x++;
            }*/
            foreach(HiddenWord h in hiddenWords)
            {
                try
                {   
                    //check flag b4 redacting
                    if(FlagSystem.GetBoolValue(h.flagToCheck))
                    stringToRedact.Add(matches[h.index].Value);
                }
                catch(Exception)
                {
                    continue;
                }
            }
            //redact happens here
            foreach(string s in stringToRedact)
            {
              ModdedText = ModdedText.Replace(s,"▇▇▇▇");
            }
            
            //remove additional brackets if they are not hidden
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
public class HiddenWord
{
    public string flagToCheck;
    public int index;
}
