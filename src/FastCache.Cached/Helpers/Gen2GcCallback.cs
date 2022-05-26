using System.Runtime.ConstrainedExecution;

namespace FastCache.Helpers;

internal sealed class Gen2GcCallback : CriticalFinalizerObject
{
    private readonly Action _callback;

    private Gen2GcCallback(Action callback)
    {
        _callback = callback;
    }

#pragma warning disable CA1806 // Intentionally registering a dead object to be finalized on Gen2 GC.
    public static void Register(Action callback) => new Gen2GcCallback(callback);
#pragma warning restore CA1806

    ~Gen2GcCallback()
    {
        try
        {
            _callback();
        }
        catch
        {
#if DEBUG
            throw;
#endif
        }

        GC.ReRegisterForFinalize(this);
    }
}
