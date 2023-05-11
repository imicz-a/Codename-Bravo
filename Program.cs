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
            CompileWithPath("test.train", "code.cs", new ImplementationCSharp());
        }
        static void CompileWithPath(string path, string outpath, IImplementation implementation)
        {
            string[] args = Environment.GetCommandLineArgs();

            IEnumerable<string> enumerator = File.ReadLines(path);
            File.WriteAllText(outpath, "");

            Namespace global = Parser.CreateAST(enumerator);
            string code = implementation.ConvertGlobalNamespace(global);
            File.WriteAllText(outpath, code);
        }
        public static void Exit(exception x)
        {
            Console.WriteLine(x);
            Environment.Exit(1);
        }
    }
}