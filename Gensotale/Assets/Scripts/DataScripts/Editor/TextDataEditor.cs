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
    int characterCount;
    TextData thisData;
    List<bool> foldouts = new List<bool>();

    bool displayCharLineup;

    bool insertNewMenu;
    bool swapLines;
    int insertIndex;
    int swapIndex;

    public override VisualElement CreateInspectorGUI()
    {
        thisData = (TextData)target;
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        while(foldouts.Count < thisData.lines.Count * 3)
        {
            foldouts.Add(false);
        }
        while (foldouts.Count > thisData.lines.Count * 3)
        {
            foldouts.RemoveAt(foldouts.Count - 1);
        }

        EditorGUI.BeginChangeCheck();
        editingLanguage = (GameMaster.Language)EditorGUILayout.EnumPopup("Editing Language", editingLanguage);
        int languageInt = (int)editingLanguage;


        characterCount = thisData.chars.Count;
        characterCount = EditorGUILayout.DelayedIntField("Character Count", characterCount);

        if(characterCount != thisData.chars.Count)
        {
            while (thisData.chars.Count < characterCount)
                thisData.chars.Add(new DialogueCharData());
            while (thisData.chars.Count > characterCount)
                thisData.chars.RemoveAt(thisData.chars.Count - 1);
        }

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

        displayCharLineup = EditorGUILayout.BeginFoldoutHeaderGroup(displayCharLineup, "Character Lineup");

        if (displayCharLineup)
        {
            for (int i = 0; i < characterCount; i++)
            {
                GUILayout.Space(40);
                thisData.chars[i] = (DialogueCharData)EditorGUILayout.ObjectField("Character " + i.ToString() + ":", thisData.chars[i], typeof(DialogueCharData), false);
                if (thisData.chars[i] == null)
                    continue;
                if (thisData.chars[i].portraits.Count > 0 && thisData.chars[i].portraits[0] != null)
                    EditorGUI.DrawPreviewTexture(new Rect(new Vector2(165, 128 + (128 * i)), thisData.chars[i].portraits[0].bounds.size.normalized * 128), thisData.chars[i].portraits[0].texture);
                GUILayout.Space(thisData.chars[i].portraits[0].bounds.size.normalized.y * 64);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        for (int i = 0; i < thisData.lines.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(32)))
            {
                thisData.lines.RemoveAt(i);
                i--;
                lineCount--;
            }
            thisData.lines[i].translation[languageInt] = EditorGUILayout.TextArea(thisData.lines[i].translation[languageInt]);
            EditorGUILayout.EndHorizontal();

            thisData.lines[i].skipDelay = EditorGUILayout.IntField("Line " + i.ToString() + " skip delay: ", thisData.lines[i].skipDelay);
            thisData.lines[i].progressAutomatically = EditorGUILayout.Toggle("Line " + i.ToString() + " progress automatically: ", thisData.lines[i].progressAutomatically);
            thisData.lines[i].addStar = EditorGUILayout.Toggle("Add star to line " + i.ToString() + ": ", thisData.lines[i].addStar);


            foldouts[i * 3] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[i * 3], "Line " + i.ToString() + " Text Basics");
            if (foldouts[i * 3])
            {
                for (int k = 0; k < thisData.lines[i].textBasics.Count; k++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X"))
                    {
                        thisData.lines[i].textBasics.RemoveAt(k);
                        k--;
                    }
                    TextData.TextBasics thisBasics = thisData.lines[i].textBasics[k];
                    EditorGUILayout.BeginVertical();
                    thisBasics.textDelay = EditorGUILayout.IntField("Text write delay: ", thisBasics.textDelay);
                    thisBasics.textSound = (AudioClip)EditorGUILayout.ObjectField(thisBasics.textSound, typeof(AudioClip), false);
                    EditorGUILayout.Space();
                    thisBasics.startIndex[languageInt] = EditorGUILayout.IntField("Start index: ", thisBasics.startIndex[languageInt]);
                    thisBasics.endIndex[languageInt] = EditorGUILayout.IntField("End index: ", thisBasics.endIndex[languageInt]);

                    if (thisData.lines[i].translation[languageInt].Length == 0)
                        EditorGUILayout.HelpBox("There is no text.", MessageType.Error);
                    else if (thisBasics.endIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
                        EditorGUILayout.HelpBox("End index beyond text end.", MessageType.Error);
                    else if (thisBasics.startIndex[languageInt] < 0)
                        EditorGUILayout.HelpBox("Start index less than 0.", MessageType.Error);
                    else if (thisBasics.endIndex[languageInt] < thisBasics.startIndex[languageInt])
                        EditorGUILayout.HelpBox("End index less than starting index.", MessageType.Error);
                    else if (thisBasics.startIndex[languageInt] >= thisData.lines[i].translation[languageInt].Length)
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
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            foldouts[i * 3 + 1] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[i * 3 + 1], "Line " + i.ToString() + " Text Effects");
            if (foldouts[i * 3 + 1])
            {
                for (int k = 0; k < thisData.lines[i].effectList.Count; k++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X"))
                    {
                        thisData.lines[i].effectList.RemoveAt(k);
                        k--;
                    }
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
            }
            EditorGUILayout.EndFoldoutHeaderGroup();



            foldouts[i * 3 + 2] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts[i * 3 + 2], "Line " + i.ToString() + " Colors");
            if (foldouts[i * 3 + 2])
            {
                for (int k = 0; k < thisData.lines[i].colorList.Count; k++)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("X"))
                    {
                        thisData.lines[i].colorList.RemoveAt(k);
                        k--;
                    }
                    TextData.ColorEffects thisColorList = thisData.lines[i].colorList[k];
                    EditorGUILayout.BeginVertical();
                    thisColorList.effectType = (TextData.ColorType)GUILayout.SelectionGrid((int)thisColorList.effectType, System.Enum.GetNames(typeof(TextData.ColorType)), 4);


                    if (thisColorList.colors.Count < 1)
                        thisColorList.colors.Add(new Color32(255, 255, 255, 255));

                    if (thisColorList.effectType == TextData.ColorType.Color)
                    {
                        while (thisColorList.colors.Count > 4)
                        {
                            thisColorList.colors.RemoveAt(thisColorList.colors.Count - 1);
                        }
                    }
                    else if (thisColorList.effectType == TextData.ColorType.VertWaveLeft ||
                        thisColorList.effectType == TextData.ColorType.VertWaveRight ||
                        thisColorList.effectType == TextData.ColorType.StripeDiagonalLeft || thisColorList.effectType == TextData.ColorType.StripeDiagonalRight)
                    {
                        while (thisColorList.colors.Count < 3)
                        {
                            thisColorList.colors.Add(new Color32(255, 255, 255, 255));
                        }
                    }

                    EditorGUILayout.Space();
                    for (int f = 0; f < thisColorList.colors.Count; f++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        thisColorList.colors[f] = EditorGUILayout.ColorField("Color " + f.ToString(), thisColorList.colors[f]);
                        if (thisColorList.colors.Count > 1)
                            if (GUILayout.Button("X", GUILayout.Width(32)))
                                thisColorList.colors.RemoveAt(f);
                        EditorGUILayout.EndHorizontal();
                    }
                    if (!(thisColorList.effectType == TextData.ColorType.Color && thisColorList.colors.Count > 3))
                        if (GUILayout.Button("New Color"))
                            thisColorList.colors.Add(new Color32(255, 255, 255, 255));
                    EditorGUILayout.Space();

                    thisColorList.time = EditorGUILayout.FloatField("Time : ", thisColorList.time);

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
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            string[] charOptions = new string[thisData.chars.Count + 1];
            for (int k = 0; k < charOptions.Length; k++)
            {
                if (k == 0)
                    charOptions[k] = "None";
                else
                    charOptions[k] = thisData.chars[k - 1].name;
            }

            EditorGUILayout.LabelField("Display Character:");
            thisData.lines[i].charIndex = GUILayout.SelectionGrid(thisData.lines[i].charIndex, charOptions, 4);

            if(thisData.lines[i].charIndex > 0)
            {
                int realCharIndex = thisData.lines[i].charIndex - 1;
                if(GUILayout.Button("Set all line text sounds to character voice"))
                    for (int k = 0; k < thisData.lines[i].textBasics.Count; k++)
                    {
                        thisData.lines[i].textBasics[k].textSound = thisData.chars[realCharIndex].speakSound;
                    }

                EditorGUILayout.LabelField("Char Portrait:");
                Rect prevRect = GUILayoutUtility.GetLastRect();

                thisData.lines[i].portraitIndex = Mathf.Clamp(thisData.lines[i].portraitIndex, 0, thisData.chars[realCharIndex].portraits.Count - 1);

                if (GUI.Button(new Rect(new Vector2(EditorGUIUtility.currentViewWidth / 2
                    - thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].bounds.size.normalized.x * 128 - 32, prevRect.position.y + 32),
                    new Vector2(64, thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].bounds.size.normalized.y * 128)),"<"))
                    thisData.lines[i].portraitIndex = Mathf.Clamp(thisData.lines[i].portraitIndex - 1, 0, thisData.chars[realCharIndex].portraits.Count - 1);

                EditorGUI.DrawPreviewTexture(new Rect(new Vector2(EditorGUIUtility.currentViewWidth / 2
                    - thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].bounds.size.normalized.x * 64, prevRect.position.y + 32), 
                    thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].bounds.size.normalized * 128), 
                    thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].texture);

                if (GUI.Button(new Rect(new Vector2(EditorGUIUtility.currentViewWidth / 2
                    + thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].bounds.size.normalized.x * 128 - 32, prevRect.position.y + 32), 
                    new Vector2(64, thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].bounds.size.normalized.y * 128)),">"))
                    thisData.lines[i].portraitIndex = Mathf.Clamp(thisData.lines[i].portraitIndex + 1, 0, thisData.chars[realCharIndex].portraits.Count - 1);

                EditorGUILayout.Space(thisData.chars[realCharIndex].portraits[thisData.lines[i].portraitIndex].bounds.size.normalized.y * 128);
            }

            /*if (displayCharacterData[i])
            {
                TextData.DialogueLineCharData currentData;
                for (int k = 0; k < thisData.chars.Count; k++)
                {
                    currentData = thisData.lines[i].dialogueLineCharDatas[k];
                    currentData.characterOnscreen = EditorGUILayout.Toggle(thisData.chars[k].charName[0] + " onscreen", currentData.characterOnscreen);
                    currentData.characterTalking = EditorGUILayout.Toggle(thisData.chars[k].charName[0] + " talking", currentData.characterTalking);
                    currentData.expressionIndex = Mathf.Clamp(EditorGUILayout.DelayedIntField(thisData.chars[k].charName[0] + " expression index: ", currentData.expressionIndex),
                        0, thisData.chars[k].portraits.Length - 1);
                    currentData.leftSide = EditorGUILayout.Toggle(thisData.chars[k].charName[0] + " on left side", currentData.leftSide);
                    currentData.faceBackwards = EditorGUILayout.Toggle(thisData.chars[k].charName[0] + " face backwards", currentData.faceBackwards);
                    currentData.displayName = EditorGUILayout.Toggle(thisData.chars[k].charName[0] + " display name?", currentData.displayName);
                    currentData.customPosition = EditorGUILayout.Toggle(thisData.chars[k].charName[0] + " custom position?", currentData.customPosition);
                    if (currentData.customPosition)
                    {
                        if (currentData.customPositionVectors.sqrMagnitude == 0)
                            currentData.customPositionVectors = thisData.charDefaultPositions[k];

                        currentData.customPositionVectors = EditorGUILayout.Vector2Field(thisData.chars[k].charName[0] + " custom position value: ", currentData.customPositionVectors);
                    }
                    thisData.lines[i].dialogueLineCharDatas[k] = currentData;
                    EditorGUILayout.Space();
                }
            }*/

            GUILayout.Space(32);
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

        GUILayout.Space(16);

        if (GUILayout.Button(insertNewMenu ? "C E A S E" : "Insert new line"))
        {
            insertNewMenu = !insertNewMenu;
            insertIndex = 0;
        }
        if(insertNewMenu)
        {
            swapLines = false;
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Insert~"))
            {
                lineCount++;
                thisData.lines.Insert(insertIndex, new TextData.TextLine());
                insertNewMenu = false;
            }
            insertIndex = EditorGUILayout.IntField("Index", Mathf.Clamp(insertIndex, 0, thisData.lines.Count - 1));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(16);
        }
        
        GUILayout.Space(16);

        if(GUILayout.Button(swapLines ? "D E S I S T" : "Swap lines"))
        {
            swapLines = !swapLines;
            insertIndex = 0;
            swapIndex = 0;
        }
        if(swapLines)
        {
            insertNewMenu = false;
            EditorGUILayout.BeginHorizontal();
            swapIndex = EditorGUILayout.IntField("From Index", Mathf.Clamp(swapIndex, 0, thisData.lines.Count - 1));
            insertIndex = EditorGUILayout.IntField("To Index", Mathf.Clamp(insertIndex, 0, thisData.lines.Count - 1));
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Switcheroo", GUILayout.Width(256)))
            {
                TextData.TextLine tempLine = thisData.lines[insertIndex];

                thisData.lines[insertIndex] = thisData.lines[swapIndex];
                thisData.lines[swapIndex] = tempLine;
                swapLines = false;
            }
            GUILayout.Space(16);
        }
        GUILayout.Space(16);

        if (GUILayout.Button("Overwrite from txt"))
        {
            Undo.RecordObject(thisData, "Import Text File");
            Undo.IncrementCurrentGroup();
            FileReader(EditorUtility.OpenFilePanel("Import text file", Application.dataPath, "txt"), false);
        }
        GUILayout.Space(16);
        if (GUILayout.Button("Import text only"))
        {
            Undo.RecordObject(thisData, "Import Text File");
            Undo.IncrementCurrentGroup();
            FileReader(EditorUtility.OpenFilePanel("Import text file", Application.dataPath, "txt"), true);
        }


        if (GUI.changed)
        {
            EditorUtility.SetDirty(thisData);
        }
    }


    void FileReader(string filePath, bool importTextOnly)
    {
        if (filePath == "")
            return;
        FileStream stream = File.OpenRead(filePath);
        string fileString = "";
        int index = 0;
        byte[] bytes = new byte[stream.Length];
        while (index < stream.Length)
        {
            bytes[index] = (byte)stream.ReadByte();
            index++;
        }

        fileString = System.Text.Encoding.Unicode.GetString(bytes);

        string[] separatedString = fileString.Split('\n');

        if (importTextOnly)
        {
            for (int i = 0; i < separatedString.Length; i++)
            {
                thisData.lines[i].translation[(int)editingLanguage] = separatedString[i];
            }
        }
        else
        {
            thisData.lines.Clear();
            for (int i = 0; i < separatedString.Length; i++)
            {
                thisData.lines.Add(new TextData.TextLine());
                for (int k = 0; k < System.Enum.GetNames(typeof(GameMaster.Language)).Length; k++)
                {
                    thisData.lines[i].translation.Add("");
                }
                if (separatedString[i].Substring(0, 2) == "* ")
                {
                    thisData.lines[i].addStar = true;
                    thisData.lines[i].translation[(int)editingLanguage] = separatedString[i].Remove(0,2);
                }
                else
                    thisData.lines[i].translation[(int)editingLanguage] = separatedString[i];
            }
        }
    }
}
