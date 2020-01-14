using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCollider : MonoBehaviour
{
    public static LinkedList<OverworldCollider> colliders = new LinkedList<OverworldCollider>();

    public Vector2 extents;
    public Vector2 offset;
    [HideInInspector] public Transform thisTrans;

    private void Awake()
    {
        thisTrans = transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawWireCube(transform.position + (Vector3)(offset * transform.lossyScale), extents * 2 * transform.lossyScale);
    }

    private void OnEnable()
    {
        colliders.AddLast(this);
    }

    private void OnDestroy()
    {
        colliders.Remove(this);
    }

    private void OnDisable()
    {
        colliders.Remove(this);
    }
}
