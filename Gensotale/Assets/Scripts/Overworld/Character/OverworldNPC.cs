using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(BaseInteractable))]
public class OverworldNPC : MonoBehaviour
{
    RoomMaster roomMaster;
    GameMaster gameMaster;

    public bool unmoving;
    public bool interacting;
    BaseInteractable interactionScript;

    public float maxDistFromStart;
    public Vector2 moveTimeConstraints;
    public Vector2 moveDelayConstraints;
    public float moveSpeed;

    Animator anim;
    
    int facing;
    float moveTime;
    float moveDelay;

    Vector2 direction;
    Vector2 startPos;
    Transform thisTrans;

    // Start is called before the first frame update
    void Start()
    {
        thisTrans = transform;
        anim = thisTrans.GetComponentInChildren<Animator>();
        interactionScript = GetComponent<BaseInteractable>();
        roomMaster = RoomMaster.roomMaster;
        gameMaster = GameMaster.gameMaster;

        startPos = thisTrans.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!interacting && !unmoving)
            Movement();

        interacting = interactionScript.triggered;
        if (interacting)
            FacePlayer();

        UpdateAnim();
    }

    void FacePlayer()
    {
        moveTime = 0;
        float angle = MathFunctions.FindAngle(thisTrans.position, roomMaster.playerTrans.position);
        facing = Mathf.RoundToInt(Mathf.Repeat(angle, 360) / 90f);
    }

    void Movement()
    {
        if(moveTime > 0)
        {
            if (Vector2.Distance(thisTrans.position, startPos) < maxDistFromStart ||
                Mathf.Abs(MathFunctions.FindAngle(direction) - MathFunctions.FindAngle(thisTrans.position, startPos)) < 130f)
            {
                thisTrans.position += (Vector3)(direction * moveSpeed * gameMaster.timeScale * gameMaster.frameTime);
            }
            else
                moveTime = 0;
        }

        moveTime -= gameMaster.timeScale * gameMaster.frameTime;
        if(moveTime <= 0 && direction.sqrMagnitude > 0)
        {
            moveDelay = GameMaster.Random(moveDelayConstraints);
            direction = Vector2.zero;
        }
        moveDelay -= gameMaster.timeScale * gameMaster.frameTime;
        if (moveDelay <= 0 && moveTime <= 0)
        {
            moveTime = GameMaster.Random(moveTimeConstraints);
            direction = MathFunctions.CalculateCircle(MathFunctions.FindAngle(thisTrans.position, startPos) + GameMaster.Random(-60f, 60f));
        }
    }


    void UpdateAnim()
    {
        if (moveTime > 0)
            facing = Mathf.RoundToInt(Mathf.Repeat(MathFunctions.FindAngle(direction), 360) / 90f);
        anim.SetInteger("Facing", facing);
        anim.SetBool("Moving", moveTime > 0);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, .5f);

        Gizmos.DrawWireSphere(Application.isPlaying ? startPos : (Vector2)transform.position, maxDistFromStart);
    }
}
