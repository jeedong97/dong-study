using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Parallax : MonoBehaviour
{
    public CardDrag CD;
    public CardEditModeHandler CardEdit;
    public Transform Target;
    public bool isParallax = false;

    public float smoothSpeed = 0.125f;
    // public Vector3 Offset;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isParallax)
        {
            Vector3 desiredPosition = Target.position;
            Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothPosition;
        }
    }
}
