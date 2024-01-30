using Microsoft.Extensions.Caching.Memory;

namespace WIAD_DemoConsole;

[ExposeClass(typeof(Encoding), nameof(Encoding.EncodingName))]
[InterceptStatic("System.IO.File.*ts")]
[InterceptStatic("System.IO.File.*")]
//[InterceptStatic("System.Console.*")]
[InterceptStatic("WIAD_DemoConsole.Fib.*")]
internal class InterceptorMethodStatic
{
    public InterceptorMethodStatic()
    {
        Console.WriteLine("!!!! This should not be intercepted!");
    }
    //static ConcurrentDictionary<string, TypeAndMethodStatic> _cache = new ();
    internal static string InterceptStaticMethodBefore(        
        string typeAndMethodStatic, 
        Dictionary<string,string?> valueValues, 
        Dictionary<string, string?> stringValues  ,
        Dictionary<string, string?> exposeValues
        )
    {
        
        var id = Guid.NewGuid().ToString();
        var typeAndMethod = System.Text.Json.JsonSerializer.Deserialize<TypeAndMethodStatic>(typeAndMethodStatic);
        if (typeAndMethod == null) return id;
        var mc = new MethodCalled(typeAndMethod, valueValues, stringValues, exposeValues);
        Program.cacheMethodsHistory.Set(id, mc);
        Program.MethodKeys.Add(id);

        string color = typeAndMethod.IsVoid ? "green" : "yellow";
        AnsiConsole.MarkupLineInterpolated($"Calling [bold {color}]{typeAndMethod.MethodName}[/] from {typeAndMethod.TypeOfClass} with [underline blue]{mc.ArgumentsAsString()}[/] ");
        return id;
    }
    internal static void InterceptStaticMethodAfterWithoutResult(string id)
    {
        var mc = Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if(mc == null) return;  
        mc.State |= AccumulatedStateMethod.Finished;
        var typeAndMethod = mc.typeAndMethodData;

        AnsiConsole.MarkupLineInterpolated($"finish method [bold green]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()}");
    }
    internal static void InterceptStaticMethodAfterWithResult(string id , object? result)
    {
        var mc = Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return; 
        mc.State |= AccumulatedStateMethod.Finished;
        var typeAndMethod = mc.typeAndMethodData;

        AnsiConsole.MarkupLineInterpolated($"end method [bold yellow]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()} returning {result}");
    }
    internal static void InterceptStaticMethodException(string id,Exception ex)
    {
        var mc = Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return; 
        mc.State |= AccumulatedStateMethod.RaiseException;
        var typeAndMethod = mc.typeAndMethodData;

        Console.WriteLine($"Exception method {typeAndMethod.TypeOfClass} with arguments {mc.ArgumentsAsString()}");
    }
    internal static void InterceptStaticMethodFinally(string id)
    {
        var mc = Program.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return; 
        mc.State |= AccumulatedStateMethod.Finished;

    }

}
