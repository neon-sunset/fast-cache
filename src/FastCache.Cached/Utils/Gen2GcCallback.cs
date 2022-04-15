using System.Runtime.ConstrainedExecution;

namespace FastCache.Utils;

internal sealed class Gen2GcCallback : CriticalFinalizerObject
{
    private readonly Func<object> _callback;

    private Gen2GcCallback(Func<object> callback)
    {
        _callback = callback;
    }

    public static void Register(Func<object> callback) => new Gen2GcCallback(callback);

    ~Gen2GcCallback()
    {
        try
        {
            _callback();
        }
        catch { }

        GC.ReRegisterForFinalize(this);
    }
}
