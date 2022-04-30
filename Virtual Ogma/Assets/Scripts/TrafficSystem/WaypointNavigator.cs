using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNavigator : MonoBehaviour
{
    private CarController _controller;
    public Waypoint CurrentWaypoint;

    private void Awake()
    {
        _controller = GetComponent<CarController>();
    }
    void Start()
    {
        _controller.SetDestination(CurrentWaypoint.GetPosition());
    }

    void Update()
    {
        if (_controller.ReachedDestination)
        {
            CurrentWaypoint = CurrentWaypoint.Next;
            _controller.SetDestination(CurrentWaypoint.GetPosition());
        }
    }
}
