using Godot;

namespace GodotPilotPlugin.Tools;

[AttributeUsage(AttributeTargets.Method)]
public class RunOnMainThreadAttribute : Attribute { }

public static class MainThreadHelper
{
    public static async Task<T> RunOnMainThread<T>(Func<T> action)
    {
        var tcs = new TaskCompletionSource<T>();
        Callable.From(() => {
            try
            {
                var result = action();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }).CallDeferred();
        return await tcs.Task;
    }

    public static async Task RunOnMainThread(Action action)
    {
        var tcs = new TaskCompletionSource();
        Callable.From(() => {
            try
            {
                action();
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }).CallDeferred();
        await tcs.Task;
    }
}
