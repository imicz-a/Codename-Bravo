﻿using System.Text;
using System.Runtime.Serialization;

namespace Train
{
    public static class Parser
    {
        static uint currentline = 1;
        static Namespace globalNamespace = new Namespace() { conname = "Global", contents=new List<ASTElem>()};
        static ushort current = 0;
        static List<Containter> hierarchy = new List<Containter>() { globalNamespace};
        public enum vartype
        {
            Int,
            String,
            Bool,
            Char,
            Float,
            Array,
            Enum,
            Struct,
            Class,
            Delegate
        }
        public static Namespace CreateAST(IEnumerable<string> text)
        {
            foreach (string i in text)
            {
                Interpret(i);
                currentline++;
            }
            var serializer = new YamlDotNet.Serialization.Serializer();
            File.WriteAllText("ast.ast", "");
            var tw = File.AppendText("ast.ast");
            serializer.Serialize(tw, globalNamespace);
            tw.Close();
            return globalNamespace;
        }
        
        static void Interpret(string s)
        {
            if (s == "")
            {
                return;
            }
            string[] context = disectContext(s);
            foreach(var i in context)
            {
                Console.WriteLine("ctx " + i);
            }
            ASTElem elem = null;
            bool goin = false;
            switch (context[0])
            {
                case "var":
                    elem = new VarDeclaration();
                    ((VarDeclaration)elem).varname = context[1];

                    if (isFunction(context[3]))
                    {
                        var v = disectFunctionCall(context[3]);
                        
                        ((VarDeclaration)elem).assignment = v;
                    }
                    else
                    {
                        ((VarDeclaration)elem).type = getType(context[3]);
                        var v = new Const
                        {
                            value = context[3]
                        };
                        ((VarDeclaration)elem).assignment = v;
                    }
                    if (context[2] != "=")
                    {
                        Program.Exit(new SyntaxError("no it won't compile, not pretty enough", currentline));
                    }
                    break;
                case "func":
                    elem = disectFunction(context[1]);
                    goin = true;
                    break;
                case "extern":
                    elem = new ExternCall() { content = context[1] };
                    break;
                case "using":
                    elem = new UsingDirective() { libname = context[1] };
                    break;
                case "for":

                case "}":
                    hierarchy.RemoveAt(current--);
                    Console.WriteLine("current " + current + " hierarchy count " + hierarchy.Count);
                    return;
                default:
                    if (context[0].EndsWith(")"))
                    {
                        elem = disectFunctionCall(context[0]);
                        ((FunctionCall)elem).isStandalone = true;
                        break;
                    }
                    if (lineContainsOperator(s))
                    {
                        elem = disectOperation(context);
                        break;
                    }
                    Program.Exit(new SyntaxError("Not an apropriate expression", currentline));
                    break;

            }
            if(elem == null)
            {
                return;
            }
            Console.WriteLine("conname " + hierarchy[current].conname);
            hierarchy[current].contents.Add(elem);
            if (goin)
            {
                current++;
                hierarchy.Add((Containter)elem);
            }
        }
        static bool lineContainsOperator(string str)
        {
            foreach(var f in Operation.operators)
            {
                if (str.Contains(f))
                {
                    return true;
                }
            }
            return false;
        }

        static vartype getType(string var)
        {
            if (var.Contains("\""))
            {
                return vartype.String;
            }
            if (var.Contains("'"))
            {
                return vartype.Char;
            }
            if(var == "true" || var == "false")
            {
                return vartype.Bool;
            }
            if (int.TryParse(var, out _))
            {
                return vartype.Int;
            }
            if(double.TryParse(var, out _))
            {
                return vartype.Float;
            }
            return vartype.Class;
        }

        static LineOperation disectOperation(string[] ctx)
        {
            LineOperation op = new LineOperation();
            for (int i = 0; i < ctx.Length; i++)
            {
                if (Operation.operators.Contains(ctx[i]))
                {
                    op.operations.Add(new Operation() { otype = (Operation.operatorType)Operation.operators.ToList().IndexOf(ctx[i])});
                }
                else
                {
                    op.operations.Add(new OperationVar() { varname = ctx[i] });
                }
            }
            return op;
        }
        static Tuple<string, List<dynamic>> interpretFunction(string str)
        {
            string[] strs = null;
            for(int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    strs = new string[2];
                    strs[0] = str.Substring(0, i);
                    strs[1] = str.Substring(i+1, str.Length - i-2);
                    Console.WriteLine("0 " + strs[0]);
                    Console.WriteLine("1 " + strs[1]);
                    break;
                }
            }
            if(strs == null)
            {
                Program.Exit(new SyntaxError("That is definitely not a function", currentline));
            }
            List<dynamic> arguments = strs[1].Split(',').ToList().OfType<dynamic>().ToList();
            for(int i = 0; i < arguments.Count; i++)
            {
                Console.WriteLine("arg "+arguments[i]);
                if (isFunction(arguments[i]))
                {
                    Console.WriteLine("function found inside!");
                    arguments[i] = interpretFunction(arguments[i]);
                }
            }
            return Tuple.Create(strs[0], arguments);
        }

        static ForLoop disectFor()
        {
            var fl = new ForLoop();
            return fl;
        }

