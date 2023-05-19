// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Linq;
using System.Text.Json;

Train.Program.Compile();
namespace Train
{
    static class Program
    {
        static Dictionary<string, IImplementation> implementations = new Dictionary<string, IImplementation>() { {"csharp", new ImplementationCSharp()}, {"cpp", new ImplementationCPlusPlus() } };
        public static void Compile()
        {
            Args args = getArgs();
            Console.WriteLine(args.inputPath);
            CompileWithPath(args.inputPath, args.outputPath, implementations[args.translateLang]);

        }
        static void CompileWithPath(string path, string outpath, IImplementation implementation)
        {
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
        static Args getArgs()
        {
            Args aargs = new Args();
            string[] args = Environment.GetCommandLineArgs();
            string prevarg = "i";
            foreach(var arg in args)
            {
                if (arg.StartsWith('-'))
                {
                    prevarg = arg.Replace("-", "");
                    continue;
                }
                switch (prevarg)
                {
                    case "i":
                        aargs.inputPath = arg;
                        break;
                    case "o":
                        aargs.outputPath = arg;
                        break;
                    case "lang":
                        aargs.translateLang = arg;
                        break;
                }
            }
            return aargs;
        }
    }
    class Args
    {
        public string inputPath { get; set; } = "a.train";
        public string outputPath { get; set; } = "a.o";
        public string translateLang { get; set; } = "csharp";
    }
}