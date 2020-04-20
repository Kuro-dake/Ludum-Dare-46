using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    public Vector3 offset;
    public Transform track;
    // Update is called once per frame
    void Update()
    {
        transform.position = track.position + offset;
    }
}
