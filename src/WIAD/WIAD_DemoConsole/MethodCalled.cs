namespace WIAD_DemoConsole;
public class MethodCalled
{
    public readonly TypeAndMethodData typeAndMethodData;
    public readonly Dictionary<string, string?> valueValues;
    public readonly Dictionary<string, string?> stringValues;
    public readonly Dictionary<string, string?> exposeValues;

    public readonly DateTime StartedAtDate;
    public DateTime? FinishedAtDate { get; set; }
    public AccumulatedStateMethod State { get; set; }
    public MethodCalled(TypeAndMethodData typeAndMethodData, 
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues,
        Dictionary<string, string?> exposeValues
    )
    {
        StartedAtDate = DateTime.UtcNow;
        State = AccumulatedStateMethod.Started;
        this.typeAndMethodData = typeAndMethodData;
        this.valueValues = valueValues;
        this.stringValues = stringValues;
        this.exposeValues = exposeValues;
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
}
