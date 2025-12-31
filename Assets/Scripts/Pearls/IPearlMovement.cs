using UnityEngine;

public interface IPearlMovement
{
    void Initialize(Rigidbody2D rb);
    void Tick(float fixedDeltaTime);
}
