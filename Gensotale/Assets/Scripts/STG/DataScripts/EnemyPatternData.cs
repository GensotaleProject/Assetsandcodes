using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[System.Serializable, CreateAssetMenu(fileName = "NewEnemyPattern", menuName = "BattleData/EnemyPattern", order = 1)]
public class EnemyPatternData : ScriptableObject
{
    public List<string> attackName = new List<string>();

    public virtual bool PatternCode(Transform caller)
    {
        return true;
    }
}
