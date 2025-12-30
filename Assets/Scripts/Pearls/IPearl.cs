using UnityEngine;

public interface IPearl
{
    void Initialize(Vector2 initialVelocity);
    void Pop(); // what happens when the pearl is removed / tapped
}
