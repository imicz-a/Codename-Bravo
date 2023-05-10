using System;
namespace Train
{
	public interface IImplementation
	{
		public string LangName { get; protected set; }
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
		string InterpretVarDeclaration(VarDeclaration vd);
		string InterpretFunctionCalls(FunctionCall fc);
		string InterpretVar(Var v);
		string InterpretVarVar(VarVar vv);
		string InterpretConst(Const c);
		string InterpretContainer(Containter c);
		string InterpretFunc(Func f);
		string InterpretNamespace(Namespace n);
		string InterpretLineOperation(LineOperation lo);
		string InterpretOperation(Operation o);
		string InterpretOperationVar(OperationVar ov);
		string InterpretExternCall(ExternCall ex);
		string InterpretUsingDirective(UsingDirective u);
		string InterpretForLoop(ForLoop fl);
		string InterpretWhileLoop(WhileLoop wl);
		string InterpretForeachLoop(ForeachLoop fl);
		string InterpretReturn(Return);
	}
}

