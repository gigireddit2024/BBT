using UnityEngine;

[DisallowMultipleComponent]
public class RandomBounceMovement2D : MonoBehaviour, IPearlMovement
{
    [SerializeField] private float speed = 10f;
    [SerializeField, Range(0f, 45f)] private float randomBounceAngleDegrees = 12f;
    [SerializeField, Range(0f, 0.05f)] private float collisionNudge = 0.01f;

    private Rigidbody2D rb;
    private bool initialized;

    public void Initialize(Rigidbody2D rb)
    {
        this.rb = rb;

        this.rb.gravityScale = 0f;
        this.rb.linearDamping = 0f;
        this.rb.angularDamping = 0f;
        this.rb.freezeRotation = true;

        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

        this.rb.linearVelocity = dir * speed;
        initialized = true;
    }

    public void Tick(float fixedDeltaTime)
    {
        if (!initialized || rb == null) return;

        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude < 0.0001f)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.up;
            rb.linearVelocity = dir * speed;
            return;
        }

        rb.linearVelocity = v.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!initialized || rb == null) return;
        if (collision.contactCount == 0) return;

        Vector2 v = rb.linearVelocity;

        // Blend normals (good for corners)
        Vector2 n = Vector2.zero;
        for (int i = 0; i < collision.contactCount; i++)
            n += collision.GetContact(i).normal;

        if (n.sqrMagnitude < 0.0001f) return;
        n.Normalize();

        Vector2 reflected = Vector2.Reflect(v.normalized, n);

        float angle = Random.Range(-randomBounceAngleDegrees, randomBounceAngleDegrees);
        Vector2 randomized = (Quaternion.Euler(0f, 0f, angle) * reflected).normalized;

        rb.linearVelocity = randomized * speed;

        // Nudge OUT of the surface/corner after setting velocity
        rb.position += n * 0.005f;
    }
}
