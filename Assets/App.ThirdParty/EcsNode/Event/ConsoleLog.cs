
using System;

public interface IConsoleLogger
{
    void Log(object log);
    void LogError(object log);
}

public class ConsoleLog
{
    public static IConsoleLogger Logger;
    public static Action<object> LogAction;
    public static Action<object> LogErrorAction;
    public static Action<Exception> LogExceptionAction;

    public static void Debug(object log)
    {
        //System.Diagnostics.StackTrace.
        Logger?.Log(log);
        LogAction?.Invoke(log);
    }

    public static void Error(object log)
    {
        Logger?.LogError(log);
        LogErrorAction?.Invoke(log);
    }

    public static void Error(Exception e)
    {
        LogExceptionAction?.Invoke(e);
    }
}
