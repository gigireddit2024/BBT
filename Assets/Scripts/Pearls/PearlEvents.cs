using System;

public static class PearlEvents
{
    // Observer pattern using C# delegates + events
    public static event Action<IPearl> OnPearlSpawned;
    public static event Action<IPearl> OnPearlPopped;
    public static event Action<int> OnPearlCountChanged;

    public static void RaisePearlSpawned(IPearl pearl)
    {
        OnPearlSpawned?.Invoke(pearl);
    }

    public static void RaisePearlPopped(IPearl pearl)
    {
        OnPearlPopped?.Invoke(pearl);
    }

    public static void RaisePearlCountChanged(int count)
    {
        OnPearlCountChanged?.Invoke(count);
    }
}
