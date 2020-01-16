using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OverworldPlayer : MonoBehaviour
{
    GameMaster gameMaster;
    InputScript input;
    RoomMaster roomMaster;

    public float moveSpeed;
    public int colliderSpawnDist;
    public Vector2 colliderExtents;
    public Vector2 colliderOffset;

    Animator anim;
    Transform thisTrans;

    Vector2 directionInputs;
    public bool prevUp;
    public bool prevDown;
    public bool prevLeft;
    public bool prevRight;

    // Awake is called when the object is first created
    private void Awake()
    {
        thisTrans = transform;
        anim = thisTrans.GetChild(0).GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        input = InputScript.inputScript;
        roomMaster = RoomMaster.roomMaster;
        gameMaster = GameMaster.gameMaster;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameMaster.lockMovement)
            GetDirectionalInputs();
        else
            directionInputs = Vector2.zero;
        UpdateAnim();
        CreateColliders();
        Collisions();
        thisTrans.position += (Vector3)(directionInputs * moveSpeed * gameMaster.timeScale * gameMaster.frameTime);
    }

    void CreateColliders()
    {
        ClearColliders();
        Tilemap[] tilemaps = roomMaster.collidingTiles;
        for (int i = 0; i < tilemaps.Length; i++)
        {
            int playerX = Mathf.RoundToInt(thisTrans.position.x / tilemaps[i].cellSize.x);
            int playerY = Mathf.RoundToInt(thisTrans.position.y / tilemaps[i].cellSize.y);
            BoundsInt tilemapBounds = tilemaps[i].cellBounds;
            for (int x = playerX - colliderSpawnDist; x < playerX + colliderSpawnDist; x++)
            {
                Vector3Int thisPos = new Vector3Int(x, 0, 0);
                if (x < tilemapBounds.xMin * tilemaps[i].cellSize.x)
                    continue;
                else if (x > tilemapBounds.xMax * tilemaps[i].cellSize.x)
                    break;
                for (int y = playerY - colliderSpawnDist; y < playerY + colliderSpawnDist; y++)
                {
                    if (y < tilemapBounds.yMin)
                        continue;
                    else if (y > tilemapBounds.yMax)
                        break;

                    thisPos.y = y;
                    if(tilemaps[i].HasTile(thisPos))
                    {
                        SpawnCollider(tilemaps[i].CellToWorld(thisPos), tilemaps[i].cellSize, roomMaster.tileColliderContainers[i]);
                    }
                }
            }
        }
    }

    void SpawnCollider(Vector2 colPos, Vector2 colSize, Transform parent)
    {
        GameObject newCollider = new GameObject("NewCollider");
        newCollider.transform.parent = parent;
        newCollider.transform.position = colPos;
        newCollider.AddComponent<OverworldCollider>().extents = colSize / 2;
    }

    void ClearColliders()
    {
        Transform[] colParents = roomMaster.tileColliderContainers;
        int childSize;
        for (int i = 0; i < colParents.Length; i++)
        {
            childSize = colParents[i].childCount;
            for (int k = 0; k < childSize; k++)
            {
                Destroy(colParents[i].GetChild(k).gameObject);
            }
        }
    }

    void Collisions()
    {
        int colliderCount = OverworldCollider.colliders.Count;
        LinkedListNode<OverworldCollider> tileNode = OverworldCollider.colliders.First;
        OverworldCollider platform;
        Vector2 worldPosCenter;
        Vector2 scale;

        Vector2 thisScale = thisTrans.localScale;
        Vector2 thisColPos = (Vector2)thisTrans.position + (directionInputs * moveSpeed * (1f / 60f)) + (colliderOffset * thisScale);
        Vector2 thisColSize = colliderExtents * thisScale;

        Vector2 distance;
        for (int i = 0; i < colliderCount; i++)
        {
            platform = tileNode.Value;

            if (platform.thisTrans == thisTrans)
            {
                tileNode = tileNode.Next;
                continue;
            }

            scale = platform.thisTrans.lossyScale;
            worldPosCenter = platform.thisTrans.position + (Vector3)(platform.offset * scale);

            distance.x = Mathf.Abs(thisColPos.x - worldPosCenter.x) - thisColSize.x - (platform.extents.x * scale.x);
            distance.y = Mathf.Abs(thisColPos.y - worldPosCenter.y) - (thisColSize.y + (platform.extents.y * scale.y));

            if (distance.x < 0 && distance.y < 0)
            {
                if (distance.x < distance.y)
                {
                    directionInputs.y = 0;
                    if (thisColPos.y > worldPosCenter.y)
                    {
                        thisTrans.position = new Vector3(thisTrans.position.x, (worldPosCenter.y + thisColSize.y) + (platform.extents.y * scale.y), 0);
                    }
                    else
                        thisTrans.position = new Vector3(thisTrans.position.x, (worldPosCenter.y - thisColSize.y) - (platform.extents.y * scale.y), 0);
                }
                else
                {
                    directionInputs.x = 0;
                    if (thisColPos.x > worldPosCenter.x)
                        thisTrans.position = new Vector3((worldPosCenter.x + thisColSize.x) + (platform.extents.x * scale.x), thisTrans.position.y, 0);
                    else
                        thisTrans.position = new Vector3((worldPosCenter.x - thisColSize.x) - (platform.extents.x * scale.x), thisTrans.position.y, 0);

                }
            }

            tileNode = tileNode.Next;
        }
    }

    void GetDirectionalInputs()
    {
        directionInputs.x = input.directionalInput.x > 0.01f ? 1 : (input.directionalInput.x < -0.01f ? -1 : 0);
        directionInputs.y = input.directionalInput.y > 0.01f ? 1 : (input.directionalInput.y < -0.01f ? -1 : 0);
        directionInputs.Normalize();
    }

    void UpdateAnim()
    {
        bool prevMoving = anim.GetBool("Moving");
        bool moving = directionInputs.sqrMagnitude > 0.01f;
        anim.SetBool("Moving", moving);

        int facing = directionInputs.y > 0 ? 0 : (directionInputs.x > 0 ? 1 : (directionInputs.y < 0 ? 2 : 3));
        bool changeFacing = moving && !prevMoving;

        if(moving && !changeFacing)
        {
            switch (anim.GetInteger("Facing"))
            {
                case 0:
                    changeFacing = directionInputs.y <= 0;
                    break;
                case 1:
                    changeFacing = directionInputs.x <= 0;
                    break;
                case 2:
                    changeFacing = directionInputs.y >= 0;
                    break;
                case 3:
                    changeFacing = directionInputs.x >= 0;
                    break;
            }
        }

        if (changeFacing)
        {
            anim.SetInteger("Facing", facing);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 0, 1, .5f);
        Gizmos.DrawWireCube(transform.position + (Vector3)(colliderOffset * transform.lossyScale), colliderExtents * 2 * transform.lossyScale);
    }
}
