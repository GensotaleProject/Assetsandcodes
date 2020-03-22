using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageCollider : MonoBehaviour
{
    public static LinkedList<PlayerDamageCollider> colliders = new LinkedList<PlayerDamageCollider>();
    public Transform colliderTransform;
    BaseBulletScript bulletScript;
    public float hitboxScale;
    public Vector2 hitboxOffset;

    private void Awake()
    {
        colliderTransform = transform;
        bulletScript = colliderTransform.GetComponent<BaseBulletScript>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawWireSphere(transform.position + (Vector3)(hitboxOffset * transform.lossyScale), hitboxScale * transform.lossyScale.x);
    }

    private void OnEnable()
    {colliders.AddLast(this);}

    private void OnDestroy()
    {colliders.Remove(this);}

    private void OnDisable()
    {colliders.Remove(this);}
}
