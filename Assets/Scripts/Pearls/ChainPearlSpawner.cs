using UnityEngine;

public class ChainPearlSpawner : MonoBehaviour, IPearlSpawner
{
    [SerializeField] private PearlChainController chain;
    [SerializeField] private PearlView pearlPrefab;

    public void SpawnPearls(int count)
    {
        if (chain == null || pearlPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            PearlView pearl = Instantiate(pearlPrefab);
            chain.AddPearlToTail(pearl);
        }
    }
}
