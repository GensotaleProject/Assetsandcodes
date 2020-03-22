using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable, CreateAssetMenu(fileName = "NewActData", menuName = "BattleData/ActData", order = 1)]
public class ACTData
{
    public List<string> actLine = new List<string>();
    public List<EnemyData.Properties> propertyChanges = new List<EnemyData.Properties>();

    //Note: Check will always be included without needing to be added
}
