using System.Text;
using WIAD_DemoConsole;
using static System.Console;

//Console.WriteLine("Heasdasdallo, World!");
string nameFile = "test.txt";
//if (File.Exists(nameFile))
//{
//    File.Delete(nameFile);
//}

//File.WriteAllText(nameFile, "Hello, World!");
//TODO: add encoding  Encoding.ASCII.EncodingName to serialize and deserialize
File.WriteAllText(nameFile, "Hello, World!", Encoding.ASCII);
//Console.WriteLine(WIAD_DemoConsole.FibMemory.Fibonaci(7));
//Console.WriteLine(FibNoMemory.Fibonaci(7));

//WriteLine("static");

//var pers = new Person("Andrei", "Ignat");
//WriteLine(pers.FullName());
//pers.WriteNameToConsole();