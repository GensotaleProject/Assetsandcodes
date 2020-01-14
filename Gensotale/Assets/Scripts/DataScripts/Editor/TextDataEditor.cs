using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.IO;

[CustomEditor(typeof(TextData), true)]
public class TextDataEditor : Editor
{
    static GameMaster.Language editingLanguage = GameMaster.Language.English;
    int lineCount;
    TextData thisData;

    public override VisualElement CreateInspectorGUI()
    {
        thisData = (TextData)target;
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        editingLanguage = (GameMaster.Language)EditorGUILayout.EnumPopup("Editing Language", editingLanguage);
        int languageInt = (int)editingLanguage;

        for (int i = 0; i < thisData.lines.Count; i++)
        {
            for (int k = 0; k < thisData.lines[i].textBasics.Count; k++)
            {
                while (thisData.lines[i].textBasics[k].startIndex.Count < System.Enum.GetNames(typeof(GameMaster.Language)).Length)
                    thisData.lines[i].textBasics[k].startIndex.Add(0);
                while (thisData.lines[i].textBasics[k].endIndex.Count < System.Enum.GetNames(typeof(GameMaster.Language)).Length)
                    thisData.lines[i].textBasics[k].endIndex.Add(0);
            }
            for (int k = 0; k < thisData.lines[i].effectList.Count; k++)
            {
                while (thisData.lines[i].effectList[k].startIndex.Count < System.Enum.GetNames(typeof(GameMaster.Language)).Length)
                    thisData.lines[i].effectList[k].startIndex.Add(0);
                while (thisData.lines[i].effectList[k].endIndex.Count < System.Enum.GetNames(typeof(GameMaster.Language)).Length)
                    thisData.lines[i].effectList[k].endIndex.Add(0);
            }
            for (int k = 0; k < thisData.lines[i].colorList.Count; k++)
            {
                while (thisData.lines[i].colorList[k].startIndex.Count < System.Enum.GetNames(typeof(GameMaster.Language)).Length)
                    thisData.lines[i].colorList[k].startIndex.Add(0);
                while (thisData.lines[i].colorList[k].endIndex.Count < System.Enum.GetNames(typeof(GameMaster.Language)).Length)
                    thisData.lines[i].colorList[k].endIndex.Add(0);
            }
        }

        if (thisData.lines.Count > 0)
        {
            if (languageInt >= thisData.lines[0].translation.Count)
            {
                int difference = languageInt - thisData.lines[0].translation.Count - 1;
                for (int i = 0; i < thisData.lines.Count; i++)
                {
                    for (int k = 0; k < difference; k++)
                    {
                        thisData.lines[i].translation.Add("");
                        for (int f = 0; f < thisData.lines[i].textBasics.Count; f++)
                        {
                            thisData.lines[i].textBasics[f].startIndex.Add(0);
                            thisData.lines[i].textBasics[f].endIndex.Add(0);
                        }
                        for (int f = 0; f < thisData.lines[i].effectList.Count; f++)
                        {
                            thisData.lines[i].effectList[f].startIndex.Add(0);
                            thisData.lines[i].effectList[f].endIndex.Add(0);
                        }
                        for (int f = 0; f < thisData.lines[i].colorList.Count; f++)
                        {
                            thisData.lines[i].colorList[f].startIndex.Add(0);
                            thisData.lines[i].colorList[f].endIndex.Add(0);
                        }
                    }
                }
            }
        }

        lineCount = thisData.lines.Count;
        lineCount = EditorGUILayout.DelayedIntField("Line Count", lineCount);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (lineCount != thisData.lines.Count)
        {
            Undo.RecordObject(thisData, "Line Count Change");
            Undo.IncrementCurrentGroup();
        }
        while (lineCount != thisData.lines.Count)
        {
            if (lineCount < thisData.lines.Count)
                thisData.lines.RemoveAt(thisData.lines.Count - 1);
            else
            {
                thisData.lines.Add(new TextData.TextLine());
                for (int i = 0; i < System.Enum.GetNames(typeof(GameMaster.Language)).Length; i++)
                {
                    thisData.lines[thisData.lines.Count - 1].translation.Add("");

                    for (int f = 0; f < thisData.lines[thisData.lines.Count - 1].textBasics.Count; f++)
                    {
                        thisData.lines[thisData.lines.Count - 1].textBasics[f].startIndex.Add(0);
                        thisData.lines[thisData.lines.Count - 1].textBasics[f].endIndex.Add(0);
                    }
                    for (int f = 0; f < thisData.lines[thisData.lines.Count - 1].effectList.Count; f++)
                    {
                        thisData.lines[thisData.lines.Count - 1].effectList[f].startIndex.Add(0);
                        thisData.lines[thisData.lines.Count - 1].effectList[f].endIndex.Add(0);
                    }
                    for (int f = 0; f < thisData.lines[thisData.lines.Count - 1].colorList.Count; f++)
                    {
                        thisData.lines[thisData.lines.Count - 1].colorList[f].startIndex.Add(0);
                        thisData.lines[thisData.lines.Count - 1].colorList[f].endIndex.Add(0);
                    }
                }
            }
        }
        for (int i = 0; i < thisData.lines.Count; i++)
        {
            thisData.lines[i].translation[languageInt] = EditorGUILayout.TextField(new GUIContent("Line " + i.ToString(), "Line index " + i.ToString() + " of the conversation"), thisData.lines[i].translation[languageInt]);
            thisData.lines[i].skipDelay = EditorGUILayout.IntField("Line " + i.ToString() + " skip delay: ", thisData.lines[i].skipDelay);
            thisData.lines[i].progressAutomatically = EditorGUILayout.Toggle("Line " + i.ToString() + " progress automatically: ", thisData.lines[i].progressAutomatically);

            EditorGUILayout.BeginFoldoutHeaderGroup(true, "Line " + i.ToString() + " Text Basics");
            for (int k = 0; k < thisData.lines[i].textBasics.Count; k++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X"))
                    thisData.lines[i].textBasics.RemoveAt(k);
                TextData.TextBasics thisBasics = thisData.lines[i].textBasics[k];
                EditorGUILayout.BeginVertical();
                thisBasics.textDelay = EditorGUILayout.IntField("Text write delay: ", thisBasics.textDelay);
                thisBasics.textSound = (AudioClip)EditorGUILayout.ObjectField(thisBasics.textSound, typeof(AudioClip), false);
                EditorGUILayout.Space();
                thisBasics.startIndex[languageInt] = EditorGUILayout.IntField("Start index: ", thisBasics.startIndex[languageInt]);
                thisBasics.endIndex[languageInt] = EditorGUILayout.IntField("End index: ", thisBasics.endIndex[languageInt]);

                if(thisData.lines[i].translation[languageInt].Length == 0)
                    EditorGUILayout.HelpBox("There is no text.", MessageType.Error);
                else if (thisBasics.endIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
                    EditorGUILayout.HelpBox("End index beyond text end.", MessageType.Error);
                else if (thisBasics.startIndex[languageInt] < 0)
                    EditorGUILayout.HelpBox("Start index less than 0.", MessageType.Error);
                else if (thisBasics.endIndex[languageInt] < thisBasics.startIndex[languageInt])
                    EditorGUILayout.HelpBox("End index less than starting index.", MessageType.Error);
                else if(thisBasics.startIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
                    EditorGUILayout.HelpBox("Start index beyond text end.", MessageType.Error);
                else
                {
                    EditorGUILayout.LabelField(thisData.lines[i].translation[languageInt].Insert(thisBasics.startIndex[languageInt], "*").Insert(thisBasics.endIndex[languageInt] + 2, "*"));
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                thisData.lines[i].textBasics[k] = thisBasics;
            }
            if (GUILayout.Button("Add TextBasics"))
            {
                thisData.lines[i].textBasics.Add(new TextData.TextBasics());
                for (int k = 0; k < System.Enum.GetNames(typeof(GameMaster.Language)).Length; k++)
                {
                    thisData.lines[i].textBasics[thisData.lines[i].textBasics.Count - 1].startIndex.Add(0);
                    thisData.lines[i].textBasics[thisData.lines[i].textBasics.Count - 1].endIndex.Add(0);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.BeginFoldoutHeaderGroup(true, "Line " + i.ToString() + " Text Effects");
            for (int k = 0; k < thisData.lines[i].effectList.Count; k++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X"))
                    thisData.lines[i].effectList.RemoveAt(k);
                TextData.TextEffects thisEffectList = thisData.lines[i].effectList[k];
                EditorGUILayout.BeginVertical();
                thisEffectList.effectType = (TextData.EffectType)GUILayout.SelectionGrid((int)thisEffectList.effectType, System.Enum.GetNames(typeof(TextData.EffectType)), 3);
                thisEffectList.distance = EditorGUILayout.FloatField("Distance : ", thisEffectList.distance);
                thisEffectList.speed = EditorGUILayout.FloatField("Speed : ", thisEffectList.speed);
                EditorGUILayout.Space();
                thisEffectList.startIndex[languageInt] = EditorGUILayout.IntField("Start index: ", thisEffectList.startIndex[languageInt]);
                thisEffectList.endIndex[languageInt] = EditorGUILayout.IntField("End index: ", thisEffectList.endIndex[languageInt]);

                if (thisData.lines[i].translation[languageInt].Length == 0)
                    EditorGUILayout.HelpBox("There is no text.", MessageType.Error);
                else if (thisEffectList.endIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
                    EditorGUILayout.HelpBox("End index beyond text end.", MessageType.Error);
                else if (thisEffectList.startIndex[languageInt] < 0)
                    EditorGUILayout.HelpBox("Start index less than 0.", MessageType.Error);
                else if (thisEffectList.endIndex[languageInt] < thisEffectList.startIndex[languageInt])
                    EditorGUILayout.HelpBox("End index less than starting index.", MessageType.Error);
                else if (thisEffectList.startIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
                    EditorGUILayout.HelpBox("Start index beyond text end.", MessageType.Error);
                else
                {
                    EditorGUILayout.LabelField(thisData.lines[i].translation[languageInt].Insert(thisEffectList.startIndex[languageInt], "*").Insert(thisEffectList.endIndex[languageInt] + 2, "*"));
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                thisData.lines[i].effectList[k] = thisEffectList;
            }
            if (GUILayout.Button("Add Effect"))
            {
                thisData.lines[i].effectList.Add(new TextData.TextEffects());
                for (int k = 0; k < System.Enum.GetNames(typeof(GameMaster.Language)).Length; k++)
                {
                    thisData.lines[i].effectList[thisData.lines[i].effectList.Count - 1].startIndex.Add(0);
                    thisData.lines[i].effectList[thisData.lines[i].effectList.Count - 1].endIndex.Add(0);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();



            EditorGUILayout.BeginFoldoutHeaderGroup(true, "Line " + i.ToString() + " Colors");
            for (int k = 0; k < thisData.lines[i].colorList.Count; k++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X"))
                    thisData.lines[i].colorList.RemoveAt(k);
                TextData.ColorEffects thisColorList = thisData.lines[i].colorList[k];
                EditorGUILayout.BeginVertical();
                thisColorList.effectType = (TextData.ColorType)GUILayout.SelectionGrid((int)thisColorList.effectType, System.Enum.GetNames(typeof(TextData.ColorType)), 4);

                EditorGUILayout.Space();
                for (int f = 0; f < thisColorList.colors.Count; f++)
                {
                    thisColorList.colors[f] = EditorGUILayout.ColorField("Color " + f.ToString(), thisColorList.colors[f]);
                }
                if (GUILayout.Button("New Color"))
                    thisColorList.colors.Add(new Color32(255, 255, 255, 255));
                EditorGUILayout.Space();

                thisColorList.speed = EditorGUILayout.FloatField("Speed : ", thisColorList.speed);

                EditorGUILayout.Space();
                thisColorList.startIndex[languageInt] = EditorGUILayout.IntField("Start index: ", thisColorList.startIndex[languageInt]);
                thisColorList.endIndex[languageInt] = EditorGUILayout.IntField("End index: ", thisColorList.endIndex[languageInt]);

                if (thisData.lines[i].translation[languageInt].Length == 0)
                    EditorGUILayout.HelpBox("There is no text.", MessageType.Error);
                else if (thisColorList.endIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
                    EditorGUILayout.HelpBox("End index beyond text end.", MessageType.Error);
                else if (thisColorList.startIndex[languageInt] < 0)
                    EditorGUILayout.HelpBox("Start index less than 0.", MessageType.Error);
                else if (thisColorList.endIndex[languageInt] < thisColorList.startIndex[languageInt])
                    EditorGUILayout.HelpBox("End index less than starting index.", MessageType.Error);
                else if (thisColorList.startIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
                    EditorGUILayout.HelpBox("Start index beyond text end.", MessageType.Error);
                else
                {
                    EditorGUILayout.LabelField(thisData.lines[i].translation[languageInt].Insert(thisColorList.startIndex[languageInt], "*").Insert(thisColorList.endIndex[languageInt] + 2, "*"));
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();
                thisData.lines[i].colorList[k] = thisColorList;
            }
            if (GUILayout.Button("Add Color Effect"))
            {
                thisData.lines[i].colorList.Add(new TextData.ColorEffects());
                for (int k = 0; k < System.Enum.GetNames(typeof(GameMaster.Language)).Length; k++)
                {
                    thisData.lines[i].colorList[thisData.lines[i].colorList.Count - 1].startIndex.Add(0);
                    thisData.lines[i].colorList[thisData.lines[i].colorList.Count - 1].endIndex.Add(0);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }




        if (GUILayout.Button("Add new line"))
        {
            thisData.lines.Add(new TextData.TextLine());
            for (int i = 0; i < System.Enum.GetNames(typeof(GameMaster.Language)).Length; i++)
            {
                thisData.lines[thisData.lines.Count - 1].translation.Add("");
                for (int f = 0; f < thisData.lines[i].textBasics.Count; f++)
                {
                    thisData.lines[i].textBasics[f].startIndex.Add(0);
                    thisData.lines[i].textBasics[f].endIndex.Add(0);
                }
                for (int f = 0; f < thisData.lines[i].effectList.Count; f++)
                {
                    thisData.lines[i].effectList[f].startIndex.Add(0);
                    thisData.lines[i].effectList[f].endIndex.Add(0);
                }
                for (int f = 0; f < thisData.lines[i].colorList.Count; f++)
                {
                    thisData.lines[i].colorList[f].startIndex.Add(0);
                    thisData.lines[i].colorList[f].endIndex.Add(0);
                }
            }
        }
    }
}
