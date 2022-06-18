using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    private CarController _controller;
    public Waypoint CurrentWaypoint;
    [Header("Random waypoint selection")]
    public bool IsRandomWaypoint;
    public Transform RootGO;

    private void Awake()
    {
        _controller = GetComponent<CarController>();
        if(IsRandomWaypoint)
            CurrentWaypoint = RootGO.GetChild(Random.Range(0, RootGO.childCount - 1)).GetComponent<Waypoint>();
    }
    void Start()
    {
        _controller.SetDestination(CurrentWaypoint.GetPosition());
    }

    void Update()
    {
        if (_controller.ReachedDestination)
        {
            bool shouldBranch = false;
            if(CurrentWaypoint.Branches != null && CurrentWaypoint.Branches.Count > 0)
                shouldBranch = Random.Range(0f, 1f) < CurrentWaypoint.BranchRatio;
            
            if(shouldBranch)
                CurrentWaypoint = CurrentWaypoint.Branches[Random.Range(0, CurrentWaypoint.Branches.Count - 1)];
            else
            {
                if(CurrentWaypoint.Next != null)
                    CurrentWaypoint = CurrentWaypoint.Next;
                else
                {
                    Debug.LogError(gameObject.name + " has reached a terminal waypoint!");
                    return;
                }
            }

            _controller.SetDestination(CurrentWaypoint.GetPosition());
        }
    }
}
