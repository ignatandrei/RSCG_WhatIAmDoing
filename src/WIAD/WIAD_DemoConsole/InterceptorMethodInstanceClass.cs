using Microsoft.Extensions.Caching.Memory;
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
    static MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(30));
    
    internal static string InterceptInstanceMethodBefore<T>(
        T instance,
        string typeAndMethodStatic,
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues,
        Dictionary<string, string?> exposeValues
            )
    {

        var id = Guid.NewGuid().ToString();
        var typeAndMethod = System.Text.Json.JsonSerializer.Deserialize<TypeAndMethodInstance>(typeAndMethodStatic);
        if (typeAndMethod == null) return id;
        var mc=new MethodCalled(typeAndMethod,valueValues,stringValues,exposeValues);
        Program.cacheMethodsHistory.Set(id, mc);
        Program.MethodKeys.Add(id);
        string color = typeAndMethod.IsVoid ? "yellow" : "red";
        AnsiConsole.MarkupLineInterpolated($"Calling [bold {color}]{typeAndMethod.MethodName}[/] from {typeAndMethod.TypeOfClass} with [underline blue]{mc.ArgumentsAsString()}[/] ");        
        return id;
    }
    internal static void InterceptInstanceMethodAfterWithoutResult(string id)
    {
        var mc = Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.State |= AccumulatedStateMethod.Finished;
        var typeAndMethod =mc.typeAndMethodData;
        AnsiConsole.MarkupLineInterpolated($"finish method [bold yellow]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()} ");
    }
    internal static void InterceptInstanceMethodAfterWithResult(string id, object? result)
    {

        var mc=Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.State |= AccumulatedStateMethod.Finished;
        var typeAndMethod = mc.typeAndMethodData;
        AnsiConsole.MarkupLineInterpolated($"end method [bold red]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()} returning {result}");
    }
    internal static void InterceptInstanceMethodException(string id, Exception ex)
    {
        var mc = Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.State |= AccumulatedStateMethod.RaiseException;
        var typeAndMethod = mc.typeAndMethodData;
        Console.WriteLine($"Exception method {typeAndMethod.TypeOfClass} with arguments {mc.ArgumentsAsString()}");

    }
    internal static void InterceptInstanceMethodFinally(string id)
    {
        var mc = Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.State |= AccumulatedStateMethod.Finished;
        //Console.WriteLine($"Exit method {typeAndMethod?.MethodName} with arguments {typeAndMethod?.Tag}");

    }

}
