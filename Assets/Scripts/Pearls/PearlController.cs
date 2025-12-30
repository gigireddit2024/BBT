using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PearlController : MonoBehaviour, IPearl
{
    [SerializeField] private float initialSpeed = 1.5f; // much gentler than before

    private Rigidbody2D rb;
    private bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (!initialized)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Initialize(randomDir * initialSpeed);
        }

        PearlEvents.RaisePearlSpawned(this);
    }

    public void Initialize(Vector2 initialVelocity)
    {
        rb.linearVelocity = initialVelocity;
        initialized = true;
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
}
