using UnityEngine;

/// <summary>
/// High-level game coordinator (Singleton).
/// Responsible only for: starting the game and tracking total pearl count.
/// </summary>
public class GameManager : MonoBehaviour
{
    // Singleton instance (careful: this is a simple pattern suitable for small projects/assignments)
    public static GameManager Instance { get; private set; }

    [SerializeField] private MonoBehaviour pearlSpawnerBehaviour; // must implement IPearlSpawner
    private IPearlSpawner pearlSpawner;

    [SerializeField] private int initialPearlCount = 20;

    private int currentPearlCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Dependency Inversion: depend on IPearlSpawner interface, not concrete class
        pearlSpawner = pearlSpawnerBehaviour as IPearlSpawner;
        if (pearlSpawner == null)
        {
            Debug.LogError("Assigned pearlSpawnerBehaviour does not implement IPearlSpawner.");
        }
    }

    private void OnEnable()
    {
        PearlEvents.OnPearlSpawned += HandlePearlSpawned;
        PearlEvents.OnPearlPopped += HandlePearlPopped;
    }

    private void OnDisable()
    {
        PearlEvents.OnPearlSpawned -= HandlePearlSpawned;
        PearlEvents.OnPearlPopped -= HandlePearlPopped;
    }

    private void Start()
    {
        if (pearlSpawner != null)
        {
            pearlSpawner.SpawnPearls(initialPearlCount);
        }
    }

    private void HandlePearlSpawned(IPearl pearl)
    {
        currentPearlCount++;
        PearlEvents.RaisePearlCountChanged(currentPearlCount);
    }

    private void HandlePearlPopped(IPearl pearl)
    {
        currentPearlCount--;
        if (currentPearlCount < 0) currentPearlCount = 0;
        PearlEvents.RaisePearlCountChanged(currentPearlCount);
    }
}
