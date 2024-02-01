namespace RSCG_WhatIAmDoing_Common;
public class InterceptorMethodStaticBase : IInterceptorMethodStatic
{

    public InterceptorMethodStaticBase()
    {
        Console.WriteLine("!!!! This should not be intercepted!");
    }
    //static ConcurrentDictionary<string, TypeAndMethodStatic> _cache = new ();
    public static string InterceptStaticMethodBefore(
        string typeAndMethodStatic,
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues,
        Dictionary<string, string?> exposeValues
        )
    {

        var id = Guid.NewGuid().ToString();
        var typeAndMethod = System.Text.Json.JsonSerializer.Deserialize<TypeAndMethodStatic>(typeAndMethodStatic);
        if (typeAndMethod == null) return id;
        var mc = new MethodCalled(typeAndMethod, valueValues, stringValues, exposeValues);
        CachingData.cacheMethodsHistory.Set(id, mc);
        CachingData.MethodKeys.Add(id);

        string color = typeAndMethod.IsVoid ? "green" : "yellow";
        //AnsiConsole.MarkupLineInterpolated($"Calling [bold {color}]{typeAndMethod.MethodName}[/] from {typeAndMethod.TypeOfClass} with [underline blue]{mc.ArgumentsAsString()}[/] ");
        return id;
    }
    public static void InterceptMethodAfterWithoutResult(string id)
    {
        //AnsiConsole.MarkupLineInterpolated($"finish method [bold green]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()}");
    }
    public static void InterceptMethodAfterWithResult(string id, object? result)
    {
        var mc = CachingData.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.SetResult(result);
        //var typeAndMethod = mc.typeAndMethodData;

        //AnsiConsole.MarkupLineInterpolated($"end method [bold yellow]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()} returning {result}");
    }
    public static void InterceptMethodException(string id, Exception ex)
    {
        var mc = CachingData.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.SetException(ex);
        
        var typeAndMethod = mc.typeAndMethodData;
        
        Console.WriteLine($"!!!Exception method {typeAndMethod.MethodName} from {typeAndMethod.TypeOfClass} with arguments {mc.ArgumentsAsString()}");
    }
    public static void InterceptMethodFinally(string id)
    {
        var mc = CachingData.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.SetFinished();
    }

}
