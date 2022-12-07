using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TimeStamp))]
public class TimeStampsEditor : PropertyDrawer
{
    SerializedProperty P_timeStamp;
    SerializedProperty P_command;
    SerializedProperty P_muteFlagCheck;
    SerializedProperty P_dl;
    SerializedProperty P_text;
    SerializedProperty P_hidden;
    SerializedProperty P_eventName;
    //SerializedProperty muteFlag;
    
    //end
    Rect foldOutBox;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //determine where to draw box
        foldOutBox = new Rect(position.xMin, position.yMin, position.size.x, EditorGUIUtility.singleLineHeight);
        Rect contentPosition = EditorGUI.PrefixLabel(foldOutBox, label);

        //get the isExpanded bool
        property.isExpanded = EditorGUI.Foldout(foldOutBox, property.isExpanded, label);
       
        //find
        //find
            P_dl = property.FindPropertyRelative("dialogLine");
            P_text = P_dl.FindPropertyRelative("textToShow");
            P_hidden = P_dl.FindPropertyRelative("hiddenWords");

        //first multi || timestamp[...] command [...v]
        P_timeStamp = property.FindPropertyRelative("timeStamp");
        P_command = property.FindPropertyRelative("command");
        
        //after first multi drawn field, underneath the timestamp, command property
        P_muteFlagCheck = property.FindPropertyRelative("muteFlag");
        P_eventName = property.FindPropertyRelative("eventName");

        GUIContent name1 = new GUIContent("Time"); 
        GUIContent name2 = new GUIContent("Cmd"); 
        GUIContent[] contents = {name1,name2};                                 //determines how much properties to draw, maximum 4 properties
        EditorGUI.MultiPropertyField(contentPosition, contents, P_timeStamp); //draw P_timeStamp and the next property found, this case is P_command

        contentPosition.y+= EditorGUIUtility.singleLineHeight+5;

        if(property.isExpanded)
        {
            switch(P_command.intValue)
            {
                case 0: //show text
                    DrawTextProp(position, out Rect old); //draw the string of dialog line

                    position.y+= foldOutBox.height + old.height + (EditorGUIUtility.singleLineHeight/2);
                    EditorGUI.indentLevel++;
                    EditorGUI.PropertyField(position, P_hidden, new GUIContent("Index"));
                    EditorGUI.indentLevel--;
                break;

                case 1: //mute
                    DrawMuteFlagProp(contentPosition);
                break;

                case 2: //unmute
                    
                break;

                case 3: //stop

                break;

                case 4: //Event
                    DrawEventProp(contentPosition);
                break;
            }
        }

        else
        {
            property.isExpanded = false;
        }
        EditorGUI.EndProperty();
        
    }
    private void DrawMuteFlagProp(Rect position) //mute flag
    {
        EditorGUIUtility.labelWidth = 40;
        float xPos = position.xMin + (position.width*0.5f);
        float yPos = position.yMin;
        float width = position.size.x/2;
        float height = EditorGUIUtility.singleLineHeight;
        Rect drawArea = new Rect(xPos, yPos, width, height);

        EditorGUI.PropertyField(drawArea, P_muteFlagCheck, new GUIContent("MFlag"));
    }
    private void DrawEventProp(Rect position) //event property field
    {
        EditorGUIUtility.labelWidth = 35;
        float xPos = position.xMin + (position.width*0.5f);
        float yPos = position.yMin;
        float width = position.size.x/2;
        float height = EditorGUIUtility.singleLineHeight;
        Rect drawArea = new Rect(xPos, yPos, width, height);

        EditorGUI.PropertyField(drawArea, P_eventName, new GUIContent("Event"));
    }
    private void DrawTextProp(Rect position, out Rect outRect)
    {
        float xPos = position.xMin;
        float yPos = position.yMin + EditorGUIUtility.singleLineHeight;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight*2.5f;

        Rect drawArea = new Rect(xPos, yPos, width, height);
        EditorGUI.PropertyField(drawArea, P_text, new GUIContent("Show"));
        outRect = drawArea;
    }
    
     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLines = 1;
        P_dl = property.FindPropertyRelative("dialogLine");
        P_hidden = P_dl.FindPropertyRelative("hiddenWords");

        if (property.isExpanded)
        {
            totalLines += 2;
            if(P_hidden.isExpanded && P_hidden!=null)
            {
                totalLines += P_hidden.CountInProperty()-1;
            }
        }

        return (EditorGUIUtility.singleLineHeight * (totalLines + property.CountInProperty()));
    }

}
//-----------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------
[CustomPropertyDrawer(typeof(HiddenWord))]
public class HiddenEditor : PropertyDrawer
{
    //hidden stuff
    SerializedProperty index;
    SerializedProperty hidden;
    SerializedProperty flag;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //determine where to draw box
        Rect foldOutBox = new Rect(position.xMin, position.yMin, position.size.x, EditorGUIUtility.singleLineHeight);
        Rect contentPosition = EditorGUI.PrefixLabel(foldOutBox, label);

        //get the isExpanded bool
        property.isExpanded = EditorGUI.Foldout(foldOutBox, property.isExpanded, label);

        //second multi
        //hidden = property.FindPropertyRelative("hiddenWords");
        index = property.FindPropertyRelative("index"); 
        flag = property.FindPropertyRelative("flagToCheck");

        GUIContent name3 = new GUIContent("index");
        GUIContent name4 = new GUIContent("Flag");

        GUIContent[] contents2 = {name3,name4};

        if(property.isExpanded)
        {
            //EditorGUI.MultiPropertyField(foldOutBox, contents2, index);
            DrawIndex(position);
            DrawFlag(position);

        }
        EditorGUI.EndProperty();
    }
    private void DrawIndex(Rect position)
    {
        EditorGUIUtility.labelWidth = 40;
        float xPos = position.xMin;
        float yPos = position.yMin + EditorGUIUtility.singleLineHeight;
        float width = position.size.x*0.4f;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(xPos, yPos, width, height);

        EditorGUI.PropertyField(drawArea, index, new GUIContent("index"));
    }
     private void DrawFlag(Rect position)
    {
        EditorGUIUtility.labelWidth = 30;
        float xPos = position.xMin + (position.width/2);
        float yPos = position.yMin + EditorGUIUtility.singleLineHeight;
        float width = position.size.x/2;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(xPos, yPos, width, height);
        EditorGUI.PropertyField(drawArea, flag, new GUIContent("flag"));
    }
     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        
       return (EditorGUIUtility.singleLineHeight * property.CountInProperty());
    }
    public int InOut()
    {
        return hidden.CountInProperty();
    } 
}
#endif