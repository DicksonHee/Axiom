using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TimeStamps))]
public class TimeStampsEditor : PropertyDrawer
{
    SerializedProperty timeStamp;
    SerializedProperty command;
    SerializedProperty muteFlagCheck;
    SerializedProperty dl;
    SerializedProperty text;
    SerializedProperty hidden;
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
            dl = property.FindPropertyRelative("dialogLine");
            text = dl.FindPropertyRelative("textToShow");
            hidden = dl.FindPropertyRelative("hiddenWords");
        //first multi
        timeStamp = property.FindPropertyRelative("timeStamp");
        command = property.FindPropertyRelative("command");
        
        //after first multi
        muteFlagCheck = property.FindPropertyRelative("muteFlag");


        GUIContent name1 = new GUIContent("Time"); 
        GUIContent name2 = new GUIContent("Cmd"); 
        GUIContent[] contents = {name1,name2};
        EditorGUI.MultiPropertyField(contentPosition, contents, timeStamp);

        contentPosition.y+= EditorGUIUtility.singleLineHeight+5;
        if(command.intValue == 2 && property.isExpanded)
        {
            DrawMuteFlag(contentPosition);
        }
        
        else if(command.intValue == 0 && property.isExpanded)
        {
            
            //draw the string of dialog line
            DrawTextProp(position, out Rect old);

            position.y+= foldOutBox.height + old.height + (EditorGUIUtility.singleLineHeight/2);
            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(position, hidden, new GUIContent("Index"));
            EditorGUI.indentLevel--;
            
        }
        else
        {
            property.isExpanded = false;
        }
        EditorGUI.EndProperty();
        
    }
    private void DrawMuteFlag(Rect position)
    {
        EditorGUIUtility.labelWidth = 40;
        float xPos = position.xMin + (position.width*0.5f);
        float yPos = position.yMin;
        float width = position.size.x/2;
        float height = EditorGUIUtility.singleLineHeight;
        Rect drawArea = new Rect(xPos, yPos, width, height);

        EditorGUI.PropertyField(drawArea, muteFlagCheck, new GUIContent("MFlag"));
    }
    private void DrawTextProp(Rect position, out Rect outRect)
    {
        float xPos = position.xMin;
        float yPos = position.yMin + EditorGUIUtility.singleLineHeight;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight*2.5f;

        Rect drawArea = new Rect(xPos, yPos, width, height);
        EditorGUI.PropertyField(drawArea, text, new GUIContent("Show"));
        outRect = drawArea;
    }
   
     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLines = 1;
        dl = property.FindPropertyRelative("dialogLine");
        hidden = dl.FindPropertyRelative("hiddenWords");

        if (property.isExpanded)
        {
            totalLines += 2;
            if(hidden.isExpanded && hidden!=null)
            {
                totalLines += hidden.CountInProperty()-1;
            }
        }
        
       return (EditorGUIUtility.singleLineHeight * (totalLines + property.CountInProperty()));
    }

}
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
        //int totalLines = 1;
        //dl = property.FindPropertyRelative("dialogLine");
        if (property.isExpanded)
        {
           // totalLines += 1; 
        }
       return (EditorGUIUtility.singleLineHeight * property.CountInProperty());
    }
    public int InOut()
    {
        return hidden.CountInProperty();
    } 
}
#endif