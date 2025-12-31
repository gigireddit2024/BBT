using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PearlController : MonoBehaviour, IPearl
{
    [Header("Movement (must implement IPearlMovement)")]
    [SerializeField] private MonoBehaviour movementBehaviour; // assign RandomBounceMovement2D

    private Rigidbody2D rb;
    private IPearlMovement movement;
    private bool initialized;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = movementBehaviour as IPearlMovement;

        if (movement == null)
        {
            Debug.LogError("movementBehaviour must implement IPearlMovement.", this);
        }
    }

    private void OnEnable()
    {
        if (!initialized)
        {
            Initialize(Vector2.zero); // velocity decided by movement strategy
        }

        PearlEvents.RaisePearlSpawned(this);
    }

    private void FixedUpdate()
    {
        movement?.Tick(Time.fixedDeltaTime);
    }

    public void Initialize(Vector2 initialVelocity)
    {
        // Keep IPearl.Initialize for your assignment interface, but delegate actual movement.
        movement?.Initialize(rb);
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
