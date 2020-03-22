using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBulletScript : MonoBehaviour
{
    public static int newKillzoneIndex;
    public int killzoneDetectIndex;

    static System.Random bulletRandom = new System.Random(0);
    public static Transform playerTransform;

    [Header("BulletType")]
    public bool ignoreRenderer;
    public BulletType spriteType;
    public BulletColor bulletColor;
    public bool animatedSprite;
    public Sprite customSprite;
    public RuntimeAnimatorController customAnim;
    public float damage;
    [Header("Movement")]
    public Vector2 movementDirection;
    public float currentSpeed,startingSpeed,endingSpeed, timeToChangeSpeed;
    public bool rotate;
    [Header("Spawn")]
    public bool turnAtStart;
    public float initialMoveDelay;
    public bool turnAfterDelay;
    public float spawnDelay = 0.25f;
    float spawnDelayCurrent;
    [Header("Misc")]
    bool destroyingBullet;
    public bool destroy;

    Transform thisTrans;
    Vector2 prevPos;
    Transform transToTurn;
    Transform spriteTrans;
    Transform colliderContainer;
    SpriteRenderer spriteRenderer;
    Animator spriteAnimator;

    GameMaster gameMaster;
    Vector3 startScale;
    Color startColor;

    const float spawnEffectStartScale = 3;
    const int framesBetweenRotate = 2;
    int rotateFramesWaited;
    float timePassed;

    [System.Serializable]
    public enum BulletColor
    {
        White, WhiteInverted, Blue, BlueInverted, Orange, OrangeInverted
    };
    [System.Serializable]
    public enum BulletType
    {
        Custom, Round, Wave, SmallShard,
        ThickShard, Crystal, Kunai, Shell, Pellet,
        RoundOutlined, Dagger, Star, BigDrop, SmallDrop,
        Charm, CharmOutline, BeamOnePxWide, BeamOnePx,
        BeamTwoPx, BeamThreePx, BeamFourPx, Heart, Popcorn,
        DarkPopcorn, Coin, Droplet, Arrow, Rest, Giant, 
        Butterfly, Fire, MusicNote, 
    };

    // Start is called before the first frame update
    void Start()
    {
        killzoneDetectIndex = newKillzoneIndex;
        newKillzoneIndex = (int)Mathf.Repeat(newKillzoneIndex + 1, 6);

        thisTrans = transform;
        transToTurn = thisTrans.GetChild(0);
        colliderContainer = transToTurn.GetChild(1);
        gameMaster = GameMaster.gameMaster;

        DetectDestroy();

        if (turnAtStart)
            transToTurn.up = MathFunctions.CalculateCircle(MathFunctions.FindAngle(movementDirection) -
                ((thisTrans.parent != null) ? thisTrans.parent.eulerAngles.z + thisTrans.eulerAngles.z : thisTrans.eulerAngles.z));

        if (!ignoreRenderer)
        {
            spriteRenderer = transToTurn.GetComponentInChildren<SpriteRenderer>();
            spriteAnimator = transToTurn.GetComponentInChildren<Animator>();
            spriteTrans = spriteRenderer.transform;

            startScale = spriteRenderer.transform.localScale;
            startColor = spriteRenderer.color;

            UpdateSprite();
            
            spriteRenderer.color = new Color(1, 1, 1, 0);
            spriteTrans.localScale *= spawnEffectStartScale;
        }

        prevPos = thisTrans.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Trigers destroy animation, if it is destroying, exit out of code immediately
        if (destroyingBullet)
            return;
        else if (destroy)
        {
            destroyingBullet = true;
            if (ignoreRenderer)
                thisTrans.gameObject.SetActive(false);
            else
                StartCoroutine(DestroyAnimation());
            return;
        }

        SpawnAnim();
        Movement();
        AdditionalFunctions();
        if(rotate)
            Rotate();
        DetectDestroy();
        prevPos = thisTrans.position;
    }


    public virtual void AdditionalFunctions()
    {
        /*To be overwritten using:
         public override void AdditionalFunctions()
         {
         }
        This is used to create unique bullet functionality in child scripts*/
    }

    public void SpawnAnim()
    {
        if (spawnDelayCurrent < spawnDelay && !ignoreRenderer)
        {
            spawnDelayCurrent += gameMaster.frameTime * gameMaster.timeScale;

            spriteTrans.localScale = startScale * Mathf.Lerp(spawnEffectStartScale, 1, spawnDelayCurrent / spawnDelay);
            spriteRenderer.color = Color.Lerp(new Color(startColor.r, startColor.g, startColor.b, 0), startColor, spawnDelayCurrent / (spawnDelay * 2.5f));

            if (spawnDelayCurrent >= spawnDelay)
            {
                spriteRenderer.color = startColor;
                spriteTrans.localScale = startScale;
            }
        }
    }

    //Moves bullet
    public void Movement()
    {
        if (initialMoveDelay <= 0)
        {
            thisTrans.localPosition += (Vector3)(movementDirection * currentSpeed * gameMaster.frameTime * gameMaster.timeScale);

            if (timeToChangeSpeed > 0)
            {
                timePassed += gameMaster.frameTime * gameMaster.timeScale;

                currentSpeed = Mathf.Lerp(startingSpeed, endingSpeed, timePassed / timeToChangeSpeed);
            }

        }
        else
            initialMoveDelay -= gameMaster.frameTime * gameMaster.timeScale;
    }

    //Rotates bullet
    public void Rotate()
    {
        if (rotateFramesWaited >= framesBetweenRotate)
        {
            if (rotate)
                if (!turnAfterDelay || initialMoveDelay <= 0)
                {
                    if (Vector2.Distance(thisTrans.position, prevPos) > 0.01f)
                        transToTurn.up = new Vector2(thisTrans.position.x - prevPos.x, thisTrans.position.y - prevPos.y).normalized;
                    rotateFramesWaited = 0;
                }
        }
        else
            rotateFramesWaited++;
    }

    //Destroy detection
    public void DetectDestroy()
    {
        /*if (killzoneDetectIndex == gameMaster.killzoneDetectIndex)
        {
            if (regularKillzone && !regularDestroyExtents)
            {
                DetectKillzone(destroyXExtents, destroyYExtents);
            }
            else if (regularKillzone)
            {
                DetectKillzone();
            }
        }*/
    }

    //Detects when bullets are destroyed
    public void DetectKillzone()
    {
        /*if (thisTrans.position.x < regularDestroyExtentsX.x || thisTrans.position.x > regularDestroyExtentsX.y || thisTrans.position.y < regularDestroyExtentsY.x || thisTrans.position.y > regularDestroyExtentsY.y)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
        else if (!destroyImmune)
        {
            DetectTrigger();
        }*/
    }

    public void DetectKillzone(Vector2 xLimits, Vector2 yLimits)
    {
        Vector2 pos = thisTrans.position;
        if (pos.x < xLimits.x || pos.x > xLimits.y || pos.y < yLimits.x || pos.y > yLimits.y)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
        else/* if (!destroyImmune)*/
        {
            DetectTrigger();
        }
    }

    //Detects areas that force bullet destruction
    public void DetectTrigger()
    {

    }

    //Destroy animation
    public IEnumerator DestroyAnimation()
    {
        float destroyTime = 0.15f;

        if (animatedSprite)
            spriteAnimator.enabled = false;

        spriteRenderer.sprite = gameMaster.destroyEffects[(int)bulletColor];

        DisableAllColliders();

        Vector2 startingScale = thisTrans.localScale;
        Vector2 endingScale = startingScale * 1.5f;
        float timePassed = 0;
        Color startingColor = spriteRenderer.color;
        while (destroyTime > timePassed)
        {
            timePassed += gameMaster.timeScale * gameMaster.frameTime;
            thisTrans.localScale = Vector2.Lerp(startingScale, endingScale, timePassed / destroyTime);
            spriteRenderer.color = new Color(startingColor.r, startingColor.g, startingColor.b, Mathf.Lerp(startingColor.a, 0, timePassed / destroyTime));
            yield return null;
        }

        gameObject.SetActive(false);
    }

    //Disables colliders on bullet
    public void DisableAllColliders()
    {
        colliderContainer.gameObject.SetActive(false);
    }

    //Returns prefab of bullet
    public static GameObject GetBulletType(BulletType type)
    {
        return GameMaster.gameMaster.bulletSpawns[(int)type - 1];
    }

    //Check if should use anim or sprite
    public bool CheckForAnimatedType(bool custom)
    {
        if (spriteType == BulletType.Custom)
            return custom;

        return (spriteType > BulletType.Giant);
    }

    //Updates sprite by checking if it's animated, and then changing either the Sprite or RuntimeAnimatorController
    public void UpdateSprite()
    {
        if (!CheckForAnimatedType(animatedSprite))
        {
            spriteRenderer.sprite = GetBulletSprite(spriteType, bulletColor, customSprite);
        }
        else
        {
            spriteAnimator.runtimeAnimatorController = GetBulletAnim(spriteType, bulletColor, customAnim);
        }
    }

    //Gets a bullet sprite by getting the index of the type by the list of the color -- or uses a custom sprite that is set
    public static Sprite GetBulletSprite(BulletType type, BulletColor color, Sprite customSprite)
    {
        if (type == BulletType.Custom)
            return customSprite;
        else
        {
            int thisType = (int)type - 1;
            switch (color)
            {
                case BulletColor.White:
                    return GameMaster.gameMaster.whiteBullets[thisType];
                case BulletColor.WhiteInverted:
                    return GameMaster.gameMaster.whiteInvertedBullets[thisType];
                case BulletColor.Blue:
                    return GameMaster.gameMaster.blueBullets[thisType];
                case BulletColor.BlueInverted:
                    return GameMaster.gameMaster.blueInvertedBullets[thisType];
                case BulletColor.Orange:
                    return GameMaster.gameMaster.orangeBullets[thisType];
                case BulletColor.OrangeInverted:
                    return GameMaster.gameMaster.orangeInvertedBullets[thisType];
                default:
                    return GameMaster.gameMaster.whiteBullets[thisType];
            }
        }
    }

    //Gets a RuntimeAnimatorController by getting the index of the type by the list of the color -- or uses a custom animator that is set
    public static RuntimeAnimatorController GetBulletAnim(BulletType type, BulletColor color, RuntimeAnimatorController customAnimController)
    {
        if (type == BulletType.Custom)
            return customAnimController;
        else
        {
            int thisType = (int)type - (int)BulletType.Butterfly;
            switch (color)
            {
                case BulletColor.White:
                    return GameMaster.gameMaster.whiteBulletAnims[thisType];
                case BulletColor.WhiteInverted:
                    return GameMaster.gameMaster.whiteInvertedBulletAnims[thisType];
                case BulletColor.Blue:
                    return GameMaster.gameMaster.blueBulletAnims[thisType];
                case BulletColor.BlueInverted:
                    return GameMaster.gameMaster.blueInvertedBulletAnims[thisType];
                case BulletColor.Orange:
                    return GameMaster.gameMaster.orangeBulletAnims[thisType];
                case BulletColor.OrangeInverted:
                    return GameMaster.gameMaster.orangeInvertedBulletAnims[thisType];
                default:
                    return GameMaster.gameMaster.whiteBulletAnims[thisType];
            }
        }
    }

    //Updates bullet random seed
    public static void UpdateSeed(int newSeed)
    {
        bulletRandom = new System.Random(newSeed);
        Debug.Log("Bullets' Random is set to " + newSeed);
    }
    //Gets random int from bullet Random
    public static int Random(int lower, int upper)
    {return bulletRandom.Next(lower, upper);}
    //Gets random float from bullet Random
    public static float Random(float lower, float upper)
    {return (((float)bulletRandom.NextDouble() * Mathf.Abs(lower - upper)) + lower);}

    //Displays bullet's direction when selected
    public void OnDrawGizmosSelected()
    {
        if (thisTrans != null)
        {
            Gizmos.DrawRay(thisTrans.position, movementDirection * 25);
        }
        else
        {
            Gizmos.DrawRay(transform.position, movementDirection * currentSpeed);
        }
    }
}
