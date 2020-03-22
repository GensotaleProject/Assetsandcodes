using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    [SerializeField] public EnemyData enemyData;
    bool calledEnemyPattern;

    Transform thisTrans;
    // Start is called before the first frame update
    void Start()
    {
        thisTrans = transform;   
    }

    // Update is called once per frame
    void Update()
    {
        if(!calledEnemyPattern)
        {
            for (int i = 0; i < enemyData.patterns.Count; i++)
            {
                calledEnemyPattern = enemyData.patterns[i].PatternCode(thisTrans);
                if (!calledEnemyPattern)
                    Debug.Log("Error with pattern index " + i + " on enemy " + thisTrans.name);
            }
        }
    }
}
