using System;
namespace Train
{
	public class ImplementationCPlusPlus : IImplementation
	{
        public string LangName { get; set; } = "C++";
        public ImplementationCPlusPlus()
		{
		}

        public string ConvertGlobalNamespace(Namespace global)
        {
            throw new NotImplementedException();
        }

        public string InterpretConst(Const c)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}

