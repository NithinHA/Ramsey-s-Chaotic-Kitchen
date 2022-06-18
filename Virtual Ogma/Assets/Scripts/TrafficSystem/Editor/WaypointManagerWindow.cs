using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WaypointManagerWindow : EditorWindow
{
    public Transform WaypointRoot;

    [MenuItem("Tools/Waypoint Editor")]
    public static void Open()
    {
        GetWindow<WaypointManagerWindow>();
    }

    private void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);
        EditorGUILayout.PropertyField(obj.FindProperty("WaypointRoot"));
        if (WaypointRoot == null)
            EditorGUILayout.HelpBox("Root for waypoints must be assigned.", MessageType.Warning);
        else
        {
            EditorGUILayout.BeginVertical("Box");
            DrawButtons();
            EditorGUILayout.EndVertical();
        }

        obj.ApplyModifiedProperties();
    }

    private void DrawButtons()
    {
        if(GUILayout.Button("Create Waypoint"))
        {
            CreateWaypoint();
        }

        if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Waypoint>())
        {
            if(GUILayout.Button("Create Waypoint Before"))
            {
                CreateWaypointBefore();
            }
            if(GUILayout.Button("Create Waypoint After"))
            {
                CreateWaypointAfter();
            }
            if(GUILayout.Button("Remove Waypoint"))
            {
                RemoveWaypoint();
            }
            if(GUILayout.Button("Add Branch"))
            {
                CreateBranch();
            }
        }
    }

    private void CreateWaypoint()
    {
        Waypoint waypoint;
        if(WaypointRoot.childCount > 1)
        {
            Waypoint previous = WaypointRoot.GetChild(WaypointRoot.childCount - 2).GetComponent<Waypoint>();
            waypoint = SetupWaypoint("waypoint_" + WaypointRoot.childCount, previous.transform.position, previous.transform.forward);
            waypoint.Previous = previous;
            previous.Next = waypoint;
        }
        else
        {
            waypoint = SetupWaypoint("waypoint_" + WaypointRoot.childCount, WaypointRoot.position, WaypointRoot.forward);
        }

        Selection.activeGameObject = waypoint.gameObject;
    }

    private void CreateWaypointBefore()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
        Waypoint waypoint = SetupWaypoint(selectedWaypoint.name, selectedWaypoint.transform.position, selectedWaypoint.transform.forward);
        waypoint.Next = selectedWaypoint;
        if (selectedWaypoint.Previous != null)
        {
            waypoint.Previous = selectedWaypoint.Previous;
            selectedWaypoint.Previous.Next = waypoint;
        }
        selectedWaypoint.Previous = waypoint;

        ShiftWaypoints(waypoint.Next);
        Selection.activeGameObject = waypoint.gameObject;
    }

    private void CreateWaypointAfter()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
        Waypoint waypoint = SetupWaypoint(selectedWaypoint.Next != null ? selectedWaypoint.Next.name : "waypoint_" + WaypointRoot.childCount, selectedWaypoint.transform.position, selectedWaypoint.transform.forward);
        waypoint.Next = selectedWaypoint.Next;
        waypoint.Previous = selectedWaypoint;
        if (selectedWaypoint.Next != null)
            selectedWaypoint.Next.Previous = waypoint;
        selectedWaypoint.Next = waypoint;

        ShiftWaypoints(waypoint.Next);
        Selection.activeGameObject = waypoint.gameObject;
    }

    private void RemoveWaypoint()
    {
        Waypoint selectedWaypoint = Selection.activeGameObject.GetComponent<Waypoint>();
        ShiftWaypoints(selectedWaypoint, false);
        if (selectedWaypoint.Previous != null)
            selectedWaypoint.Previous.Next = selectedWaypoint.Next;
        if (selectedWaypoint.Next != null)
            selectedWaypoint.Next.Previous = selectedWaypoint.Previous;

        DestroyImmediate(selectedWaypoint .gameObject);
    }

    private void CreateBranch()
    {
        Waypoint branchedFrom = Selection.activeGameObject.GetComponent<Waypoint>();
        Waypoint waypoint = SetupWaypoint("waypoint_" + WaypointRoot.childCount, branchedFrom.transform.position, branchedFrom.transform.forward);
        branchedFrom.Branches.Add(waypoint);
        Selection.activeGameObject = waypoint.gameObject;
    }

    private Waypoint SetupWaypoint(string name, Vector3 position, Vector3 forward)
    {
        Waypoint waypoint = new GameObject(name, typeof(Waypoint)).GetComponent<Waypoint>();
        waypoint.transform.SetParent(WaypointRoot);
        waypoint.transform.position = position;
        waypoint.transform.forward = forward;
        return waypoint;
    }

    private void ShiftWaypoints(Waypoint waypoint, bool isLeftShift = true)
    {
        if (waypoint == null)
            return;

        if (isLeftShift)
        {
            while (waypoint != null)
            {
                waypoint.name = waypoint.Next != null ? waypoint.Next.name : "waypoint_" + (WaypointRoot.childCount - 1);
                waypoint = waypoint.Next;
            }
        }
        else
        {
            var lastWaypoint = waypoint;
            while (lastWaypoint.Next != null)
                lastWaypoint = lastWaypoint.Next;
            while(lastWaypoint != waypoint.Previous)
            {
                lastWaypoint.name = lastWaypoint.Previous != null ? lastWaypoint.Previous.name : "waypoint_0";
                lastWaypoint = lastWaypoint.Previous;
            }
        }


    }
}
