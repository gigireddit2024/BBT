using System.Collections.Generic;
using UnityEngine;

public class LoopCatmullRomPath2D : MonoBehaviour, IPath2D
{
    [SerializeField] private Transform[] waypoints;

    [Header("Quality")]
    [SerializeField, Range(8, 200)]
    private int samplesPerSegment = 30;

    private readonly List<Vector2> sampledPoints = new();
    private readonly List<float> cumulative = new();
    private float totalLength;

    public float Length => totalLength;

    private void Awake() => Rebuild();

    private void OnValidate()
    {
        if (!Application.isPlaying) Rebuild();
    }

    public Vector2 EvaluatePosition(float distance)
    {
        if (sampledPoints.Count == 0) return transform.position;
        if (sampledPoints.Count == 1) return sampledPoints[0];

        float d = WrapDistance(distance, totalLength);

        // Find first cumulative index >= d
        int idx = 0;
        for (int i = 0; i < cumulative.Count; i++)
        {
            if (cumulative[i] >= d)
            {
                idx = i;
                break;
            }
        }

        if (idx == 0) return sampledPoints[0];

        float d0 = cumulative[idx - 1];
        float d1 = cumulative[idx];
        float t = (d - d0) / Mathf.Max(0.0001f, (d1 - d0));

        return Vector2.Lerp(sampledPoints[idx - 1], sampledPoints[idx], t);
    }

    private void Rebuild()
    {
        sampledPoints.Clear();
        cumulative.Clear();
        totalLength = 0f;

        if (waypoints == null || waypoints.Length < 3)
            return; // loop spline needs at least 3 points (4 is nicer)

        int n = waypoints.Length;

        // Each waypoint-to-next is a segment; for loop, last connects to first.
        for (int seg = 0; seg < n; seg++)
        {
            Vector2 p0 = GetWrappedPoint(seg - 1);
            Vector2 p1 = GetWrappedPoint(seg);
            Vector2 p2 = GetWrappedPoint(seg + 1);
            Vector2 p3 = GetWrappedPoint(seg + 2);

            for (int s = 0; s <= samplesPerSegment; s++)
            {
                float t = s / (float)samplesPerSegment;
                Vector2 pt = CatmullRom(p0, p1, p2, p3, t);

                if (sampledPoints.Count == 0 || (pt - sampledPoints[^1]).sqrMagnitude > 0.000001f)
                    sampledPoints.Add(pt);
            }
        }

        // Ensure loop closure in the sampled polyline (last connects back to first)
        if (sampledPoints.Count >= 2 &&
            (sampledPoints[0] - sampledPoints[^1]).sqrMagnitude > 0.000001f)
        {
            sampledPoints.Add(sampledPoints[0]);
        }

        cumulative.Add(0f);
        for (int i = 1; i < sampledPoints.Count; i++)
        {
            totalLength += Vector2.Distance(sampledPoints[i - 1], sampledPoints[i]);
            cumulative.Add(totalLength);
        }
    }

    private Vector2 GetWrappedPoint(int index)
    {
        int n = waypoints.Length;
        int wrapped = ((index % n) + n) % n;
        return waypoints[wrapped].position;
    }

    private static float WrapDistance(float value, float length)
    {
        if (length <= 0.0001f) return 0f;
        float wrapped = value % length;
        return wrapped < 0f ? wrapped + length : wrapped;
    }

    private static Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 3) return;

        Gizmos.color = Color.gray;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Transform next = waypoints[(i + 1) % waypoints.Length];
            if (next == null) continue;
            Gizmos.DrawLine(waypoints[i].position, next.position);
        }

        if (sampledPoints.Count < 2) return;
        Gizmos.color = Color.white;
        for (int i = 0; i < sampledPoints.Count - 1; i++)
        {
            Gizmos.DrawLine(sampledPoints[i], sampledPoints[i + 1]);
        }
    }
#endif
}
