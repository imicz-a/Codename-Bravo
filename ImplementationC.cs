using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Train
{
	public class ImplementationCpp : IImplementation
	{
		public string LangName { get; set; } = "C++";
		public TrainProject proj;
		string code = "";
		public string header = "#include \"trainBaseLib.h\"\n" +
			"extern \"C\"{";
		List<Containter> hierarchy = new List<Containter>();
		bool isInFunction;
		bool isInsideAClass;

		public ImplementationCpp()
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
			code += "}";
			header += "}";
			return code;
		}

		public string InterpretConst(Const c)
		{
			code += c.value;
			return "";
		}

		public string InterpretContainer(Containter c)
		{
			throw new NotImplementedException();
		}

		public string InterpretExternCall(ExternCall ex)
		{
			code += ex.content;
			return null;
		}

		public string InterpretForeachLoop(ForeachLoop fl)
		{
			code += "for(" + "auto" + fl.iterationvar + ":" + fl.enumerable+ "){\n";
			foreach(var c in fl.contents)
			{
				Interpret(c);
			}
			code += "}";
			throw new NotImplementedException();
		}

		public string InterpretForLoop(ForLoop fl)
		{
			string code = "for(";
			code += "int " + fl.indexIterator.varname + "=" + fl.startval + "; ";
			code += fl.indexIterator.varname + Operation.operators[(uint)fl.check.Item1.otype] + fl.check.Item2.value + "; ";
			code += fl.indexIterator.varname + "+=" + fl.step + "){\n";
			foreach (var i in fl.contents)
			{
				Interpret(i);
			}
			code += "}";
			return null;
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

		public string InterpretClass(Class c)
		{
			return null;
		}
		public string InterpretWhileLoop(WhileLoop wl)
		{
			string code = "while(";
			code += wl.op.ToString();
			code += "){\n";
			foreach (var v in wl.contents)
			{
				Interpret(v);
			}
			code += "}\n";
			return code;
		}

		public string mangle(string nm)
		{
			string mang = "";
			foreach (var v in hierarchy)
			{
				mang += v.conname + "dot";
			}
			mang += nm;
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
							Checker.types = Checker.types.Concat(getTypesAndFunctionsFromAHeader(File.ReadAllLines(headerLocations[headername] + "/" + headername))).ToList();
						}
						else
						{
							foreach (var s in proj.sourceFiles)
							{
								if (Path.GetFileName(s) == headername + ".train")
								{
									Checker.types = Checker.types.Concat(getTypesAndFunctionsFromAHeader(File.ReadAllLines(header+".il.h"))).ToList();
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

		List<string> getTypesAndFunctionsFromAHeader(string[] headerlines)
		{
			List<string> types = new();
			for(int i = 0; i < headerlines.Length; i++)
			{
				string l = headerlines[i];
				l = removeIndent(l);
				if (l.StartsWith("friend "))
				{
					l.Replace("friend ", "");
				}
				if (l.EndsWith("{"))
				{
					l.Remove(l.Length-1, 1);
				}
				if (l.EndsWith(";"))
				{
                    l.Remove(l.Length - 1, 1);
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
				if (l.EndsWith(")"))
				{
					var ctx = Parser.disectContext(l);
					funcsandreturns.Add(getFuncName(ctx[1]), ctx[0]);
				}
			}
			return types;
		}
		public Dictionary<string, string> varsandtypes = new();
		Dictionary<string, string/*this is the first assignment*/> undeterminedVars = new();
		public Dictionary<string, string> funcsandreturns = new();
		Dictionary<string, string/*this is the first return*/> undeterminedFuncs = new();
		public void preprocessFunctions()
		{
			foreach (var source in proj.sourceFiles)
			{
				bool isInAFunction = false;
				string funcInName = null;
				var lines = File.ReadAllLines(source);
				for (int i = 0; i < lines.Length; i++)
				{
					var l = lines[i];
					var ctx = Parser.disectContext(l);
					if (ctx[0] == "func")
					{
						string funcname = getFuncName(ctx[1]);

						if (funcname == null)
						{
							Console.WriteLine("Well, that function is not a function");
							continue;
						}

						isInAFunction = true;
						funcInName = funcname;

					}
					if (ctx[0] == "return")
					{
						if (!isInAFunction)
						{
							Console.WriteLine("return outside of function");
							continue;
						}
						
					}
					if (ctx[0] == "}")
					{
						isInAFunction = false;
						funcInName = null;
					}
				}
			}
		}
		public string GetReturnType(string str)
		{
			if (Checker.types.Contains(getFuncName(str))) {

			}
			else if (Parser.isFunction(str))
			{

			}
			else
			{

			}
		}
		public void preprocessVars()
		{
			List<string> varsToTypeCheck = new List<string>();
			foreach (var source in proj.sourceFiles)
			{
				var lines = File.ReadAllLines(source);
				for (int i = 0; i < lines.Length; i++)
				{
					var l = lines[i];
					l = removeIndent(l);
					var ctx = Parser.disectContext(l);
					if (ctx[0] == "var")
					{
						varsToTypeCheck.Add(ctx[1]);
					}
					else if (Parser.isTypeName(ctx[0]))
					{
						varsandtypes.Add(ctx[1], ctx[0]);
					}
					else if (varsToTypeCheck.Contains(ctx[0]))
					{
						if (ctx[1] != "=")
						{
							Console.WriteLine("There has to be space after and before equals");
							return;
						}
						if (ctx[2].EndsWith(")"))
						{
							
							break;
						}
					}
					
				}
			}
		}
		string removeIndent(string l)
		{
			int indentNum = 0;
			for (int z = 0; z < l.Length; z++)
			{
				if (l[indentNum] == ' ' || l[indentNum] == '\t')
				{
					indentNum++;
				}
			}
			return l.Substring(indentNum, l.Length - indentNum);
		}
		string getFuncName(string func)
		{
            for (int z = 0; z < func.Length; z++)
            {
                if (func[z] == '(')
                {
                    return func.Substring(0, z);
                }
            }
			return null;
        }
	}
}

