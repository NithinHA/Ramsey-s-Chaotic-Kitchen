using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaypointEditor
{
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        Gizmos.color = (gizmoType & GizmoType.NotInSelectionHierarchy) != 0 ? Color.yellow : Color.yellow * .5f;
        Gizmos.DrawSphere(waypoint.transform.position, .5f);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(waypoint.transform.position + (waypoint.transform.right * waypoint.Width / 2f), 
            waypoint.transform.position - (waypoint.transform.right * waypoint.Width / 2f));

        if(waypoint.Previous != null)
        {
            Gizmos.color = Color.red;
            Vector3 offset = waypoint.transform.right * waypoint.Width / 2f;
            Vector3 offsetTo = waypoint.Previous.transform.right * waypoint.Previous.Width / 2f;
            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.Previous.transform.position + offsetTo);
        }
        if(waypoint.Next != null)
        {
            Gizmos.color = Color.green;
            Vector3 offset = -waypoint.transform.right * waypoint.Width / 2f;
            Vector3 offsetTo = -waypoint.Next.transform.right * waypoint.Next.Width / 2f;
            Gizmos.DrawLine(waypoint.transform.position + offset, waypoint.Next.transform.position + offsetTo);
        }
    }
}
