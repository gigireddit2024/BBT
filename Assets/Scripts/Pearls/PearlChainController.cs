using System.Collections.Generic;
using UnityEngine;

public class PearlChainController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private MonoBehaviour pathBehaviour; // must implement IPath2D
    private IPath2D path;

    [Header("Chain Settings")]
    [SerializeField] private float speed = 2.0f;      // units per second along path
    [SerializeField] private float spacing = 0.6f;    // distance between pearls

    private readonly List<IPearl> pearls = new();
    private float headDistance;

    private void Awake()
    {
        path = pathBehaviour as IPath2D;
        if (path == null)
            Debug.LogError("pathBehaviour does not implement IPath2D.");
    }

    private void OnEnable()
    {
        PearlEvents.OnPearlPopped += HandlePearlPopped;
    }

    private void OnDisable()
    {
        PearlEvents.OnPearlPopped -= HandlePearlPopped;
    }

    private void Update()
    {
        if (path == null) return;
        if (pearls.Count == 0) return;

        headDistance += speed * Time.deltaTime;
        headDistance = WrapDistance(headDistance, path.Length);

        for (int i = 0; i < pearls.Count; i++)
        {
            float d = headDistance - i * spacing;
            d = WrapDistance(d, path.Length);

            pearls[i].SetDistance(d);

            if (pearls[i] is MonoBehaviour mb && mb != null)
                mb.transform.position = path.EvaluatePosition(d);
        }
    }

    private static float WrapDistance(float value, float length)
    {
        if (length <= 0.0001f) return 0f;
        float wrapped = value % length;
        return wrapped < 0f ? wrapped + length : wrapped;
    }


    public void AddPearlToTail(IPearl pearl)
    {
        if (pearl == null) return;

        pearls.Add(pearl);

        // Place it immediately (tail starts behind current head)
        float d = Mathf.Clamp(headDistance - (pearls.Count - 1) * spacing, 0f, path.Length);
        pearl.SetDistance(d);

        if (pearl is MonoBehaviour mb && mb != null && path != null)
            mb.transform.position = path.EvaluatePosition(d);
    }

    private void HandlePearlPopped(IPearl popped)
    {
        int index = pearls.IndexOf(popped);
        if (index < 0) return;

        pearls.RemoveAt(index);

        // Optional: immediately close the gap by shifting headDistance backward slightly
        // (Usually you just let the next Update naturally re-space the chain.)
    }
}
