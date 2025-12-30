using UnityEngine;

public class PearlSpawner : MonoBehaviour, IPearlSpawner
{
    [SerializeField] private PearlController pearlPrefab;
    [SerializeField] private Collider2D spawnArea; // Assign in Inspector
    [SerializeField] private float spawnPadding = 0.2f; // keeps pearls from clipping edges

    public void SpawnPearls(int count)
    {
        if (pearlPrefab == null || spawnArea == null)
            return;

        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetRandomPointInsideCollider(spawnArea);

            // Optional: small offset so pearls never spawn intersecting walls
            pos += Random.insideUnitCircle.normalized * spawnPadding;

            Instantiate(pearlPrefab, pos, Quaternion.identity);
        }
    }

    private Vector2 GetRandomPointInsideCollider(Collider2D collider)
    {
        Vector2 point;

        // Repeat until point is inside collider (robust for all collider shapes)
        do
        {
            Vector2 min = collider.bounds.min;
            Vector2 max = collider.bounds.max;

            point = new Vector2(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y)
            );
        }
        while (!collider.OverlapPoint(point));

        return point;
    }
}
