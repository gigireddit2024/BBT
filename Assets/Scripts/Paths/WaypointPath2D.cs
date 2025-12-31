using System.Collections.Generic;
using UnityEngine;

public class WaypointPath2D : MonoBehaviour, IPath2D
{
    [SerializeField] private Transform[] waypoints;

    private readonly List<float> cumulative = new();
    private float totalLength;

    public float Length => totalLength;

    private void Awake()
    {
        Rebuild();
    }

    private void OnValidate()
    {
        if (Application.isPlaying) return;
        Rebuild();
    }

    private void Rebuild()
    {
        cumulative.Clear();
        totalLength = 0f;

        if (waypoints == null || waypoints.Length < 2)
            return;

        cumulative.Add(0f);

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            float seg = Vector2.Distance(waypoints[i].position, waypoints[i + 1].position);
            totalLength += seg;
            cumulative.Add(totalLength);
        }
    }

    public Vector2 EvaluatePosition(float distance)
    {
        if (waypoints == null || waypoints.Length == 0)
            return transform.position;

        if (waypoints.Length == 1)
            return waypoints[0].position;

        distance = Mathf.Clamp(distance, 0f, totalLength);

        // find segment containing distance
        int segIndex = 0;
        for (int i = 0; i < cumulative.Count - 1; i++)
        {
            if (distance >= cumulative[i] && distance <= cumulative[i + 1])
            {
                segIndex = i;
                break;
            }
        }

        float segStart = cumulative[segIndex];
        float segEnd = cumulative[segIndex + 1];
        float segLen = Mathf.Max(0.0001f, segEnd - segStart);

        float t = (distance - segStart) / segLen;

        Vector2 a = waypoints[segIndex].position;
        Vector2 b = waypoints[segIndex + 1].position;

        return Vector2.Lerp(a, b, t);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        Gizmos.color = Color.white;
        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null) continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
#endif
}
