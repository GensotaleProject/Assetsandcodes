using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "NewEnemy", menuName = "BattleData/Enemy", order = 1)]
public class EnemyData : ScriptableObject
{
    public List<string> enemyName = new List<string>();
    public int maxHp;
    public int hp;
    public int xpDrop;
    public int gDrop;
    public int defense;
    public int attack;
    [Space]
    public TextData.TextLine enemyDescription;
    [Space]
    public List<ConditionalLine> enemyLines = new List<ConditionalLine>();
    public bool orderedDialogue;
    public List<ConditionalLine> battlefieldText = new List<ConditionalLine>();
    [Space]
    public List<ACTData> actData = new List<ACTData>();
    [Space]
    public List<EnemyPatternData> patterns = new List<EnemyPatternData>();
    [Space]
    public GameObject enemySpawn;
    [Space]
    public List<Properties> properties = new List<Properties>();

    /***Notes concerning enemy text***
     * the description is what is displayed when you Check an enemy
     * When the text is written, there are a few commands I will write code for
     * {n} inserts the enemy's name
     * {d} inserts the enemy's defense
     * {a} inserts the enemy's attack
     * {h} inserts the enemy's hp
     * {m} inserts the enemy's max hp
     * {e} inserts the enemy's exp drop
     * {c} inserts the enemy's currency drop
     * Example description with a character named 'FAIRY' with 2 attack and 2 defense:
     * "{n}  {a}  {d}" = "FAIRY  2 ATK  2 DEF"
     * This will make it easy to change values such as minor name and stat tweaks without needing to edit the text manually
     */

    [System.Serializable]
    public enum DataType { Bool, Int, Float, String };
    [System.Serializable]
    public enum Condition { Equal, NotEqual, LessThan, LessOrEqual, GreaterThan, GreaterOrEqual, };

    [System.Serializable]
    public struct Properties
    {
        public string propName;
        public DataType type;
        public bool boolVar;
        public int intVar;
        public float floatVar;
        public string stringVar;
    }

    [System.Serializable]
    public class ConditionalLine
    {
        public int priority;
        public TextData.TextLine line;
        public List<ConditionParams> conditions = new List<ConditionParams>();
    }

    [System.Serializable]
    public struct ConditionParams
    {
        public string propertyName;
        public Condition conditionToMeet;
        public string compareProperty;
        public Properties newCompareValue;
    }
}
