using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Train
{
	public class ImplementationC : IImplementation
	{
        public string LangName { get; set; } = "C";
        public TrainProject proj;
        string code = "";
        public string header = "#include \"trainBaseLib.h\"\n";
        List<Containter> hierarchy = new List<Containter>();
        bool isInFunction;

        public ImplementationC()
		{
		}

        public void Interpret(ASTElem elem)
        {
            switch (elem.elemtype)
            {
                case ASTElem.ElementType.VarDeclaration:
                    InterpretVarDeclaration((VarDeclaration)elem);
                    return;
                case ASTElem.ElementType.FunctionCall:
                    InterpretFunctionCalls((FunctionCall)elem);
                    return;
                case ASTElem.ElementType.Var:
                    InterpretVar((Var)elem);
                    return;
                case ASTElem.ElementType.VarVar:
                    InterpretVarVar((VarVar)elem);
                    return;
                case ASTElem.ElementType.Const:
                    InterpretConst((Const)elem);
                    return;
                case ASTElem.ElementType.Container:
                    InterpretContainer((Containter)elem);
                    return;
                case ASTElem.ElementType.Func:
                    InterpretFunc((Func)elem);
                    return;
                case ASTElem.ElementType.Namespace:
                    InterpretNamespace((Namespace)elem);
                    return;
                case ASTElem.ElementType.LineOperation:
                    InterpretLineOperation((LineOperation)elem);
                    return;
                case ASTElem.ElementType.Operation:
                    InterpretOperation((Operation)elem);
                    return;
                case ASTElem.ElementType.OperationVar:
                    InterpretOperationVar((OperationVar)elem);
                    return;
                case ASTElem.ElementType.ExternCall:
                    InterpretExternCall((ExternCall)elem);
                    return;
                case ASTElem.ElementType.UsingDirective:
                    InterpretUsingDirective((UsingDirective)elem);
                    return;
                case ASTElem.ElementType.ForLoop:
                    InterpretForLoop((ForLoop)elem);
                    return;
                case ASTElem.ElementType.WhileLoop:
                    InterpretWhileLoop((WhileLoop)elem);
                    return;
                case ASTElem.ElementType.ForeachLoop:
                    InterpretForeachLoop((ForeachLoop)elem);
                    return;
                default:
                    return;
            }
        }

        public string ConvertGlobalNamespace(Namespace global, string filename)
        {
            code = "#include \"" + filename + ".h\"";
            foreach(var v in global.contents)
            {
                Interpret(v);
            }
            return code;
        }

        public string InterpretConst(Const c)
        {

            return "";
        }

        public string InterpretContainer(Containter c)
        {
            throw new NotImplementedException();
        }

        public string InterpretExternCall(ExternCall ex)
        {
            throw new NotImplementedException();
        }

        public string InterpretForeachLoop(ForeachLoop fl)
        {
            throw new NotImplementedException();
        }

        public string InterpretForLoop(ForLoop fl)
        {
            throw new NotImplementedException();
        }

        public string InterpretFunc(Func f)
        {
            throw new NotImplementedException();
        }

        public string InterpretFunctionCalls(FunctionCall fc)
        {
            throw new NotImplementedException();
        }

        public string InterpretLineOperation(LineOperation lo)
        {
            throw new NotImplementedException();
        }

        public string InterpretNamespace(Namespace n)
        {
            throw new NotImplementedException();
        }

        public string InterpretOperation(Operation o)
        {
            throw new NotImplementedException();
        }

        public string InterpretOperationVar(OperationVar ov)
        {
            throw new NotImplementedException();
        }

        public string InterpretReturn(Return r)
        {
            throw new NotImplementedException();
        }

        public string InterpretUsingDirective(UsingDirective u)
        {
            foreach(var src in proj.sourceFiles)
            {
                if(Path.GetFileName(src) == u.libname + ".train")
                {
                    header += "#include \"" + u.libname + ".il.h\"\n";
                    return null;
                }
            }
                
            header += "#include <" + u.libname + ">\n";
            return null;
        }

        public string InterpretVar(Var v)
        {
            throw new NotImplementedException();
        }

        public string InterpretVarDeclaration(VarDeclaration vd)
        {
            throw new NotImplementedException();
        }

        public string InterpretVarVar(VarVar vv)
        {
            throw new NotImplementedException();
        }

        public string InterpretWhileLoop(WhileLoop wl)
        {
            throw new NotImplementedException();
        }

        public string mangle()
        {
            string mang = "";
            foreach (var v in hierarchy)
            {
                mang += v.conname + "dot";
            }

            return mang;
        }

        public void getAllTypes(TrainProject proj)
        {
            foreach (var src in proj.sourceFiles)
            {
                deserializeHeaderDict();
                var lines = File.ReadAllLines(src);
                List<string> headersToFind = new();
                foreach (var v in lines)
                {
                    if (v.StartsWith("class "))
                    {
                        if (v.EndsWith("{"))
                            v.Remove(v.Length - 1, 1);
                        Checker.types.Add(v.Split()[1]);
                    }
                    if (v.StartsWith("using "))
                    {
                        string headername = v.Split()[1];
                        if (headerLocations.ContainsKey(headername))
                        {
                            Checker.types = Checker.types.Concat(getTypesFromAHeader(File.ReadAllLines(headerLocations[headername] + "/" + headername))).ToList();
                        }
                        else
                        {
                            foreach (var s in proj.sourceFiles)
                            {
                                if (Path.GetFileName(s) == headername + ".train")
                                {
                                    Checker.types = Checker.types.Concat(getTypesFromAHeader(File.ReadAllLines(header+".il.h"))).ToList();
                                }
                            }
                            Console.WriteLine(headername + " header does not exist!");
                        }
                    }
                    
                }
            }
        }
        Dictionary<string, string> headerLocations = new Dictionary<string, string>();
        void SearchDirForHeaders(string includedir)
        {
            foreach (var header in Directory.GetFiles(includedir))
            {
                if(!headerLocations.ContainsKey(Path.GetFileName(header)))
                    headerLocations.Add(Path.GetFileName(header), includedir);
            }
            foreach(var dir in Directory.GetDirectories(includedir))
            {
                SearchDirForHeaders(dir);
            }
        }
        public void ScanHeaders()
        {
            string includedir = getIncludeDirs();
            SearchDirForHeaders(includedir);
            serializeHeaderDict();
        }
        void serializeHeaderDict()
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite("headers.dict"));
            bw.Write(headerLocations.Count);
            foreach (var h in headerLocations.Keys)
            {
                bw.Write(h);
                bw.Write(headerLocations[h]);
            }
            bw.Close();
        }
        void deserializeHeaderDict()
        {
            if (!File.Exists("headers.dict"))
            {
                ScanHeaders();
            }
            BinaryReader br = new BinaryReader(File.OpenRead("headers.dict"));
            int count = br.ReadInt32();
            Console.WriteLine("there are " + count.ToString() + " headers");
            for(int i = 0; i > count; i++)
            {
                headerLocations.Add(br.ReadString(), br.ReadString());
            }
            br.Close();
        }
        string getIncludeDirs()
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "gcc";
            psi.Arguments = "-print-search-dirs";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            var process = Process.Start(psi);

            process.WaitForExit();

            var line = process.StandardOutput.ReadLine();
            line = process.StandardOutput.ReadLine();
            line = line.Replace("libraries: =", "");
            return line;
        }

        List<string> getTypesFromAHeader(string[] headerlines)
        {
            
            List<string> types = new();
            for(int i = 0; i < headerlines.Length; i++)
            {
                string l = headerlines[i];
                int indentNum = 0;
                for(int z = 0; z < l.Length; z++)
                {
                    if(l[indentNum] == ' ' || l[indentNum] == '\t')
                    {
                        indentNum++;
                    }
                    break;
                }
                l = l.Substring(indentNum, l.Length - indentNum);
                if (l.StartsWith("friend "))
                {
                    l.Replace("friend ", "");
                }
                if (l.EndsWith("{"))
                {
                    l.Remove(l.Length-1, 1);
                }
                if (l.StartsWith("class "))
                {
                    l.Replace("class ", "");
                    types.Add(l);
                }
                if (l.StartsWith("#typedef "))
                {
                    l.Replace("#typedef ", "");
                    var spl = l.Split(" ");
                    l.Replace(spl[spl.Length - 1], "");
                    types.Add(l);
                }
            }
            return types;
        }
    }
}

