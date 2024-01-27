
namespace WIAD_DemoConsole;
internal class FibNoMemory
{
    public static int Fibonaci(int n)
    {
        if (n < 0)
        {
            throw new ArgumentException("must be positive", nameof(n));
        }
        Console.WriteLine($"Calculating FIB for {n}");

        if (n == 0)
        {
            return 0;
        }
        if (n == 1)
        {
            return 1;
        }
        return Fibonaci(n - 1) + Fibonaci(n - 2);
    }
}
