
namespace WIAD_DemoConsole;
internal class FibMemory
{
    static Dictionary<int,int> data = new()
    {
        { 0 , 0 },
        { 1 , 1 }
    };
    public static int Fibonaci(int n)
    {
        if(data.ContainsKey(n))
            return data[n];
        
        //Console.WriteLine($"I am starting to calculate for {n}");
        if (n < 0)
        {
            throw new ArgumentException("must be positive", nameof(n));
        }
                
        var ret= Fibonaci(n - 1) + Fibonaci(n - 2);
        data.Add(n, ret);
        return ret;
    }
}
