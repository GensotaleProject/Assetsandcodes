using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "NewEncounter", menuName = "BattleData/EncounterData", order = 1)]
public class EncounterData : ScriptableObject
{
    public TextData.TextLine openingLine;
    public EnemyData[] enemySpawns;
}
