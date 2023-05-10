using System;
namespace Train
{
    public class ImplementationCSharp : IImplementation
    {
        public string LangName { get; set; } = "C#";

        public string Interpret(ASTElem elem)
        {
            switch (elem.elemtype)
            {
                case ASTElem.ElementType.VarDeclaration:
                    return InterpretVarDeclaration((VarDeclaration)elem);
                case ASTElem.ElementType.FunctionCall:
                    return InterpretFunctionCalls((FunctionCall)elem);
                case ASTElem.ElementType.Var:
                    return InterpretVar((Var)elem);
                case ASTElem.ElementType.VarVar:
                    return InterpretVarVar((VarVar)elem);
                case ASTElem.ElementType.Const:
                    return InterpretConst((Const)elem);
                case ASTElem.ElementType.Container:
                    return InterpretContainer((Containter)elem);
                case ASTElem.ElementType.Func:
                    return InterpretFunc((Func)elem);
                case ASTElem.ElementType.Namespace:
                    return InterpretNamespace((Namespace)elem);
                case ASTElem.ElementType.LineOperation:
                    return InterpretLineOperation((LineOperation)elem);
                case ASTElem.ElementType.Operation:
                    return InterpretOperation((Operation)elem);
                case ASTElem.ElementType.OperationVar:
                    return InterpretOperationVar((OperationVar)elem);
                case ASTElem.ElementType.ExternCall:
                    return InterpretExternCall((ExternCall)elem);
                case ASTElem.ElementType.UsingDirective:
                    return InterpretUsingDirective((UsingDirective)elem);
                default:
                    return "";
            }
        }

        public string InterpretConst(Const c)
        {
            return c.value;
        }

        public string InterpretContainer(Containter c)
        {
            throw new TypeLoadException();
        }

        public string InterpretFunc(Func f)
        {
            string code = "";
            foreach(var m in f.modifiers)
            {
                code += m;
            }
            code += "void " + f.conname;
            code += "(";
            foreach(var a in f.arguments)
            {
                code += a.Item1;
                code += " ";
                code += a.Item2;
            }
            code += "){\n";
            foreach(var c in f.contents)
            {
                code += Interpret(c); 
            }
            code += "}\n";
            return code;

        }

        public string InterpretFunctionCalls(FunctionCall fc)
        {
            string code = fc.funcName + "(";
            foreach(var i in fc.arguments)
            {
                code += i;
            }
            code += ")";
            if (fc.isStandalone)
            {
                code += ";\n";
            }
            return code;
        }

        public string InterpretLineOperation(LineOperation lo)
        {
            string code = "";
            foreach (var o in lo.operations)
            {
                code += Interpret(o);
            }
            code += ";";
            return code;
        }

        public string InterpretNamespace(Namespace n)
        {
            string code = "static class " + n.conname + "{\n";
            foreach(var c in n.contents)
            {
                code += Interpret(c);
            }
            code += "}\n";
            return code;
        }

        public string InterpretOperation(Operation o)
        {
            return Operation.operators[(int)o.otype];
        }

        public string InterpretOperationVar(OperationVar ov)
        {
            return ov.varname;
        }

        public string InterpretVar(Var v)
        {
            throw new TypeLoadException();
        }

        public string InterpretVarDeclaration(VarDeclaration vd)
        {
            return getStringType(vd.type, vd.assignment) + " " + vd.varname + " = " + Interpret(vd.assignment) + ";\n";
        }

        public string InterpretVarVar(VarVar vv)
        {
            return vv.varname;
        }
        string getStringType(Parser.vartype vartype, ASTElem assignment)
        {
            string[] vartypes = new string[] { "int", "string", "bool", "char", "float"};
            if((int)vartype < 6)
            {
                return vartypes[(int)vartype];
            }
            return "var";
        }
        public string InterpretExternCall(ExternCall ex)
        {
            return ex.content + ";";
        }
        public string ConvertGlobalNamespace(Namespace global)
        {
            string code = "";
            foreach (var c in global.contents)
            {
                code += Interpret(c);
            }
            return code;
        }
        public string InterpretUsingDirective(UsingDirective u)
        {
            return "using TrainBaseLib." + u.libname + ";";
        }
    }
}

