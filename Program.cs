// See https://aka.ms/new-console-template for more information
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

Train.Program.Run();
namespace Train
{
    static class Program
    {
        static string[] argsStr;
        public static void Run()
        {
            argsStr = Environment.GetCommandLineArgs();
            switch (argsStr[1])
            {
                case "compile":
                    Compile();
                    break;
                case "mkproject":
                    MakeProject();
                    break;
                case "scanheaders":
                    break;
                default:
                    Console.WriteLine("Bad operation: " + argsStr[1]);
                    break;
            }
        }
        static Dictionary<string, IImplementation> implementations = new Dictionary<string, IImplementation>() { {"csharp", new ImplementationCSharp()}, {"c", new ImplementationC() } };
        public static void Compile()
        {
            Args args = getArgs();
            Console.WriteLine(argsStr[2]);
            var deserializer = new YamlDotNet.Serialization.Deserializer();
            TrainProject project = deserializer.Deserialize<TrainProject>(File.ReadAllText(argsStr[2] + ".yaml"));
            implementations[args.translateLang].getAllTypes(project);
            if (args.translateLang == "c")
            {
                ((ImplementationC)implementations["c"]).proj = project;
            }
            foreach (var source in project.sourceFiles)
                CompileWithPath(source, source+".il", implementations[args.translateLang]);

        }
        static void MakeProject()
        {
            var serializer = new YamlDotNet.Serialization.Serializer();
            File.WriteAllText(argsStr[2] + ".yaml", "");
            var tw = File.AppendText(argsStr[2] + ".yaml");
            serializer.Serialize(tw, new TrainProject() { sourceFiles = new List<string>() { "test.train"} });
            Console.WriteLine("made project " + argsStr[2]);
            tw.Close();
        }
        static void ScanHeaders()
        {

        }
        static void CompileWithPath(string path, string outpath, IImplementation implementation)
        {
            File.WriteAllText(outpath, "");
            List<string> enumerator = File.ReadAllLines(path).ToList();
            Namespace global = Parser.CreateAST(enumerator);
            string code = implementation.ConvertGlobalNamespace(global, Path.GetFileName(path));
            if(implementation.LangName == "C")
            {
                string header = ((ImplementationC)implementation).header;
                File.WriteAllText(outpath + ".h", header);
            }
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
            string prevarg = "operation";
            for(int i = 1; i < argsStr.Length; i++)
            {
                string arg = argsStr[i];
                if (arg.StartsWith('-'))
                {
                    prevarg = arg.Replace("-", "");
                    continue;
                }
                switch (prevarg)
                {
                    case "operation":
                        aargs.operationType = arg;
                        break;
                    case "project":
                        aargs.projectPath = arg;
                        break;
                    case "output":
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
        public string operationType { get; set; } = "compile";
        public string projectPath { get; set; } = "a.train";
        public string outputPath { get; set; } = "a.o";
        public string translateLang { get; set; } = "c";
    }
}