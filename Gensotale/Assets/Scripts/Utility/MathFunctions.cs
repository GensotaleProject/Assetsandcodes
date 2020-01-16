using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathFunctions {
    //Note: All angles are calculated with 0 degrees being the vector 0,1,0 -- straight up

    //Calculates a position in a circle by generating a random angle value with a set radius and centerpoint
    public static Vector2 RandomCircle(Vector3 center, float radius)
    {
        //Creates random angle between 0 and 360
        float ang = GameMaster.Random(0f, 1f) * 360;
        Vector2 pos;
        //Calculates the sin for the x position and cos for the y, multiplies it by the radius, and then adds the center x and y to get it to the correct point
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    //Same as above, except the center point is 0,0 -- good for if you only need relative coordinates
    public static Vector2 RandomCircle(float radius)
    {
        //Creates random angle between 0 and 360
        float ang = GameMaster.Random(0f, 1f) * 360;
        Vector2 pos;
        //Calculates the sin for the x position and cos for the y, and then multiplies it by the radius to get it to the correct point
        pos.x = radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    //Same as above, except the center point is 0,0 and the radius is 1 -- good for if you only need the direction
    public static Vector2 RandomCircle()
    {
        //Creates random angle between 0 and 360
        float ang = GameMaster.Random(0f, 1f) * 360;
        Vector2 pos;
        //Calculates the sin for the x position and the cos for the y
        pos.x = Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }

    //Calculates a position in a circle by taking an exact angle and uses a centerpoint and radius
    public static Vector2 CalculateCircle(Vector2 center, float radius, float angle)
    {
        Vector2 pos;
        //Calculates the sin for the x position and cos for the y, multiplies it by the radius, and then adds the center x and y to get it to the correct point
        pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }

    //Same as above, except the center point is 0,0 -- good for if you only need relative coordinates 
    public static Vector2 CalculateCircle(float radius, float angle)
    {
        Vector2 pos;
        //Calculates the sin for the x position and cos for the y, and then multiplies it by the radius to get it to the correct point
        pos.x = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }

    //Same as above, except the center point is 0,0 and the radius is 1 -- good for if you only need the direction
    public static Vector2 CalculateCircle(float angle)
    {
        Vector2 pos;
        //Calculates the sin for the x position and the cos for the y
        pos.x = Mathf.Sin(angle * Mathf.Deg2Rad);
        pos.y = Mathf.Cos(angle * Mathf.Deg2Rad);
        return pos;
    }

    //Finds the direction to the center of the play area -- change the vector value as needed
    public static Vector2 FindDirectionToCenter(Vector2 startingPosition)
    {
        return (new Vector2(0, 224) - startingPosition).normalized;
    }

    //Uses the above function to find the direction, and then converts it to an angle
    public static float FindAngleToCenter(Vector2 startingPosition)
    {
        Vector2 directionToCenter = FindDirectionToCenter(startingPosition);
        return Vector2.SignedAngle(directionToCenter, Vector2.up);
    }

    //Finds the direction to the player object stored in the GameMaster
    public static Vector2 FindDirectionToPlayer(Vector2 startingPosition)
    {
        return ((Vector2)GameMaster.gameMaster.playerTrans.position - startingPosition).normalized;
    }

    //Uses the above function to find the angle to the player object stored in the GameMaster
    public static float FindAngleToPlayer(Vector2 startingPosition)
    {
        Vector2 directionToPlayer = FindDirectionToPlayer(startingPosition);
        return Vector2.SignedAngle(directionToPlayer, Vector2.up);
    }

    //Finds the direction between an object's x and z and the starting position's x and z -- mainly used for background effects and turning sprites towards the camera
    public static Vector3 FindDirectionToObject3D(Vector3 startingPosition, Transform otherObject)
    {
        return (new Vector3(otherObject.position.x, 0, otherObject.position.z) - new Vector3(startingPosition.x, 0, startingPosition.z)).normalized;
    }

    //Uses the transform of an object to find the direction towards it from the starting position
    public static Vector2 FindDirectionToObject(Vector2 startingPosition, Transform otherObject)
    {
        return ((Vector2)otherObject.position - startingPosition).normalized;
    }

    //Uses the function above to find the angle towards another object from the starting position
    public static float FindAngleToObject(Vector2 startingPosition, Transform otherObject)
    {
        Vector2 directionToObject = FindDirectionToObject(startingPosition, otherObject);
        return Vector2.SignedAngle(directionToObject, Vector2.up);
    }

    //Finds the angle between two positions
    public static float FindAngle(Vector2 startPoint, Vector2 endPoint)
    {
        Vector2 direction = endPoint - startPoint;
        return Vector2.SignedAngle(direction, Vector2.up);
    }

    //Finds the angle difference between two directions
    public static float FindAngleBetween(Vector2 startPoint, Vector2 endPoint)
    {
        return Vector2.SignedAngle(startPoint, endPoint);
    }

    //Finds the angle of the direction, based off of 0,1
    public static float FindAngle(Vector2 direction)
    {
        return Vector2.SignedAngle(direction, Vector2.up);
    }

    //Runs through every enemy position under the enemy container, and finds the nearest to the position within a maximum distance of the max radius
    //And returns the shortest distance as an out
    //This accounts for bosses under the if statements below the main for loop
   /* public static Transform FindNearestEnemy(Vector2 position, float maxRadius, out float shortestDist)
    {
        shortestDist = float.MaxValue;
        float tempDist;
        Transform newTarget = null;
        GameMaster gameMaster = GameMaster.gameMaster;
        Transform enemyContainer = gameMaster.enemyContainer;
        int childCount = enemyContainer.childCount;

        for (int i = 0; i < childCount; i++)
        {
            tempDist = Vector2.Distance(position, enemyContainer.GetChild(i).position);
            if (tempDist <= maxRadius && tempDist < shortestDist)
            {
                newTarget = enemyContainer.GetChild(i);
                shortestDist = tempDist;
            }
        }

        //Finds nearest distance to any midboss while fighting the midboss -- is set up to allow for multiple-character midbosses
        if (gameMaster.stageScript.fightingMidboss)
        {
            BaseBossScript[] midbosses;
            midbosses = gameMaster.stageScript.midbossSpawn.GetComponentsInChildren<BaseBossScript>();
            Transform thisBoss;
            for (int i = 0; i < midbosses.Length; i++)
            {
                thisBoss = midbosses[i].transform;
                tempDist = Vector2.Distance(thisBoss.position, position);
                if (tempDist <= maxRadius && tempDist <= shortestDist)
                {
                    newTarget = thisBoss;
                    shortestDist = tempDist;
                }
            }
        }

        //Finds nearest distance to any boss while fighting the boss -- is set up to allow for multiple-character bosses
        if (gameMaster.stageScript.fightingBoss)
        {
            BaseBossScript[] bosses;
            bosses = gameMaster.stageScript.bossSpawn.GetComponentsInChildren<BaseBossScript>();
            Transform thisBoss;
            for (int i = 0; i < bosses.Length; i++)
            {
                thisBoss = bosses[i].transform;
                tempDist = Vector2.Distance(thisBoss.position, position);
                if (tempDist <= maxRadius && tempDist <= shortestDist)
                {
                    newTarget = thisBoss;
                    shortestDist = tempDist;
                }
            }
        }

        //Returns the nearest enemy's transform
        return newTarget;
    }
    */
    //Same the above function, and even uses it, but doesn't return the shortest distance
    /*public static Transform FindNearestEnemy(Vector2 position, float maxRadius)
    {
        float outDeletion;
        return FindNearestEnemy(position, maxRadius, out outDeletion);
    }*/


    //Runs through every enemy position under the enemy container, and finds the nearest to the position
    //And returns the shortest distance as an out
    //This accounts for bosses under the if statements below the main for loop
    /*public static Transform FindNearestEnemy(Vector2 position, out float shortestDist)
    {
        shortestDist = float.MaxValue;
        float tempDist;
        Transform newTarget = null;
        GameMaster gameMaster = GameMaster.gameMaster;
        Transform enemyContainer = gameMaster.enemyContainer;
        int childCount = enemyContainer.childCount;

        for (int i = 0; i < childCount; i++)
        {
            tempDist = Vector2.Distance(position, enemyContainer.GetChild(i).position);
            if (tempDist < shortestDist)
            {
                newTarget = enemyContainer.GetChild(i);
                shortestDist = tempDist;
            }
        }

        //Finds nearest distance to any midboss while fighting the midboss -- is set up to allow for multiple-character midbosses
        if (gameMaster.stageScript.fightingMidboss)
        {
            BaseBossScript[] midbosses;
            midbosses = gameMaster.stageScript.midbossSpawn.GetComponentsInChildren<BaseBossScript>();
            Transform thisBoss;
            for (int i = 0; i < midbosses.Length; i++)
            {
                thisBoss = midbosses[i].transform;
                tempDist = Vector2.Distance(thisBoss.position, position);
                if (tempDist <= shortestDist)
                {
                    newTarget = thisBoss;
                    shortestDist = tempDist;
                }
            }
        }

        //Finds nearest distance to any boss while fighting the boss -- is set up to allow for multiple-character bosses
        if (gameMaster.stageScript.fightingBoss)
        {
            BaseBossScript[] bosses;
            bosses = gameMaster.stageScript.bossSpawn.GetComponentsInChildren<BaseBossScript>();
            Transform thisBoss;
            for (int i = 0; i < bosses.Length; i++)
            {
                thisBoss = bosses[i].transform;
                tempDist = Vector2.Distance(thisBoss.position, position);
                if (tempDist <= shortestDist)
                {
                    newTarget = thisBoss;
                    shortestDist = tempDist;
                }
            }
        }

        return newTarget;
    }*/

    //Same the above function, and even uses it, but doesn't return the shortest distance
    /*public static Transform FindNearestEnemy(Vector2 position)
    {
        float outDeletion;
        return FindNearestEnemy(position, out outDeletion);
    }*/
}
