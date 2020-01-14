using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomMaster : MonoBehaviour
{
    public static RoomMaster roomMaster;

    GameMaster gameMaster;

    public Vector2 cameraPositionBoundsMin;
    public Vector2 cameraPositionBoundsMax;
    public Tilemap[] collidingTiles;
    [HideInInspector] public Transform[] tileColliderContainers;
    public LoadingZone[] doors;

    Transform camTrans;
    public Transform playerTrans;

    private void Awake()
    {
        gameMaster = GameMaster.gameMaster;
        roomMaster = this;
        GetReferences();
        EnterDoor();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CamFollowPlayer();
    }

    void GetReferences()
    {
        camTrans = Camera.main.transform;
        playerTrans = GameObject.Find("RenkoOverworld").transform;
        tileColliderContainers = new Transform[collidingTiles.Length];
        for (int i = 0; i < tileColliderContainers.Length; i++)
        {
            tileColliderContainers[i] = new GameObject("TileCollider " + i.ToString()).transform;
            tileColliderContainers[i].parent = collidingTiles[i].transform;
        }
    }

    void CamFollowPlayer()
    {
        camTrans.position = new Vector3(Mathf.Clamp(playerTrans.position.x, cameraPositionBoundsMin.x, cameraPositionBoundsMax.x),
            Mathf.Clamp(playerTrans.position.y, cameraPositionBoundsMin.y, cameraPositionBoundsMax.y), camTrans.position.z);
    }

    void EnterDoor()
    {
        if (gameMaster.enteringDoor < 0 || gameMaster.enteringDoor > doors.Length - 1)
            return;

        playerTrans.position = (Vector2)doors[gameMaster.enteringDoor].thisTrans.position + doors[gameMaster.enteringDoor].playerSpawnOffset;
    }
}
