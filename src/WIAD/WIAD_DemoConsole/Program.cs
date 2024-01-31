using Microsoft.Extensions.Caching.Memory;
using System.Text;
using WIAD_DemoConsole;
using static System.Console;


//Console.WriteLine("Heasdasdallo, World!");
string nameFile = "test.txt";
if (File.Exists(nameFile))
{
    File.Delete(nameFile);
}

File.WriteAllText(nameFile, "Hello, World!");
File.WriteAllText(nameFile, "Hello, World!", Encoding.ASCII);
Console.WriteLine(WIAD_DemoConsole.FibMemory.Fibonaci(7));
Console.WriteLine(FibNoMemory.Fibonaci(7));

//WriteLine("static");

var pers = new Person("Andrei", "Ignat");
WriteLine(pers.FullName());
//pers.WriteNameToConsole();
Console.WriteLine(pers.FullNameWithSeparator("++++"));
//Console.WriteLine(pers.WithEncoding(Encoding.ASCII));
Console.WriteLine("Press enter to see the history");
Console.ReadLine();
List<MethodCalled> list = new List<MethodCalled>();
var data= MethodKeys.Select(x =>
{
    try
    {
        return (MethodCalled?)Program.cacheMethodsHistory.Get<MethodCalled>(x);
    }
    catch (Exception)
    {
        return null;
    }
}
)
       .Where(it => it!=null)
    .Select(it => it!)
    .OrderBy(it => it.StartedAtTicks)
    .ToArray();
;

foreach (var item in data)
{
    WriteLine($"Method {item.typeAndMethodData.MethodName} from class {item.typeAndMethodData.TypeOfClass} Time: {item.StartedAtDate} state {item.State} ");
    WriteLine($"  =>Arguments: {item.ArgumentsAsString()}");
}

//foreach (var item in Program.cacheMethodsHistory)
public partial class Program
{
    public static IMemoryCache cacheMethodsHistory = new MemoryCache(new MemoryCacheOptions());
    public static ConcurrentBag<string> MethodKeys= new ConcurrentBag<string>();


}