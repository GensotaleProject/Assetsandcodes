using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Reflection;

[CustomEditor(typeof(BaseInteractable), true)]
public class BasicInteractableEditor : Editor
{
    int shownInteraction = 0;
    BaseInteractable thisData;
    List<bool> showTriggers = new List<bool>();
    List<bool> showVarNames = new List<bool>();
    List<int> varSelection = new List<int>();
    List<Vector2> varScroll = new List<Vector2>();

    bool insertSelection;
    int insertIndex;

    bool deleteMenu;
    bool deserialized;

    int prevShown = -1;

    public override VisualElement CreateInspectorGUI()
    {
        thisData = (BaseInteractable)target;
        return base.CreateInspectorGUI();
    }

    public override void OnInspectorGUI()
    {
        thisData.interactOffset = EditorGUILayout.Vector2Field("Interact Position Offset", thisData.interactOffset);
        thisData.interactDistance = EditorGUILayout.FloatField("Interact Area Size", thisData.interactDistance);
        thisData.loopIndex = EditorGUILayout.Toggle("Loop Interact Index", thisData.loopIndex);

        GUILayout.Space(32);

        if (!deserialized)
        {
            deserialized = true;
            //thisData.Deserialize();
        }
        else if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            deserialized = false;
            EditorApplication.Beep();
        }

