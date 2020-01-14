using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingZone : MonoBehaviour
{
    RoomMaster roomMaster;

    public int doorIndex;
    public string sceneName;

    public Vector2 extents;
    public Vector2 playerSpawnOffset;
    [HideInInspector] public Transform thisTrans;

    private void Awake()
    {
        thisTrans = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        roomMaster = RoomMaster.roomMaster;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 distance = new Vector2(Mathf.Abs(roomMaster.playerTrans.position.x - thisTrans.position.x),
            Mathf.Abs(roomMaster.playerTrans.position.y - thisTrans.position.y));
        distance -= extents;
        if (distance.x <= 0 && distance.y <= 0)
            GameMaster.gameMaster.LoadScene(sceneName, false, doorIndex, GameMaster.SceneType.Overworld);

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, .1f);
        Gizmos.DrawCube(transform.position, extents * 2 * transform.lossyScale);

        Gizmos.color = new Color(0, 1, 0, .25f);
        Gizmos.DrawSphere(transform.position + (Vector3)playerSpawnOffset, 5);
    }
}
