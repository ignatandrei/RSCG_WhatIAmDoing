using RSCG_WhatIAmDoing;
using System.Collections.Concurrent;

namespace WIAD_DemoConsole;

//[InterceptStatic("System.IO.File.*ts")]
//[InterceptStatic("System.IO.File.*")]
//[InterceptStatic("System.Console.*")]
[InterceptStatic("WIAD_DemoConsole.Fib.*")]
internal class InterceptorMethodStatic
{
    static ConcurrentDictionary<string, TypeAndMethodStatic> _cache = new ();
    internal static string InterceptStaticMethodBefore(string typeAndMethodStatic, Dictionary<string,string?> valueValues, Dictionary<string, string?> stringValues  )
    {
        Dictionary<string, string> dict = new()
        {
            {"asd","asdasd" }
        };
        var id = Guid.NewGuid().ToString();
        //InterceptorMethodStatic.InterceptStaticMethodBefore("asd", new[] { "a" }, new[] { "b" });
        var typeAndMethod = System.Text.Json.JsonSerializer.Deserialize<TypeAndMethodStatic>(typeAndMethodStatic);
        ArgumentNullException.ThrowIfNull(typeAndMethod);
        _cache.TryAdd(id, typeAndMethod);
        Console.WriteLine($"!!!!Calling {typeAndMethod.MethodName} from {typeAndMethod.TypeOfClass} with following arguments");
        if(valueValues.Count > 0)
        {
            var values=string.Join(";", valueValues.Select(static it => $"{it.Key}={it.Value}"));
            Console.WriteLine($"value arguments {values}");
        }

        if (stringValues.Count > 0)
        {
            var values = string.Join(";", stringValues.Select(static it => $"{it.Key}={it.Value}"));
            Console.WriteLine($"string arguments {values}");
        }

        Console.WriteLine($"please remember id {id}");
        return id;
    }
    internal static void InterceptStaticMethodAfterWithoutResult(string id)
    {

        Console.WriteLine($"After method {id} ");
    }
    internal static void InterceptStaticMethodAfterWithResult(string id , object? result)
    {
        
        Console.WriteLine($"After method {id} returning {result}");
    }
    internal static void InterceptStaticMethodException(string id,Exception ex)
    {
        if(_cache.TryGetValue(id, out var typeAndMethod))
        {
            Console.WriteLine($"Exception method {typeAndMethod.TypeOfClass} with arguments ");

        }
        else
        {
            Console.WriteLine($"strange the call to begin has not been intercepted ");
        }
    }
    internal static void InterceptStaticMethodFinally(string id)
    {
        _cache.TryRemove(id, out var typeAndMethod);
        Console.WriteLine($"Exit method {id} ");

    }

}
