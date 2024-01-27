using System.Text;

Console.WriteLine("Heasdasdallo, World!");
string nameFile = "test.txt";
if(File.Exists(nameFile))
{
    File.Delete(nameFile);
}

File.WriteAllText(nameFile, "Hello, World!");
File.WriteAllText(nameFile, "Hello, World!",Encoding.ASCII);

Console.WriteLine(WIAD_DemoConsole.Fib.Fibonaci(7));