namespace WIAD_DemoConsole;

//[InterceptInstanceClass(typeof(Person),"ame")]
//[InterceptInstanceClass(typeof(Person), "parat")]
//[InterceptInstanceClass(typeof(Person), "ncodi")]
[InterceptInstanceClass(typeof(Person), ".*")]

internal class InterceptorMethodInstanceClass
{
    public InterceptorMethodInstanceClass()
    {
        
    }
    static ConcurrentDictionary<string, TypeAndMethodInstance> _cache = new();
    internal static string InterceptInstanceMethodBefore<T>(
        T instance,
        string typeAndMethodStatic,
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues
        )
    {

        var id = Guid.NewGuid().ToString();
        var typeAndMethod = System.Text.Json.JsonSerializer.Deserialize<TypeAndMethodInstance>(typeAndMethodStatic);
        ArgumentNullException.ThrowIfNull(typeAndMethod);
        _cache.TryAdd(id, typeAndMethod);
        var argsToBeTyped = "";
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
        typeAndMethod.Tag = argsToBeTyped;
        string color = typeAndMethod.IsVoid ? "yellow" : "red";
        AnsiConsole.MarkupLineInterpolated($"Calling [bold {color}]{typeAndMethod.MethodName}[/] from {typeAndMethod.TypeOfClass} with [underline blue]{argsToBeTyped}[/] ");
        return id;
    }
    internal static void InterceptInstanceMethodAfterWithoutResult(string id)
    {
        AnsiConsole.MarkupLineInterpolated($"finish method [bold yellow]{_cache[id].MethodName}[/] with args {_cache[id].Tag} ");
    }
    internal static void InterceptInstanceMethodAfterWithResult(string id, object? result)
    {

        AnsiConsole.MarkupLineInterpolated($"end method [bold red]{_cache[id].MethodName}[/] with args {_cache[id].Tag} returning {result}");
    }
    internal static void InterceptInstanceMethodException(string id, Exception ex)
    {
        if (_cache.TryGetValue(id, out var typeAndMethod))
        {
            Console.WriteLine($"Exception method {typeAndMethod.TypeOfClass} with arguments {typeAndMethod.Tag}");

        }
        else
        {
            Console.WriteLine($"strange the call to begin has not been intercepted ");
        }
    }
    internal static void InterceptInstanceMethodFinally(string id)
    {
        _cache.TryRemove(id, out var typeAndMethod);
        //Console.WriteLine($"Exit method {typeAndMethod?.MethodName} with arguments {typeAndMethod?.Tag}");

    }

}
