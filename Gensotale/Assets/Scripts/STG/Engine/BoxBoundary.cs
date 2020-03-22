using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxBoundary : MonoBehaviour
{
    public static LinkedList<BoxBoundary> colliders = new LinkedList<BoxBoundary>();
    
    [HideInInspector] public RectTransform thisTrans;

    private void Awake()
    {
        thisTrans = GetComponent<RectTransform>();
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