        static Func disectFunction(string str)
        {
            if (str.EndsWith('{'))
            {
                str = str.Remove(str.Length-1);
            }
            string[] strs = null;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    strs = new string[2];
                    strs[0] = str.Substring(0, i);
                    strs[1] = str.Substring(i + 1, str.Length - i - 2);
                    Console.WriteLine("0 " + strs[0]);
                    Console.WriteLine("1 " + strs[1]);
                    break;
                }
            } 
            Func f = new Func();
            f.conname = strs[0];
            //strs[0] - function name
            //strs[1] - function arguments, sth like "type name" ex "int i"
            strs[1].Split(",").ToList();
            List<Tuple<string, string>> args = new List<Tuple<string, string>>();
            foreach(var s in strs[1])
            {
                string[] typeplusname = strs[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                args.Add(Tuple.Create(typeplusname[0], typeplusname[1]));
            }
            f.arguments = args;
            return f;
        }
        static FunctionCall disectFunctionCall(string str)
        {
            if (str.EndsWith('{'))
            {
                str = str.Remove(str.Length - 1);
            }
            string[] strs = null;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    strs = new string[2];
                    strs[0] = str.Substring(0, i);
                    strs[1] = str.Substring(i + 1, str.Length - i - 2);
                    Console.WriteLine("0 " + strs[0]);
                    Console.WriteLine("1 " + strs[1]);
                    break;
                }
            }
            FunctionCall fcall = new FunctionCall();
            fcall.funcName = strs[0];
            strs[1] = strs[1].Replace(" ", "");
            fcall.arguments = strs[1].Split(",").ToList();
            return fcall;
        }

        static bool isFunction(string s)
        {
            bool hasNawias = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\"')
                {
                    break;
                }
                if (c == '(')
                {
                    hasNawias = true;
                    break;
                }
            }
            if (!hasNawias)
            {
                return false;
            }
            bool result = false;
            Console.WriteLine("checking " + s);
            for (int i = s.Length-1; i >= 0; i--)
            {
                char c = s[i];
                if (c == '\"')
                {
                    break;
                }
                if (c == ')')
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        static string[] disectArguments(string str)
        {
            str = "y" + str;
            byte exitcharcount = 0;
            bool isInsideString = false;
            int previousindex = 0;
            bool isPrevCharBackslash = false;

            List<string> ctx = new List<string>();

            char[] startchars = new char[] { '(', '{', '[' };
            char[] endchars = new char[] { ')', '}', ']' };
            for (int i = 0; i < str.Length; i++)
            {
            skip:
                char c = str[i];
                if (!isInsideString)
                {
                    if (c == ',' && exitcharcount == 0)
                    {
                        ctx.Add(str.Substring(previousindex + 1, i - previousindex - 1));
                        previousindex = i;
                    }
                    else if (startchars.Contains(c))
                    {
                        exitcharcount++;
                    }
                    else if (endchars.Contains(c))
                    {
                        exitcharcount--;
                    }
                }
                else if (c == '\"' && !isPrevCharBackslash)
                {
                    isInsideString = !isInsideString;
                }
                else if (c == '\\')
                {
                    isPrevCharBackslash = true;
                    goto skip;
                }
                isPrevCharBackslash = false;
            }
            ctx.Add(str.Substring(previousindex + 1, str.Length - previousindex - 1));
            int startindex = 0;
            for (int i = 0; i < ctx.Count; i++)
            {
                if (ctx[i] == "")
                {
                    startindex++;
                    continue;
                }
                else if (ctx[i] == "\t")
                {
                    startindex++;
                    continue;
                }
                break;
            }
            List<string> buffer = new List<string>();
            Console.WriteLine("startindex " + startindex.ToString());
            for (int i = startindex; i < ctx.Count; i++)
            {
                buffer.Add(ctx[i]);
            }
            return buffer.ToArray();
        }
        static string[] disectContext(string str)
        {
            str = "y" + str;
            byte exitcharcount = 0;
            bool isInsideString = false;
            int previousindex = 0;
            bool isPrevCharBackslash = false;

            List<string> ctx = new List<string>();

            char[] startchars = new char[] {'(', '{', '['};
            char[] endchars = new char[] { ')', '}', ']' };
            for (int i = 0; i < str.Length; i++)
            {
                skip:
                char c = str[i];
                if (!isInsideString)
                {
                    if (c == ' ' && exitcharcount == 0)
                    {
                        ctx.Add(str.Substring(previousindex+1, i - previousindex- 1));
                        previousindex = i;
                    }
                    else if (startchars.Contains(c))
                    {
                        exitcharcount++;
                    }
                    else if (endchars.Contains(c))
                    {
                        exitcharcount--;
                    }
                }
                else if (c == '\"' || isPrevCharBackslash)
                {
                    isInsideString = !isInsideString;
                }
                else if (c == '\\')
                {
                    isPrevCharBackslash = true;
                    goto skip;
                }
                isPrevCharBackslash = false;
            }
            ctx.Add(str.Substring(previousindex+1, str.Length-previousindex-1));
            int startindex = 0;
            for(int i = 0; i < ctx.Count; i++)
            {
                if (ctx[i] == "")
                {
                    startindex++;
                    continue;
                }
                else if (ctx[i] == "\t")
                {
                    startindex++;
                    continue;
                }
                break;
            }
            List<string> buffer = new List<string>();
            Console.WriteLine("startindex " + startindex.ToString());
                for(int i = startindex; i < ctx.Count; i++)
            {
                buffer.Add(ctx[i]);
            }
            return buffer.ToArray();
        }
        
    }
}

