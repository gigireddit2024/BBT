using UnityEngine;

public interface IPath2D
{
    float Length { get; }
    Vector2 EvaluatePosition(float distance);
}
