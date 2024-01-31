namespace RSCG_WhatIAmDoing_Common;
public class MethodCalled(TypeAndMethodData typeAndMethodData,
    Dictionary<string, string?> valueValues,
    Dictionary<string, string?> stringValues,
    Dictionary<string, string?> exposeValues
    )
{
    public readonly TypeAndMethodData typeAndMethodData = typeAndMethodData;
    public readonly Dictionary<string, string?> valueValues = valueValues;
    public readonly Dictionary<string, string?> stringValues = stringValues;
    public readonly Dictionary<string, string?> exposeValues = exposeValues;

    public readonly DateTime StartedAtDate = DateTime.UtcNow;
    public readonly long StartedAtTicks = Environment.TickCount64;
    public DateTime? FinishedAtDate { get; set; }
    public long? EndAtTicks { get; private set; }
    public AccumulatedStateMethod State { get; private set; } = AccumulatedStateMethod.Started;
    public void SetFinished()
    {
        FinishedAtDate = DateTime.UtcNow;
        State |= AccumulatedStateMethod.Finished;
        EndAtTicks = Environment.TickCount64;

    }
    public Exception Exception { get; private set; }
    public void SetException(Exception ex)
    {

        State |= AccumulatedStateMethod.RaiseException;
        this.Exception = ex;

    }

    public string ArgumentsAsString()
    {
        var argsToBeTyped= "";
        if (valueValues.Count > 0)
        {
            var values = string.Join(";", valueValues.Select(static it => $"{it.Key}={it.Value}"));
            //Console.WriteLine($"value arguments {values}");
            argsToBeTyped += values;
        }

        if (stringValues.Count > 0)
        {
            var values = string.Join(";", stringValues.Select(static it => $"{it.Key}={it.Value}"));
            //Console.WriteLine($"string arguments {values}");
            argsToBeTyped += values;
        }
        if (exposeValues.Count > 0)
        {
            var values = string.Join(";", exposeValues.Select(static it => $"{it.Key}={it.Value}"));
            //Console.WriteLine($"string arguments {values}");
            argsToBeTyped += values;
        }
        return argsToBeTyped;
    }
    public object? Result { get; private set; }
    internal void SetResult(object? result)
    {
        State |= AccumulatedStateMethod.HasResult;
        Result = result;
    }
}
