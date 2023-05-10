// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Linq;
using System.Text.Json;

Train.Program.Compile();
namespace Train
{
    static class Program
    {
        public static void Compile()
        {
            CompileWithPath("/Users/alexjakubiak/Codename Bravo/test.bravo", "/Users/alexjakubiak/Codename Bravo/test.cs", false);
        }
        static void CompileWithPath(string path, string outpath, bool cpp)
        {
            string[] args = Environment.GetCommandLineArgs();

            IEnumerable<string> enumerator = File.ReadLines(path);
            File.WriteAllText(outpath, "");

            Namespace global = Parser.CreateAST(enumerator);
            var imp = new ImplementationCSharp();
            string code = imp.ConvertGlobalNamespace(global);
            File.WriteAllText("code.cs", code);
        }
        public static void Exit(exception x)
        {
            Console.WriteLine(x);
            Environment.Exit(1);
        }
    }
}