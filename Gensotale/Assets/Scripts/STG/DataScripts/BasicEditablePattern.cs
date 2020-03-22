using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable, CreateAssetMenu(fileName = "NewEnemyPattern", menuName = "BattleData/EnemyPattern/BasicEditable", order = 1)]
public class BasicEditablePattern : EnemyPatternData
{
    public List<Pattern> patterns = new List<Pattern>();

    public override bool PatternCode(Transform caller)
    {
        for (int i = 0; i < patterns.Count; i++)
        {
            caller.GetComponent<BaseEnemy>().StartCoroutine(ProcessData(caller, patterns[i]));
        }
        return true;
    }

    public IEnumerator ProcessData(Transform caller, Pattern thisPattern)
    {
        Vector2 position = caller.position;
        GameMaster gameMaster = GameMaster.gameMaster;
        BattleMaster battleMaster = BattleMaster.battleMaster;

        int perBulletIndex = 0, delayIndex = 0, bulletTypeIndex = 0, colorIndex = 0, movementIndex = 0, offsetIndex = 0;
        float random = 0;
        float shootDelay = 0;
        int burstCount = thisPattern.burstCount;

        int thisBulletType;
        int thisColor;
        int thisMovement;
        int thisOffset;
        GameObject newBullet;
        BaseBulletScript newScript = null;

        do
        {
            while (shootDelay > 0)
            {
                shootDelay -= gameMaster.timeScale * gameMaster.frameTime;
                yield return null;
            }
            
            if (perBulletIndex == 0)
                random = !thisPattern.perBulletRandomOffset ? GameMaster.Random(-thisPattern.randomDegreeOffset, thisPattern.randomDegreeOffset) : 0;
            float startingAngle = thisPattern.degreeOffset + ((thisPattern.aimed) ? MathFunctions.FindAngleToPlayer(position) : 0)
                + random;
            float currentAngle;
            Vector2 offset;
            for (int i = perBulletIndex; i < thisPattern.bulletsInBurst; i++)
            {
                thisBulletType = bulletTypeIndex;
                thisColor = colorIndex;
                thisMovement = movementIndex;
                thisOffset = offsetIndex;

                currentAngle = startingAngle + (((-thisPattern.overallDegrees / thisPattern.bulletsInBurst) * (thisPattern.bulletsInBurst / 2)) +
                        ((thisPattern.overallDegrees / thisPattern.bulletsInBurst) * i)) + ((thisPattern.perBulletRandomOffset) ?
                        GameMaster.Random(-thisPattern.randomDegreeOffset, thisPattern.randomDegreeOffset) : 0);

                offset = !thisPattern.turnOffsets[thisOffset] ? thisPattern.offsets[thisOffset] :
                    MathFunctions.CalculateCircle(thisPattern.offsets[thisOffset].magnitude, MathFunctions.FindAngle(thisPattern.offsets[thisOffset]) + currentAngle);

                bool spawnCustom = thisPattern.bulletType[thisBulletType] == BaseBulletScript.BulletType.Custom/* || thisPattern.bulletType[thisBulletType] == BaseBulletScript.BulletType.CustomLaser*/;
                //bool isLaser = thisPattern.bulletType[thisBulletType] == BaseBulletScript.BulletType.CustomLaser || thisPattern.bulletType[thisBulletType] == BaseBulletScript.BulletType.StraightLaser;
                bool isLaser = false;
                //This code is from another project of mine, but since I haven't put lasers into Gensotale
                //I am just commenting it out in case they're added in the future -- we can clean out the 
                //Comments once we decide

                newBullet = Instantiate((spawnCustom ? thisPattern.customBullet[thisBulletType] :
                    BaseBulletScript.GetBulletType(thisPattern.bulletType[thisBulletType])),
                    position + offset, new Quaternion(), battleMaster.enemyBulletContainer);

                if (!(thisPattern.customHasScript[thisBulletType] && thisPattern.bulletType[thisBulletType] == BaseBulletScript.BulletType.Custom)
                    && !isLaser)
                {
                    newScript = newBullet.GetComponent<BaseBulletScript>();
                    newScript.movementDirection = MathFunctions.CalculateCircle(currentAngle);

                    newScript.endingSpeed = thisPattern.bulletEndSpeed[thisMovement];
                    newScript.startingSpeed = thisPattern.bulletStartSpeed[thisMovement];
                    newScript.timeToChangeSpeed = thisPattern.bulletTimeToSpeedUp[thisMovement];
                    newScript.turnAtStart = thisPattern.turn[thisMovement];
                }
                /*else if (isLaser)
                {
                    CurvingLaserScript laserScript = newBullet.GetComponent<CurvingLaserScript>();
                    LaserRenderer laserRenderer = newBullet.GetComponent<LaserRenderer>();
                    newBullet.transform.up = MathFunctions.CalculateCircle(currentAngle);
                    laserScript.width = thisPattern.bulletStartingSpeed[thisMovement];
                    laserScript.timeToSpawn = thisPattern.burstDelay;
                    laserScript.segmentDelay = thisPattern.bulletTimeToSpeedUp[thisMovement];
                    laserScript.laserColor = thisPattern.bulletColors[thisColor];
                    laserRenderer.laserColor = thisPattern.bulletColors[thisColor];

                    laserScript.speed = thisPattern.bulletSpeed[thisMovement];
                }*/

                if (!isLaser)
                {
                    if (thisPattern.bulletType[thisBulletType] != BaseBulletScript.BulletType.Custom)
                    {
                        newScript.spriteType = thisPattern.bulletType[thisBulletType];
                        newScript.bulletColor = thisPattern.bulletColors[thisColor];
                    }
                    else if (!thisPattern.customHasSprite[thisBulletType])
                    {
                        newScript.spriteType = thisPattern.customSpriteType[thisBulletType];
                        newScript.bulletColor = thisPattern.bulletColors[thisColor];
                    }
                }


                bulletTypeIndex = (int)Mathf.Repeat(bulletTypeIndex + 1, thisPattern.bulletType.Count);
                colorIndex = (int)Mathf.Repeat(colorIndex + 1, thisPattern.bulletColors.Count);
                movementIndex = (int)Mathf.Repeat(movementIndex + 1, thisPattern.bulletEndSpeed.Count);
                offsetIndex = (int)Mathf.Repeat(offsetIndex + 1, thisPattern.offsets.Count);

                perBulletIndex++;
                if (perBulletIndex >= thisPattern.bulletsInBurst)
                {
                    if (thisPattern.bulletsInBurst < 2)
                        shootDelay += thisPattern.perBulletDelay[delayIndex];
                    shootDelay += thisPattern.burstDelay;
                    perBulletIndex = 0;
                    burstCount--;
                }
                else if (thisPattern.perBulletDelay[delayIndex] > 0)
                {
                    //shootDelay -= bulletSpawnWait;
                    shootDelay += thisPattern.perBulletDelay[delayIndex];
                    delayIndex = (int)Mathf.Repeat(delayIndex + 1, thisPattern.perBulletDelay.Count);
                    break;
                }
                delayIndex = (int)Mathf.Repeat(delayIndex + 1, thisPattern.perBulletDelay.Count);
                yield return null;
            }
        } while (burstCount > 0);
    }
}
