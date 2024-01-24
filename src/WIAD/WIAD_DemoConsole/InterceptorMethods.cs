using RSCG_WhatIAmDoing;
using System.Collections.Concurrent;

namespace WIAD_DemoConsole;

[InterceptStatic("System.IO.File.*")]
internal class InterceptorMethodStatic
{
    static ConcurrentDictionary<string, TypeAndMethodStatic> _cache = new ();
    internal static string InterceptStaticMethodBefore(string typeAndMethodStatic)
    {        
        var typeAndMethod = System.Text.Json.JsonSerializer.Deserialize<TypeAndMethodStatic>(typeAndMethodStatic);
        ArgumentNullException.ThrowIfNull(typeAndMethod);
        Console.WriteLine($"!!!!Intercept static Before call method {typeAndMethod.TypeOfClass} with ");
        if (typeAndMethod.ValueArguments.Length > 0)
        {
            Console.WriteLine($"!!!!values");

            foreach (var item in typeAndMethod.ValueArguments)
            {
                Console.WriteLine($" {item.Name}");
            }
        }
        if (typeAndMethod.StringArguments.Length > 0)
        {
            Console.WriteLine($"!!!!strings ");
            foreach (var item in typeAndMethod.StringArguments)
            {
                Console.WriteLine($" {item.Name}");
            }
        }
        var id =Guid.NewGuid().ToString();
        _cache.TryAdd(id, typeAndMethod);
        return id;
    }
    internal static void InterceptStaticMethodAfter(string id , object? result)
    {
        _cache.TryRemove(id, out var typeAndMethod);

        Console.WriteLine($"After method  with arguments ");
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

}
