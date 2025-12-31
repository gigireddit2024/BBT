using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PearlView : MonoBehaviour, IPearl
{
    public float Distance { get; private set; }

    public void SetDistance(float distanceAlongPath)
    {
        Distance = distanceAlongPath;
    }

    public void Pop()
    {
        PearlEvents.RaisePearlPopped(this);
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        Pop();
    }

    private void OnEnable()
    {
        PearlEvents.RaisePearlSpawned(this);
    }
}
