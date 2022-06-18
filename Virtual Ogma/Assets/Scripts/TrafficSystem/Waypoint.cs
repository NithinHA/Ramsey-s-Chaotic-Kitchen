using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint Previous;
    public Waypoint Next;
    [Range(0f, 8f)] public float Width = 5f;

    public List<Waypoint> Branches = new List<Waypoint>();
    [Range(0, 1)] public float BranchRatio = .5f;

    public Vector3 GetPosition()
    {
        Vector3 minBound = transform.position + transform.right * Width / 2f;
        Vector3 maxBound = transform.position - transform.right * Width / 2f;
        return Vector3.Lerp(minBound, maxBound, Random.Range(0f, 1f));
    }

}
