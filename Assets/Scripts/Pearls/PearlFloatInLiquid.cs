using UnityEngine;

/// <summary>
/// Makes a pearl gently float and drift, like in liquid.
/// No random jitters; motion is stable until an external force disturbs it.
/// SRP: only handles floaty movement.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PearlFloatInLiquid : MonoBehaviour
{
    [Header("Buoyancy")]
    [SerializeField] private float upwardForce = 0.4f;     // gentle upward push
    [SerializeField] private float maxVerticalSpeed = 1.2f;

    [Header("Horizontal Drift")]
    [SerializeField] private float horizontalForce = 0.3f; // subtle sideways drift
    [SerializeField] private float maxHorizontalSpeed = 1.2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        ApplyBuoyancy();
        ApplyHorizontalDrift();
        ClampVelocity();
    }

    private void ApplyBuoyancy()
    {
        // Always a small upward force
        rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Force);
    }

    private void ApplyHorizontalDrift()
    {
        // Use Perlin noise for gentle sideways liquid movement
        float side = Mathf.PerlinNoise(Time.time * 0.15f, transform.position.y) - 0.5f;
        rb.AddForce(new Vector2(side * horizontalForce, 0f), ForceMode2D.Force);
    }

    private void ClampVelocity()
    {
        Vector2 v = rb.linearVelocity;

        // Limit vertical drift
        v.y = Mathf.Clamp(v.y, -maxVerticalSpeed, maxVerticalSpeed);

        // Limit sideways drift
        v.x = Mathf.Clamp(v.x, -maxHorizontalSpeed, maxHorizontalSpeed);

        rb.linearVelocity = v;
    }
}
