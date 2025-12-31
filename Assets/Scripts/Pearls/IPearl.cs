public interface IPearl
{
    void SetDistance(float distanceAlongPath);
    float Distance { get; }

    void Pop();
}
