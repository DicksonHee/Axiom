using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CSVtoSO
{
    private static string DialogListCSVPath = "/Editor/CSVs/DialogListCSV.csv";

    [MenuItem("Utilities/Generate Dialog Lists")]
    public static void GenerateDialogList()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + DialogListCSVPath);//each row
        string PrevSceneName = null;
        DialogListData dialogListInstance = null;
        string PrevFileName = null;
        Dialog dialog = null;
        
        //assigning
        for(int y = 1; y < allLines.Length; y++)//x = 1, ignore first row as it is names
        {                                       // for each row
            //split the string
            string[] splitData = allLines[y].Split(';');

            //Assign collum A as previous scene name
            if(y!=1)
            {
                string[] PrevData = allLines[y-1].Split(';');
                PrevSceneName = PrevData[0]; //get previous scence name
                PrevFileName = PrevData[1];
            }
            
            if(PrevSceneName != splitData[0] && splitData[0] != string.Empty)//Create New DialogListSO based of SceneName (each row)
            {                                                               //Ignore if PrevSceneName is equals to next in line
                dialogListInstance = ScriptableObject.CreateInstance<DialogListData>();
                dialogListInstance.dialogLists = new List<Dialog>();
                

                //Assign collum A as dialoglist name
                dialogListInstance.name = splitData[0];

                Debug.Log("created new asset and list dialog ||"+ dialogListInstance.name);
            }

            if(PrevFileName != splitData[1] && splitData[1] != string.Empty || y == 1  //create new dialog checks
            ||PrevSceneName != splitData[0] && splitData[0] != string.Empty) //if different file name / in different scene create dialog
            
            {
                dialog = new Dialog();
                dialog.timestamps = new List<TimeStamp>();

                dialog.audioFileName = splitData[1]; //file name
                PrevFileName = splitData[1];

                dialogListInstance.dialogLists.Add(dialog);
                Debug.Log("PrevFileName is different"+"|| prev: "+PrevFileName);
            }

            
            if(splitData[2] == string.Empty) continue; // no timestamp means skip to next row

                TimeStamp ts = new TimeStamp(); //start create new timestamp
                
                if(splitData[2] != string.Empty)//if the row is not empty
                {
                    try
                    {
                        ts.timeStamp = float.Parse(splitData[2]); // decide timestamp time
                    }
                    catch(System.Exception e)
                    {
                        Debug.Log(e +"||"+splitData[2]);
                    }
                }
                    
                    DialogLine dlInstance = new DialogLine(); //new dialog line in each row
                    dlInstance.hiddenWords = new List<HiddenWord>();

                    switch(splitData[3]) // decide timestamp command
                    {
                        case "Show":
                        ts.command = TimeStamp.Commands.ShowText;
                        dlInstance.textToShow = splitData[4]; //only change this if show text
                        
                        
                        int TotalCount = (splitData.Length-6)/2;
                        for(int i = 1; i < TotalCount; i++) // for how many hiddenword needed to be added
                        {
                            //create new hiddenword once every second time
                            HiddenWord hw = new HiddenWord();
                            Debug.Log("row"+ y +"//index "+ splitData[(i*2)+5] +"//"+ splitData[(i*2)+4]+"//command: "+splitData[3]);
                            try
                            {
                                hw.index = int.Parse(splitData[(i*2)+5]);//add hidden word indexes
                            }
                            catch
                            {
                                Debug.Log("skip, no index found");
                                continue;
                            }
                            hw.flagToCheck = splitData[(i*2)+4]; //set text flag to check 
                            dlInstance.hiddenWords.Add(hw);
                        }
                        break;

                        case "Mute":
                        ts.command = TimeStamp.Commands.Mute;
                        ts.muteFlag = splitData[5];
                        break;

                        case "Unmute":
                        ts.command = TimeStamp.Commands.Unmute;
                        break;

                        case "Stop":
                        ts.command = TimeStamp.Commands.Stop;
                        break;

                        case"Event":
                        ts.command = TimeStamp.Commands.Event;
                        ts.eventName = splitData[5];
                        break;
                    }
                    if(dialog == null)
                    Debug.Log("dialog is null");
                    
                    if(dialog.timestamps == null)
                    {
                        dialog.timestamps = new List<TimeStamp>();
                        Debug.Log("created new list, dialog.timestamps was null");
                    }
                    
                    ts.dialogLine = dlInstance;//set dialogline
                    dialog.timestamps.Add(ts);//add ts to list
                try
                {
                    if(PrevSceneName!= splitData[0])//if name is not equals to prevname, crate asset
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
//number of dialogList is the number of containers in A collum
//dialog in dialogList is the number of collums(ignoring first collum) in the row
