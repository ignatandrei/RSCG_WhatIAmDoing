namespace WIAD_DemoConsole;

//[InterceptStatic("System.IO.File.*ts")]
//[InterceptStatic("System.IO.File.*")]
//[InterceptStatic("System.Console.*")]
//[InterceptStatic("WIAD_DemoConsole.Fib.*")]
internal class InterceptorMethodStatic
{
    public InterceptorMethodStatic()
    {
        Console.WriteLine("!!!! This should not be intercepted!");
    }
    static ConcurrentDictionary<string, TypeAndMethodStatic> _cache = new ();
    internal static string InterceptStaticMethodBefore(        
        string typeAndMethodStatic, 
        Dictionary<string,string?> valueValues, 
        Dictionary<string, string?> stringValues  
        )
    {
        
        var id = Guid.NewGuid().ToString();
        //InterceptorMethodStatic.InterceptStaticMethodBefore("asd", new[] { "a" }, new[] { "b" });
        var typeAndMethod = System.Text.Json.JsonSerializer.Deserialize<TypeAndMethodStatic>(typeAndMethodStatic);
        ArgumentNullException.ThrowIfNull(typeAndMethod);
        _cache.TryAdd(id, typeAndMethod);
        var argsToBeTyped = "";
        if(valueValues.Count > 0)
        {
            var values=string.Join(";", valueValues.Select(static it => $"{it.Key}={it.Value}"));
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
        string color = typeAndMethod.IsVoid ? "green" : "yellow";
        AnsiConsole.MarkupLineInterpolated($"Calling [bold {color}]{typeAndMethod.MethodName}[/] from {typeAndMethod.TypeOfClass} with [underline blue]{argsToBeTyped}[/] ");
        return id;
    }
    internal static void InterceptStaticMethodAfterWithoutResult(string id)
    {

        AnsiConsole.MarkupLineInterpolated($"finish method [bold green]{_cache[id].MethodName}[/] with args {_cache[id].Tag} ");
    }
    internal static void InterceptStaticMethodAfterWithResult(string id , object? result)
    {
        
        AnsiConsole.MarkupLineInterpolated($"end method [bold yellow]{_cache[id].MethodName}[/] with args {_cache[id].Tag} returning {result}");
    }
    internal static void InterceptStaticMethodException(string id,Exception ex)
    {
        if(_cache.TryGetValue(id, out var typeAndMethod))
        {
            Console.WriteLine($"Exception method {typeAndMethod.TypeOfClass} with arguments {typeAndMethod.Tag}");

        }
        else
        {
            Console.WriteLine($"strange the call to begin has not been intercepted ");
        }
    }
    internal static void InterceptStaticMethodFinally(string id)
    {
        _cache.TryRemove(id, out var typeAndMethod);
        //Console.WriteLine($"Exit method {typeAndMethod?.MethodName} with arguments {typeAndMethod?.Tag}");

    }

}
