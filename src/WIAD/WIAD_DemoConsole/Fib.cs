
namespace WIAD_DemoConsole;
internal class Fib
{
    public static int Fibonaci(int n)
    {
        if (n < 0)
        {
            throw new ArgumentException("must be positive", nameof(n));
        }
        if (n == 0)
        {
            return 0;
        }
        if (n == 1)
        {
            return 1;
        }
        //return 10;
        return Fibonaci(n - 1) + Fibonaci(n - 2);
    }
}
