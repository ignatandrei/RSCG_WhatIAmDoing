namespace RSCG_WhatIAmDoing_Common;

public class CachingData
{
    public static IMemoryCache cacheMethodsHistory = new MemoryCache(new MemoryCacheOptions());
    public static ConcurrentBag<string> MethodKeys = new ConcurrentBag<string>();

    public static IOrderedEnumerable<MethodCalled> Methods()
    {
        return MethodKeys.Select(x =>
        {
            try
            {
                return (MethodCalled?)cacheMethodsHistory.Get<MethodCalled>(x);
            }
            catch (Exception)
            {
                return null;
            }
        }
)
       .Where(it => it != null)
    .Select(it => it!)
    .OrderBy(it => it.StartedAtTicks)
    ;
    }
}
public class InterceptorMethodInstanceClassBase : IInterceptorMethodInstanceClass
{
    public static string InterceptInstanceMethodBefore<T>(
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
        var mc = new MethodCalled(typeAndMethod, valueValues, stringValues, exposeValues);
        CachingData.cacheMethodsHistory.Set(id, mc);
        CachingData.MethodKeys.Add(id);
        string color = typeAndMethod.IsVoid ? "yellow" : "red";
        //AnsiConsole.MarkupLineInterpolated($"Calling [bold {color}]{typeAndMethod.MethodName}[/] from {typeAndMethod.TypeOfClass} with [underline blue]{mc.ArgumentsAsString()}[/] ");        
        return id;
    }
    public static void InterceptMethodAfterWithoutResult(string id)
    {
        //AnsiConsole.MarkupLineInterpolated($"finish method [bold yellow]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()} ");
    }
    public static void InterceptMethodAfterWithResult(string id, object? result)
    {

        var mc = CachingData.cacheMethodsHistory.Get<MethodCalled>(id);
        if (mc == null) return;
        mc.SetResult(result);
        var typeAndMethod = mc.typeAndMethodData;
        //AnsiConsole.MarkupLineInterpolated($"end method [bold red]{typeAndMethod.MethodName}[/] with args {mc.ArgumentsAsString()} returning {result}");
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
        //Console.WriteLine($"Exit method {typeAndMethod?.MethodName} with arguments {typeAndMethod?.Tag}");

    }

    

    
}
