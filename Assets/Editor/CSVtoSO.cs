using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CSVtoSO
{
    //private 
    /////////////
    
    ////////////
    private static string DialogListCSVPath = "/Editor/CSVs/DialogListCSV.csv";

    [MenuItem("Utilities/Generate Dialog List")]
    public static void GenerateDialogList()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + DialogListCSVPath);
        string PreSceneName = null;
        DialogListData dialogListInstance = null;
        string PrevFileName = null;
        Dialog dialog = null;
        //bool canCreate = false;
        //assigning
        for(int y = 1; y < allLines.Length; y++)//x = 1, ignore first row as it is names
        {                                       // for each row
            //split the string
            string[] splitData = allLines[y].Split(',');

            //Assign collum A as previous scene name
            if(y!=1)
            {
                string[] PrevData = allLines[y-1].Split(',');
                PreSceneName = PrevData[0]; //get previous scence name
                PrevFileName = PrevData[1];
            }
            
            if(PreSceneName != splitData[0])//Create New DialogListSO based of SceneName (each row)
            {                               //Ignore if PreSceneName is equals to next in line
                dialogListInstance = ScriptableObject.CreateInstance<DialogListData>();
                dialogListInstance.dialogLists = new List<Dialog>();

                //Assign collum A as dialoglist name
                dialogListInstance.name = splitData[0];
                //AssetDatabase.Refresh();
                //AssetDatabase.SaveAssets();
            }
            
            DialogLine dlInstance = new DialogLine(); //new dialog line in each row
            dlInstance.hiddenWords = new List<HiddenWord>();
            TimeStamp ts = new TimeStamp(); //start create new timestamp
            //List<HiddenWord> tempWords = new List<HiddenWord>();

            //if(PrevFileName != splitData[1])
            
            //
            for(int x = 1; x < 4; x++) // x = 1, ignore first collum
            {                                         
                //if(PrevFileName != splitData[1])

                if(x==1 && PrevFileName != splitData[1])//start create dialog
                {
                    dialog = new Dialog();

                    dialog.audioFileName = splitData[1]; //file name
                    PrevFileName = splitData[1];

                    dialogListInstance.dialogLists.Add(dialog);
                    //AssetDatabase.Refresh();
                    //AssetDatabase.SaveAssets();
                }
                if(x==2)
                {
                    //TimeStamp ts = new TimeStamp(); //start create new timestamp
                    ts.timeStamp = float.Parse(splitData[2]); // decide timestamp time

                    switch(splitData[3]) // decide timestamp command
                    {
                        case "Show":
                        {
                            ts.command = TimeStamp.Commands.ShowText;
                            dlInstance.textToShow = splitData[4]; //only change this if show text
                            
                            int TotalCount = (splitData.Length-6)/2;

                            for(int i = 1; i < TotalCount; i++)
                            {
                                //create new hiddenword once every second time
                                HiddenWord hw = new HiddenWord();
                                Debug.Log(y +"//"+splitData[(i*2)-1+6]+"//"+ splitData[1]);
                                try
                                {
                                    hw.index = int.Parse(splitData[(i*2)-1+6]);//add hidden word indexes
                                }
                                catch
                                {
                                    Debug.Log("skip, no index found");
                                    continue;
                                }
                                hw.flagToCheck = splitData[(i*2)+6]; //set text flag to check 
                                dlInstance.hiddenWords.Add(hw);
                                foreach(HiddenWord h in dlInstance.hiddenWords)
                                {
                                    Debug.Log(h.index +"//dude");
                                }
                            }
                            
                        }
                        
                        break;

                        case "Next":
                        ts.command = TimeStamp.Commands.NextDialogLine;
                        break;

                        case "Mute":
                        ts.command = TimeStamp.Commands.Mute;
                        ts.muteFlag = splitData[5];
                        break;

                        case "Unmute":
                        ts.command = TimeStamp.Commands.Unmute;
                        break;
                    }
                    if(dialog == null)
                    Debug.Log("dialog is null");

                    dialog.timestamps = new List<TimeStamp>();
                    dialog.timestamps.Add(ts);//add ts to list
                    //AssetDatabase.SaveAssets();
                }
                
                    //AssetDatabase.SaveAssets();
                   
            }//end collum loop
                try
                {
                    if(PreSceneName!= splitData[0])
                    AssetDatabase.CreateAsset(dialogListInstance, $"Assets/DialogListFolder/{dialogListInstance.name}.asset");
                }
                catch(UnityException e)
                {
                    Debug.Log(e);
                    continue;
                }
        }//end row loop
       AssetDatabase.SaveAssets();
    }   
}
public class DataContainer
{
    public string[,] matrix;

}
//number of dialogList is the number of containers in A collum
//dialog in dialogList is the number of collums(ignoring first collum) in the row
