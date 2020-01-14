using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class BaseInteractable : MonoBehaviour
{
    public Vector2 interactOffset;
    public float interactDistance;
    public bool loopIndex;

    public int interactIndex;
    [SerializeField] public List<InteractionEffects> interactionEffects = new List<InteractionEffects>() { new InteractionEffects() };

    Transform playerTrans;
    Transform thisTrans;

    [System.Serializable]
    public class InteractionEffects
    {
        public List<VarChanges> varChanges = new List<VarChanges>();

        /*[HideInInspector, SerializeField] public List<bool> boolVar = new List<bool>();
        [HideInInspector, SerializeField] public List<int> intVar = new List<int>();
        [HideInInspector, SerializeField] public List<float> floatVar = new List<float>();
        [HideInInspector, SerializeField] public List<string> stringVar = new List<string>();
        [HideInInspector, SerializeField] public List<Vector2> vec2Var = new List<Vector2>();
        [HideInInspector, SerializeField] public List<Vector3> vec3Var = new List<Vector3>();
        [HideInInspector, SerializeField] public List<GameObject> gameobjectVar = new List<GameObject>();
        [HideInInspector, SerializeField] public List<Transform> transVar = new List<Transform>();
        [HideInInspector, SerializeField] public List<SpriteRenderer> spriteRendVar = new List<SpriteRenderer>();
        [HideInInspector, SerializeField] public List<int> enumVar = new List<int>();

        [HideInInspector, SerializeField] public List<TriggerChange> varOrder = new List<TriggerChange>();

        [HideInInspector, SerializeField] public List<bool> boolParam = new List<bool>();
        [HideInInspector, SerializeField] public List<int> intParam = new List<int>();
        [HideInInspector, SerializeField] public List<float> floatParam = new List<float>();
        [HideInInspector, SerializeField] public List<string> stringParam = new List<string>();
        [HideInInspector, SerializeField] public List<Vector2> vec2Param = new List<Vector2>();
        [HideInInspector, SerializeField] public List<Vector3> vec3Param = new List<Vector3>();
        [HideInInspector, SerializeField] public List<GameObject> gameobjectParam = new List<GameObject>();
        [HideInInspector, SerializeField] public List<Transform> transParam = new List<Transform>();
        [HideInInspector, SerializeField] public List<SpriteRenderer> spriteRendParam = new List<SpriteRenderer>();
        [HideInInspector, SerializeField] public List<int> enumParam = new List<int>();*/

        [HideInInspector, SerializeField] public List<VarTypes> paramOrder = new List<VarTypes>();
    }

    [System.Serializable]
    public class VarChanges
    {
        [SerializeField] public string name;
        [SerializeField] public Object script;
        [SerializeField] public bool property;
        [Space]
        public TriggerChange trigger;

        public bool boolVal;
        public int intVal;
        public float floatVal;
        public string stringVal;
        public Vector2 vec2Val;
        public Vector3 vec3Val;
        public GameObject gameObjectVal;
        public Transform transVal;
        public SpriteRenderer spriteRendVal;
        public int enumVal;

        //[SerializeField] public object value;
        [SerializeField] public List<FunctionParams> functionParams = new List<FunctionParams>();

        public VarChanges()
        {
            trigger = TriggerChange.Bool;
        }
    }

    [System.Serializable]
    public class FunctionParams
    {
        [SerializeField] public VarTypes varType;
        //[SerializeField] public object value;
        public bool boolVal;
        public int intVal;
        public float floatVal;
        public string stringVal;
        public Vector2 vec2Val;
        public Vector3 vec3Val;
        public GameObject gameObjectVal;
        public Transform transVal;
        public SpriteRenderer spriteRendVal;
        public int enumVal;

        public FunctionParams()
        {
            this.varType = VarTypes.Bool;
        }
    }

    public enum TriggerChange { Function, Bool, Int, Float, String, Vector2, Vector3, GameObject, Transform, SpriteRenderer, Enum }
    public enum VarTypes { Bool, Int, Float, String, Vector2, Vector3, GameObject, Transform, SpriteRenderer, Enum }

    private void Awake()
    {
        thisTrans = transform;
        //Deserialize();
    }

    private void Start()
    {
        playerTrans = RoomMaster.roomMaster.playerTrans;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInteract();
    }

    void CheckInteract()
    {
        if(InputScript.inputScript.shootDown && Vector2.Distance(playerTrans.position, (Vector2)thisTrans.position + interactOffset) <= interactDistance)
            TriggerInteraction();
    }

    void TriggerInteraction()
    {
        VarChanges thisChange;
        for (int i = 0; i < interactionEffects[interactIndex].varChanges.Count; i++)
        {
            thisChange = interactionEffects[interactIndex].varChanges[i];
            if (interactionEffects[interactIndex].varChanges[i].trigger == TriggerChange.Function)
            {
                thisChange.script.GetType().GetMethod(thisChange.name).Invoke(thisChange.script, GetFunctionParams(i));
                continue;
            }

            if(!thisChange.property)
                thisChange.script.GetType().GetField(thisChange.name).SetValue(thisChange.script, GetValue(thisChange));
            else
                thisChange.script.GetType().GetProperty(thisChange.name).SetValue(thisChange.script, GetValue(thisChange));
        }
        interactIndex++;
        if (loopIndex)
        {
            if (interactIndex >= interactionEffects.Count)
                interactIndex = 0;
        }
        else
            interactIndex = Mathf.Clamp(interactIndex, 0, interactionEffects.Count - 1);
    }

    object[] GetFunctionParams(int varChangeIndex)
    {
        List<object> objList = new List<object>();
        for (int i = 0; i < interactionEffects[interactIndex].varChanges[varChangeIndex].functionParams.Count; i++)
        {
            objList.Add(GetValue(interactionEffects[interactIndex].varChanges[varChangeIndex].functionParams[i]));
        }
        return objList.ToArray();
    }


    public object GetValue(VarChanges trigger)
    {
        switch(trigger.trigger)
        {
            case TriggerChange.Bool:
                return trigger.boolVal;
            case TriggerChange.Int:
                return trigger.intVal;
            case TriggerChange.Float:
                return trigger.floatVal;
            case TriggerChange.String:
                return trigger.stringVal;
            case TriggerChange.Vector2:
                return trigger.vec2Val;
            case TriggerChange.Vector3:
                return trigger.vec3Val;
            case TriggerChange.GameObject:
                return trigger.gameObjectVal;
            case TriggerChange.Transform:
                return trigger.transVal;
            case TriggerChange.SpriteRenderer:
                return trigger.spriteRendVal;
            case TriggerChange.Enum:
                return trigger.enumVal;
            default:
                return trigger.boolVal;
        }
    }

    public object GetValue(FunctionParams param)
    {
        switch (param.varType)
        {
            case VarTypes.Bool:
                return param.boolVal;
            case VarTypes.Int:
                return param.intVal;
            case VarTypes.Float:
                return param.floatVal;
            case VarTypes.String:
                return param.stringVal;
            case VarTypes.Vector2:
                return param.vec2Val;
            case VarTypes.Vector3:
                return param.vec3Val;
            case VarTypes.GameObject:
                return param.gameObjectVal;
            case VarTypes.Transform:
                return param.transVal;
            case VarTypes.SpriteRenderer:
                return param.spriteRendVal;
            case VarTypes.Enum:
                return param.enumVal;
            default:
                return param.boolVal;
        }
    }

    /*public void Deserialize()
    {
        for (int i = 0; i < interactionEffects.Count; i++)
        {
            for (int k = 0; k < interactionEffects[i].varChanges.Count; k++)
            {
                //Debug.Log(interactionEffects[i].varOrder.Count);
                switch (interactionEffects[i].varOrder[0])
                {
                    case TriggerChange.Bool:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].boolVar[0];
                        interactionEffects[i].boolVar.RemoveAt(0);
                        break;
                    case TriggerChange.Int:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].intVar[0];
                        interactionEffects[i].intVar.RemoveAt(0);
                        break;
                    case TriggerChange.Float:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].floatVar[0];
                        interactionEffects[i].floatVar.RemoveAt(0);
                        break;
                    case TriggerChange.String:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].stringVar[0];
                        interactionEffects[i].stringVar.RemoveAt(0);
                        break;
                    case TriggerChange.Vector2:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].vec2Var[0];
                        interactionEffects[i].vec2Var.RemoveAt(0);
                        break;
                    case TriggerChange.Vector3:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].vec3Var[0];
                        interactionEffects[i].vec3Var.RemoveAt(0);
                        break;
                    case TriggerChange.GameObject:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].gameobjectVar[0];
                        interactionEffects[i].gameobjectVar.RemoveAt(0);
                        break;
                    case TriggerChange.Transform:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].transVar[0];
                        interactionEffects[i].transVar.RemoveAt(0);
                        break;
                    case TriggerChange.SpriteRenderer:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].spriteRendVar[0];
                        interactionEffects[i].spriteRendVar.RemoveAt(0);
                        break;
                    case TriggerChange.Enum:
                        interactionEffects[i].varChanges[k].value = interactionEffects[i].enumVar[0];
                        interactionEffects[i].enumVar.RemoveAt(0);
                        break;

                    case TriggerChange.Function:
                        //Debug.Log(interactionEffects[i].varChanges[k].functionParams.Count);
                        for (int f = 0; f < interactionEffects[i].varChanges[k].functionParams.Count; f++)
                        {

                            switch (interactionEffects[i].paramOrder[0])
                            {
                                case VarTypes.Bool:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].boolParam[0];
                                    interactionEffects[i].boolParam.RemoveAt(0);
                                    break;
                                case VarTypes.Int:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].intParam[0];
                                    interactionEffects[i].intParam.RemoveAt(0);
                                    break;
                                case VarTypes.Float:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].floatParam[0];
                                    interactionEffects[i].floatParam.RemoveAt(0);
                                    break;
                                case VarTypes.String:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].stringParam[0];
                                    interactionEffects[i].stringParam.RemoveAt(0);
                                    break;
                                case VarTypes.Vector2:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].vec2Param[0];
                                    interactionEffects[i].vec2Param.RemoveAt(0);
                                    break;
                                case VarTypes.Vector3:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].vec3Param[0];
                                    interactionEffects[i].vec3Param.RemoveAt(0);
                                    break;
                                case VarTypes.GameObject:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].gameobjectParam[0];
                                    interactionEffects[i].gameobjectParam.RemoveAt(0);
                                    break;
                                case VarTypes.Transform:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].transParam[0];
                                    interactionEffects[i].transParam.RemoveAt(0);
                                    break;
                                case VarTypes.SpriteRenderer:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].spriteRendParam[0];
                                    interactionEffects[i].spriteRendParam.RemoveAt(0);
                                    break;
                                case VarTypes.Enum:
                                    interactionEffects[i].varChanges[k].functionParams[f].value = interactionEffects[i].enumParam[0];
                                    interactionEffects[i].enumParam.RemoveAt(0);
                                    break;
                            }
                            interactionEffects[i].paramOrder.RemoveAt(0);
                        }
                        break;
                }
                interactionEffects[i].varOrder.RemoveAt(0);
            }
        }
    }*/
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 1, .25f);
        Gizmos.DrawSphere(transform.position + (Vector3)interactOffset, interactDistance);
    }
}