        if (thisData.interactionEffects.Count > 0)
        {
            if (!insertSelection && GUILayout.Button("Insert New Interaction"))
            {
                insertSelection = true;
                insertIndex = shownInteraction;
            }
            else if (insertSelection)
            {
                EditorGUILayout.BeginHorizontal();
                insertIndex = EditorGUILayout.IntField(new GUIContent("Index", "Start counting from 0"), Mathf.Clamp(insertIndex, 0, thisData.interactionEffects.Count - 1));
                if (GUILayout.Button("Insert"))
                {
                    thisData.interactionEffects.Insert(insertIndex, new BaseInteractable.InteractionEffects());
                    insertSelection = false;
                    shownInteraction = insertIndex;
                }
                else if (GUILayout.Button("Frick no"))
                    insertSelection = false;
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(16);
            if (!deleteMenu && GUILayout.Button("Delete shown interaction"))
                deleteMenu = true;
            else if (deleteMenu)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Yeetus deletus"))
                {
                    thisData.interactionEffects.RemoveAt(shownInteraction);
                    shownInteraction = Mathf.Clamp(shownInteraction, 0, thisData.interactionEffects.Count - 1);
                }
                else if (GUILayout.Button("Wait, pls no"))
                {
                    deleteMenu = false;
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(32);
        }

        EditorGUILayout.LabelField("Interaction Selection:");
        string[] interactionSelections = new string[thisData.interactionEffects.Count + 1];
        for (int i = 0; i < interactionSelections.Length; i++)
        {
            if (i == thisData.interactionEffects.Count)
                interactionSelections[i] = "Add";
            else
                interactionSelections[i] = i.ToString();
        }
        shownInteraction = GUILayout.SelectionGrid(shownInteraction, interactionSelections, 6);

        if(shownInteraction != prevShown)
        {
            prevShown = shownInteraction;
            showTriggers.Clear();
            showVarNames.Clear();
            varScroll.Clear();
            varSelection.Clear();
            for (int i = 0; i < thisData.interactionEffects[shownInteraction].varChanges.Count; i++)
            {
                showTriggers.Add(false);
                showVarNames.Add(false);
                varScroll.Add(Vector2.zero);
                varSelection.Add(GetVarInt(thisData.interactionEffects[shownInteraction].varChanges[i]));
            }
        }

        if (shownInteraction >= thisData.interactionEffects.Count)
            thisData.interactionEffects.Add(new BaseInteractable.InteractionEffects());

        for (int i = 0; i < thisData.interactionEffects[shownInteraction].varChanges.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("X", GUILayout.Width(64)))
            {
                thisData.interactionEffects[shownInteraction].varChanges.RemoveAt(i);
                i -= 1;
            }

            if (!showTriggers[i] && GUILayout.Button("Show Trigger " + i.ToString()))
                showTriggers[i] = true;
            else if(showTriggers[i] && GUILayout.Button("Hide Trigger " + i.ToString()))
                showTriggers[i] = false;
            EditorGUILayout.EndHorizontal();
            if (!showTriggers[i])
                continue;

            BaseInteractable.VarChanges thisTrigger = thisData.interactionEffects[shownInteraction].varChanges[i];

            EditorGUILayout.BeginHorizontal();
            if (!showVarNames[i] && GUILayout.Button(thisTrigger.trigger == BaseInteractable.TriggerChange.Function ? "Show Functions" : "Show Var Selections"))
                showVarNames[i] = true;
            else if (showVarNames[i] && GUILayout.Button(thisTrigger.trigger == BaseInteractable.TriggerChange.Function ? "Hide Functions" : "Hide Vars"))
                showVarNames[i] = false;

            if (thisTrigger.trigger != BaseInteractable.TriggerChange.Function && GUILayout.Button(thisTrigger.property ? "Is Field" : "Is Property"))
                thisTrigger.property = !thisTrigger.property;
            EditorGUILayout.EndHorizontal();

            if (showVarNames[i])
            {
                ShowVarNames(i, ref thisTrigger);
                if (GUILayout.Button("Select " + (thisTrigger.trigger == BaseInteractable.TriggerChange.Function ? "Function" : "Variable")))
                {
                    thisTrigger.functionParams.Clear();
                    thisTrigger.name = GetVarNames(thisTrigger)[varSelection[i]];
                    if(thisTrigger.trigger == BaseInteractable.TriggerChange.Function)
                    {
                        MethodInfo thisFunct = thisTrigger.script.GetType().GetMethod(thisTrigger.name);
                        ParameterInfo[] functParams = thisFunct.GetParameters();

                        for (int k = 0; k < functParams.Length; k++)
                        {
                            thisTrigger.functionParams.Add(new BaseInteractable.FunctionParams());
                            switch(functParams[k].ParameterType.Name)
                            {
                                case "Boolean":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.Bool;
                                    break;
                                case "Int32":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.Int;
                                    break;
                                case "Single":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.Float;
                                    break;
                                case "String":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.String;
                                    break;
                                case "Vector2":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.Vector2;
                                    break;
                                case "Vector3":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.Vector3;
                                    break;
                                case "GameObject":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.GameObject;
                                    break;
                                case "Transform":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.Transform;
                                    break;
                                case "SpriteRenderer":
                                    thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.SpriteRenderer;
                                    break;
                                default:
                                    if(!functParams[k].ParameterType.IsEnum)
                                        Debug.Log("Missing type \"" + functParams.GetType().Name + "\"");
                                    else
                                        thisTrigger.functionParams[k].varType = BaseInteractable.VarTypes.Enum;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        string typeName;
                        object value;
                        if (thisTrigger.property)
                        {
                            PropertyInfo property = thisTrigger.script.GetType().GetProperty(thisTrigger.name);
                            value = property.GetValue(thisTrigger.script);
                        }
                        else
                        {
                            FieldInfo field = thisTrigger.script.GetType().GetField(thisTrigger.name);

                            value = field.GetValue(thisTrigger.script);
                        }

                        typeName = value.GetType().Name;
                        switch (typeName)
                        {
                            case "Boolean":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.Bool;
                                thisTrigger.boolVal = (bool)value;
                                break;
                            case "Int32":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.Int;
                                thisTrigger.intVal = (int)value;
                                break;
                            case "Single":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.Float;
                                thisTrigger.floatVal = (float)value;
                                break;
                            case "String":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.String;
                                thisTrigger.stringVal = (string)value;
                                break;
                            case "Vector2":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.Vector2;
                                thisTrigger.vec2Val = (Vector2)value;
                                break;
                            case "Vector3":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.Vector3;
                                thisTrigger.vec3Val = (Vector3)value;
                                break;
                            case "GameObject":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.GameObject;
                                thisTrigger.gameObjectVal = (GameObject)value;
                                break;
                            case "Transform":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.Transform;
                                thisTrigger.transVal = (Transform)value;
                                break;
                            case "SpriteRenderer":
                                thisTrigger.trigger = BaseInteractable.TriggerChange.SpriteRenderer;
                                thisTrigger.spriteRendVal = (SpriteRenderer)value;
                                break;
                            default:
                                if (!value.GetType().IsEnum)
                                    Debug.Log("Missing type \"" + value.GetType().Name + "\"");
                                else
                                    thisTrigger.trigger = BaseInteractable.TriggerChange.Enum;
                                break;
                        }
                    }
                }
            }

            thisTrigger.name = EditorGUILayout.TextField("Var Name", thisTrigger.name);
            thisTrigger.script = EditorGUILayout.ObjectField("Script", thisTrigger.script, typeof(Object), true);
            thisTrigger.trigger = (BaseInteractable.TriggerChange)EditorGUILayout.EnumPopup("Trigger Type", thisTrigger.trigger);
            
            if(thisTrigger.trigger == BaseInteractable.TriggerChange.Function)
            {
                ParameterInfo[] theseParams = thisTrigger.script.GetType().GetMethod(thisTrigger.name).GetParameters();
                for (int k = 0; k < thisTrigger.functionParams.Count; k++)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.LabelField(theseParams[k].Name, GUILayout.ExpandWidth(true));
                    thisTrigger.functionParams[k].varType = (BaseInteractable.VarTypes)EditorGUILayout.EnumPopup("Variable Type", thisTrigger.functionParams[k].varType);
                    switch(thisTrigger.functionParams[k].varType)
                    {
                        case BaseInteractable.VarTypes.Bool:
                            try
                            {
                                thisTrigger.functionParams[k].boolVal = EditorGUILayout.Toggle("Value", (bool)thisTrigger.functionParams[k].boolVal);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].boolVal = false;
                            }
                            break;
                        case BaseInteractable.VarTypes.Int:
                            try
                            {
                                thisTrigger.functionParams[k].intVal = EditorGUILayout.IntField("Value", (int)thisTrigger.functionParams[k].intVal);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].intVal = 0;
                            }
                            break;
                        case BaseInteractable.VarTypes.Float:
                            try
                            {
                                thisTrigger.functionParams[k].floatVal = EditorGUILayout.FloatField("Value", (float)thisTrigger.functionParams[k].floatVal);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].floatVal = 0f;
                            }
                            break;
                        case BaseInteractable.VarTypes.String:
                            try
                            {
                                thisTrigger.functionParams[k].stringVal = EditorGUILayout.TextField("Value", (string)thisTrigger.functionParams[k].stringVal);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].stringVal = "";
                            }
                            break;
                        case BaseInteractable.VarTypes.Vector2:
                            try
                            {
                                thisTrigger.functionParams[k].vec2Val = EditorGUILayout.Vector2Field("Value", (Vector2)thisTrigger.functionParams[k].vec2Val);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].vec2Val = Vector2.zero;
                            }
                            break;
                        case BaseInteractable.VarTypes.Vector3:
                            try
                            {
                                thisTrigger.functionParams[k].vec3Val = EditorGUILayout.Vector3Field("Value", (Vector3)thisTrigger.functionParams[k].vec3Val);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].vec3Val = Vector3.zero;
                            }
                            break;
                        case BaseInteractable.VarTypes.GameObject:
                            try
                            {
                                thisTrigger.functionParams[k].gameObjectVal = (GameObject)EditorGUILayout.ObjectField("Value", thisTrigger.functionParams[k].gameObjectVal, typeof(GameObject), true);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].gameObjectVal = null;
                            }
                            break;
                        case BaseInteractable.VarTypes.Transform:
                            try
                            {
                                thisTrigger.functionParams[k].transVal = (Transform)EditorGUILayout.ObjectField("Value", (Transform)thisTrigger.functionParams[k].transVal, typeof(Transform), true);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].transVal = null;
                            }
                            break;
                        case BaseInteractable.VarTypes.SpriteRenderer:
                            try
                            {
                                thisTrigger.functionParams[k].spriteRendVal = (SpriteRenderer)EditorGUILayout.ObjectField("Value", (SpriteRenderer)thisTrigger.functionParams[k].spriteRendVal, typeof(SpriteRenderer), true);
                            }
                            catch
                            {
                                thisTrigger.functionParams[k].spriteRendVal = null;
                            }
                            break;
                        case BaseInteractable.VarTypes.Enum:
                            if (CheckIfEnum(thisTrigger, k))
                            {
                                try
                                {
                                    thisTrigger.functionParams[k].enumVal = GUILayout.SelectionGrid(thisTrigger.functionParams[k].enumVal, GetEnumStrings(thisTrigger, k), 6);
                                }
                                catch
                                {
                                    thisTrigger.functionParams[k].enumVal = 0;
                                    EditorGUILayout.HelpBox("Error!", MessageType.Error);
                                }
                            }
                            break;
                    }
                }
            }
            else
            {
                switch (thisTrigger.trigger)
                {
                    case BaseInteractable.TriggerChange.Bool:
                        try
                        {
                            thisTrigger.boolVal = EditorGUILayout.Toggle("Value", (bool)thisTrigger.boolVal);
                        }
                        catch
                        {
                            thisTrigger.boolVal = false;
                        }
                        break;
                    case BaseInteractable.TriggerChange.Int:
                        try
                        {
                            thisTrigger.intVal = EditorGUILayout.IntField("Value", (int)thisTrigger.intVal);
                        }
                        catch
                        {
                            thisTrigger.intVal = 0;
                        }
                        break;
                    case BaseInteractable.TriggerChange.Float:
                        try
                        {
                            thisTrigger.floatVal = EditorGUILayout.FloatField("Value", (float)thisTrigger.floatVal);
                        }
                        catch
                        {
                            thisTrigger.floatVal = 0f;
                        }
                        break;
                    case BaseInteractable.TriggerChange.String:
                        try
                        {
                            thisTrigger.stringVal = EditorGUILayout.TextField("Value", (string)thisTrigger.stringVal);
                        }
                        catch
                        {
                            thisTrigger.stringVal = "";
                        }
                        break;
                    case BaseInteractable.TriggerChange.Vector2:
                        try
                        {
                            thisTrigger.vec2Val = EditorGUILayout.Vector2Field("Value", (Vector2)thisTrigger.vec2Val);
                        }
                        catch
                        {
                            thisTrigger.vec2Val = Vector2.zero;
                        }
                        break;
                    case BaseInteractable.TriggerChange.Vector3:
                        try
                        {
                            thisTrigger.vec3Val = EditorGUILayout.Vector3Field("Value", (Vector3)thisTrigger.vec3Val);
                        }
                        catch
                        {
                            thisTrigger.vec3Val = Vector3.zero;
                        }
                        break;
                    case BaseInteractable.TriggerChange.GameObject:
                        try
                        {
                            thisTrigger.gameObjectVal = (GameObject)EditorGUILayout.ObjectField("Value", thisTrigger.gameObjectVal, typeof(GameObject), true);
                        }
                        catch
                        {
                            thisTrigger.gameObjectVal = null;
                        }
                        break;
                    case BaseInteractable.TriggerChange.Transform:
                        try
                        {
                            thisTrigger.transVal = (Transform)EditorGUILayout.ObjectField("Value", thisTrigger.transVal, typeof(Transform), true);
                        }
                        catch
                        {
                            thisTrigger.transVal = null;
                        }
                        break;
                    case BaseInteractable.TriggerChange.SpriteRenderer:
                        try
                        {
                            thisTrigger.spriteRendVal = (SpriteRenderer)EditorGUILayout.ObjectField("Value", thisTrigger.spriteRendVal, typeof(SpriteRenderer), true);
                        }
                        catch
                        {
                            thisTrigger.spriteRendVal = null;
                        }
                        break;
                    case BaseInteractable.TriggerChange.Enum:
                        if (CheckIfEnum(thisTrigger))
                        {
                            try
                            {
                                thisTrigger.enumVal = GUILayout.SelectionGrid((int)thisTrigger.enumVal, GetEnumStrings(thisTrigger), 6);
                            }
                            catch
                            {
                                thisTrigger.enumVal = 0;
                                EditorGUILayout.HelpBox("Error!", MessageType.Error);
                            }
                        }
                        break;
                }
            }

            thisData.interactionEffects[shownInteraction].varChanges[i] = thisTrigger;
        }

        GUILayout.Space(32);
        if(GUILayout.Button("Add new trigger"))
        {
            thisData.interactionEffects[shownInteraction].varChanges.Add(new BaseInteractable.VarChanges());
            showTriggers.Add(false);
            showVarNames.Add(false);
            varScroll.Add(Vector2.zero);
            varSelection.Add(GetVarInt(thisData.interactionEffects[shownInteraction].varChanges[thisData.interactionEffects[shownInteraction].varChanges.Count - 1]));
        }
        
        if (GUI.changed)
        {
            //SerializeData();
            EditorUtility.SetDirty(thisData);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(thisData.gameObject.scene);
        }
    }
    
    bool CheckIfEnum(BaseInteractable.VarChanges thisTrigger)
    {
        if (thisTrigger.property)
        {
            if (thisTrigger.script.GetType().GetProperty(thisTrigger.name).GetValue(thisTrigger.script).GetType().IsEnum)
                return true;
            else
            {
                EditorGUILayout.HelpBox("This variable is not an enum!", MessageType.Error);
                return false;
            }
        }
        else
        {
            if(thisTrigger.script.GetType().GetField(thisTrigger.name).GetValue(thisTrigger.script).GetType().IsEnum)
                return true;
            else
            {
                EditorGUILayout.HelpBox("This variable is not an enum!", MessageType.Error);
                return false;
            }
        }
    }

    bool CheckIfEnum(object variable)
    {
        if (variable.GetType().IsEnum)
            return true;
        else
        {
            EditorGUILayout.HelpBox("This variable is not an enum!", MessageType.Error);
            return false;
        }
    }

    bool CheckIfEnum(BaseInteractable.VarChanges thisTrigger, int index)
    {
        if (thisTrigger.script.GetType().GetMethod(thisTrigger.name).GetParameters()[index].ParameterType.IsEnum)
            return true;
        else
        {
            EditorGUILayout.HelpBox("This variable is not an enum!", MessageType.Error);
            Debug.Log(thisTrigger.script.GetType().GetMethod(thisTrigger.name).GetParameters()[index].ParameterType.Name);
            return false;
        }
    }

    System.Type GetEnumType(BaseInteractable.VarChanges thisTrigger)
    {
        if(thisTrigger.property)
            return thisTrigger.script.GetType().GetProperty(thisTrigger.name).GetValue(thisTrigger.script).GetType();
        else
            return thisTrigger.script.GetType().GetField(thisTrigger.name).GetValue(thisTrigger.script).GetType();
    }

    System.Type GetEnumType(object variable)
    {
        return variable.GetType();
    }


    System.Type GetEnumType(BaseInteractable.VarChanges thisTrigger, int index)
    {
        return thisTrigger.script.GetType().GetMethod(thisTrigger.name).GetParameters()[index].ParameterType;
    }

    string[] GetEnumStrings(BaseInteractable.VarChanges thisTrigger, int index)
    {
        return thisTrigger.script.GetType().GetMethod(thisTrigger.name).GetParameters()[index].ParameterType.GetEnumNames();
    }

    string[] GetEnumStrings(BaseInteractable.VarChanges thisTrigger)
    {
        if (thisTrigger.property)
            return thisTrigger.script.GetType().GetProperty(thisTrigger.name).GetValue(thisTrigger.script).GetType().GetEnumNames();
        else
            return thisTrigger.script.GetType().GetField(thisTrigger.name).GetValue(thisTrigger.script).GetType().GetEnumNames();
    }


    string[] GetVarNames(BaseInteractable.VarChanges thisTrigger)
    {
        if (thisTrigger.script == null)
            return new string[0];
        string[] varNames;
        if (thisTrigger.trigger == BaseInteractable.TriggerChange.Function)
        {
            MethodInfo[] methods = thisTrigger.script.GetType().GetMethods();
            varNames = new string[methods.Length];
            for (int k = 0; k < methods.Length; k++)
            {
                varNames[k] = methods[k].Name;
            }
        }
        else if (!thisTrigger.property)
        {
            FieldInfo[] fields = thisTrigger.script.GetType().GetFields();
            varNames = new string[fields.Length];
            for (int k = 0; k < fields.Length; k++)
            {
                varNames[k] = fields[k].Name;
            }
        }
        else
        {
            PropertyInfo[] properties = thisTrigger.script.GetType().GetProperties();
            varNames = new string[properties.Length];
            for (int k = 0; k < properties.Length; k++)
            {
                varNames[k] = properties[k].Name;
            }
        }
        return varNames;
    }

    void ShowVarNames(int i, ref BaseInteractable.VarChanges thisTrigger)
    {
        if (thisTrigger.script == null)
            return;
        varScroll[i] = EditorGUILayout.BeginScrollView(varScroll[i], false, false, GUILayout.Height(128));
        string[] varNames = GetVarNames(thisTrigger);
        
        varSelection[i] = GUILayout.SelectionGrid(varSelection[i], varNames, 1, GUILayout.Width(512));
        EditorGUILayout.EndScrollView();
    }

    int GetVarInt(BaseInteractable.VarChanges thisTrigger)
    {
        string[] varNames = GetVarNames(thisTrigger);
        for (int i = 0; i < varNames.Length; i++)
        {
            if (varNames[i] == thisTrigger.name)
                return i;
        }

        return 0;
    }

    /*void SerializeData()
    {
        thisData.interactionEffects[shownInteraction].boolVar = new List<bool>();
        thisData.interactionEffects[shownInteraction].intVar = new List<int>();
        thisData.interactionEffects[shownInteraction].floatVar = new List<float>();
        thisData.interactionEffects[shownInteraction].stringVar = new List<string>();
        thisData.interactionEffects[shownInteraction].vec2Var = new List<Vector2>();
        thisData.interactionEffects[shownInteraction].vec3Var = new List<Vector3>();
        thisData.interactionEffects[shownInteraction].gameobjectVar = new List<GameObject>();
        thisData.interactionEffects[shownInteraction].transVar = new List<Transform>();
        thisData.interactionEffects[shownInteraction].spriteRendVar = new List<SpriteRenderer>();
        thisData.interactionEffects[shownInteraction].enumVar = new List<int>();

        thisData.interactionEffects[shownInteraction].varOrder = new List<BaseInteractable.TriggerChange>();

        thisData.interactionEffects[shownInteraction].boolParam = new List<bool>();
        thisData.interactionEffects[shownInteraction].intParam = new List<int>();
        thisData.interactionEffects[shownInteraction].floatParam = new List<float>();
        thisData.interactionEffects[shownInteraction].stringParam = new List<string>();
        thisData.interactionEffects[shownInteraction].vec2Param = new List<Vector2>();
        thisData.interactionEffects[shownInteraction].vec3Param = new List<Vector3>();
        thisData.interactionEffects[shownInteraction].gameobjectParam = new List<GameObject>();
        thisData.interactionEffects[shownInteraction].transParam = new List<Transform>();
        thisData.interactionEffects[shownInteraction].spriteRendParam = new List<SpriteRenderer>();
        thisData.interactionEffects[shownInteraction].enumParam = new List<int>();

        thisData.interactionEffects[shownInteraction].paramOrder = new List<BaseInteractable.VarTypes>();

        for (int i = 0; i < thisData.interactionEffects[shownInteraction].varChanges.Count; i++)
        {
            BaseInteractable.VarChanges thisTrigger = thisData.interactionEffects[shownInteraction].varChanges[i];
            thisData.interactionEffects[shownInteraction].varOrder.Add(thisTrigger.trigger);
            if (thisTrigger.trigger == BaseInteractable.TriggerChange.Function)
            {
                for (int k = 0; k < thisTrigger.functionParams.Count; k++)
                {
                    thisData.interactionEffects[shownInteraction].paramOrder.Add(thisTrigger.functionParams[k].varType);
                    switch (thisTrigger.functionParams[k].varType)
                    {
                        case BaseInteractable.VarTypes.Bool:
                            thisData.interactionEffects[shownInteraction].boolParam.Add((bool)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.Int:
                            thisData.interactionEffects[shownInteraction].intParam.Add((int)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.Float:
                            thisData.interactionEffects[shownInteraction].floatParam.Add((float)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.String:
                            thisData.interactionEffects[shownInteraction].stringParam.Add((string)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.Vector2:
                            thisData.interactionEffects[shownInteraction].vec2Param.Add((Vector2)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.Vector3:
                            thisData.interactionEffects[shownInteraction].vec3Param.Add((Vector3)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.GameObject:
                            thisData.interactionEffects[shownInteraction].gameobjectParam.Add((GameObject)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.Transform:
                            thisData.interactionEffects[shownInteraction].transParam.Add((Transform)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.SpriteRenderer:
                            thisData.interactionEffects[shownInteraction].spriteRendParam.Add((SpriteRenderer)thisTrigger.functionParams[k].value);
                            break;
                        case BaseInteractable.VarTypes.Enum:
                            thisData.interactionEffects[shownInteraction].enumParam.Add((int)thisTrigger.functionParams[k].value);
                            break;
                    }
                }
            }
            else
            {
                switch (thisTrigger.trigger)
                {
                    case BaseInteractable.TriggerChange.Bool:
                        thisData.interactionEffects[shownInteraction].boolVar.Add((bool)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.Int:
                        thisData.interactionEffects[shownInteraction].intVar.Add((int)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.Float:
                        thisData.interactionEffects[shownInteraction].floatVar.Add((float)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.String:
                        thisData.interactionEffects[shownInteraction].stringVar.Add((string)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.Vector2:
                        thisData.interactionEffects[shownInteraction].vec2Var.Add((Vector2)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.Vector3:
                        thisData.interactionEffects[shownInteraction].vec3Var.Add((Vector3)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.GameObject:
                        thisData.interactionEffects[shownInteraction].gameobjectVar.Add((GameObject)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.Transform:
                        thisData.interactionEffects[shownInteraction].transVar.Add((Transform)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.SpriteRenderer:
                        thisData.interactionEffects[shownInteraction].spriteRendVar.Add((SpriteRenderer)thisTrigger.value);
                        break;
                    case BaseInteractable.TriggerChange.Enum:
                        thisData.interactionEffects[shownInteraction].enumVar.Add((int)thisTrigger.value);
                        break;
                }
            }
        }
    }*/
}
