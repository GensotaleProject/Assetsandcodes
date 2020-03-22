using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STGPlayer : MonoBehaviour
{
    const float DIST_FROM_EDGE = 5;

    public float hitboxSize, interactSize, moveSpeed, focusMultiplier;
    public SpriteRenderer playerRenderer, hitboxRenderer, focusRenderer;

    Vector2 velocity;
    Transform thisTrans;
    InputScript input;
    GameMaster gameMaster;
    BoxBoundary currentBoundary;

    bool prevFocus;
    private void Start()
    {
        gameMaster = GameMaster.gameMaster;
        input = InputScript.inputScript;
        thisTrans = transform;
        playerRenderer = thisTrans.Find("PlayerSprite").GetComponent<SpriteRenderer>();
        hitboxRenderer = thisTrans.Find("HitboxSprite").GetComponent<SpriteRenderer>();
        focusRenderer = thisTrans.Find("FocusSprite").GetComponent<SpriteRenderer>();
        focusRenderer.enabled = input.focusPressed;
        hitboxRenderer.enabled = input.focusPressed;
    }

    private void Update()
    {
        Vector2 inputSigns = new Vector2(input.directionalInput.x.CompareTo(0) != 0 ? Mathf.Sign(input.directionalInput.x) : 0,
            input.directionalInput.y.CompareTo(0) != 0 ? Mathf.Sign(input.directionalInput.y) : 0);

        Movement(inputSigns);

        prevFocus = input.focusPressed;
    }

    void Movement(Vector2 inputSigns)
    {
        velocity = inputSigns.normalized * moveSpeed * (input.focusPressed ? focusMultiplier : 1) * gameMaster.timeScale;

        if (prevFocus != input.focusPressed)
        {
            focusRenderer.enabled = input.focusPressed;
            hitboxRenderer.enabled = input.focusPressed;
        }
        
        thisTrans.position += (Vector3)velocity;
        Collisions();
    }

    void Collisions()
    {
        int colliderCount = BoxBoundary.colliders.Count;
        Vector2 playerPos = thisTrans.position, colliderPos, colliderBounds;
        if (colliderCount > 1)
        {
            float shortestDist = float.MaxValue, dist;
            LinkedListNode<BoxBoundary> node = BoxBoundary.colliders.First;
            BoxBoundary thisBoundary, nearest = null;
            for (int i = 0; i < colliderCount; i++)
            {
                thisBoundary = node.Value;
                colliderPos = thisBoundary.thisTrans.position;
                colliderBounds = thisBoundary.thisTrans.sizeDelta;
                if (Mathf.Abs(playerPos.x - colliderPos.x) - colliderBounds.x / 2 < 0 && Mathf.Abs(playerPos.y - colliderPos.y) - colliderBounds.y / 2 < 0)
                {
                    dist = Vector2.Distance(playerPos, colliderPos);
                    if (dist < shortestDist)
                    {
                        nearest = thisBoundary;
                        shortestDist = dist;
                    }
                }
                node = node.Next;
            }
            if (nearest == null)
                nearest = BoxBoundary.colliders.First.Value;
            currentBoundary = nearest;
        }
        else if (currentBoundary == null)
            currentBoundary = BoxBoundary.colliders.First.Value;

        colliderPos = currentBoundary.thisTrans.position;
        colliderBounds = currentBoundary.thisTrans.sizeDelta;

        /*if (Mathf.Abs((playerPos.x + velocity.x) - colliderPos.x) - (colliderBounds.x / 2 + DIST_FROM_EDGE) >= 0)
            velocity.x = Mathf.Abs(playerPos.x - (colliderBounds.x / 2 + DIST_FROM_EDGE)) * Mathf.Sign(velocity.x);

        if (Mathf.Abs((playerPos.y + velocity.y) - colliderPos.y) - (colliderBounds.y / 2 + DIST_FROM_EDGE) >= 0)
            velocity.y = Mathf.Abs(playerPos.y - (colliderBounds.y / 2 + DIST_FROM_EDGE)) * Mathf.Sign(velocity.y);*/

        float xEdge = colliderBounds.x / 2 - DIST_FROM_EDGE,
            yEdge = colliderBounds.y / 2 - DIST_FROM_EDGE;
        thisTrans.position = new Vector3(Mathf.Clamp(playerPos.x, colliderPos.x - xEdge, colliderPos.x + xEdge),
            Mathf.Clamp(playerPos.y, colliderPos.y - yEdge, colliderPos.y + yEdge), 0);

        playerPos = thisTrans.position;
        if (playerPos.x.CompareTo(colliderPos.x - xEdge) == 0 || playerPos.x.CompareTo(colliderPos.x + xEdge) == 0)
            velocity.x = 0;
        if (playerPos.y.CompareTo(colliderPos.y - yEdge) == 0 || playerPos.y.CompareTo(colliderPos.y + yEdge) == 0)
            velocity.y = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, hitboxSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactSize);
    }
}
