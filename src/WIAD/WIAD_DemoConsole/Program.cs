// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");
File.WriteAllText("test1.txt", "Hello, World!");
File.WriteAllText("test2.txt", "Hello, World!",Encoding.ASCII);
