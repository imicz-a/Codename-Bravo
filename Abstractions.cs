using System;
using System.Xml;
using System.Xml.Serialization;

namespace Train
{
	[Serializable]
	public abstract class ASTElem
	{
		public enum ElementType
		{
			VarDeclaration,
			FunctionCall,
			Var,
			VarVar,
			Const,
			Assignment,
			Container,
			Func,
			Namespace,
			LineOperation,
			Operation,
			OperationVar,
			ExternCall,
			UsingDirective,
			ForLoop,
			WhileLoop,
			ForeachLoop,
			ReturnStatement
		}
        public virtual ElementType elemtype{ get; protected set; }
    }

	[Serializable]
	public class VarDeclaration : ASTElem
	{
		public override ElementType elemtype { get; protected set; } = ElementType.VarDeclaration;
        public string varname { get; set; }
		public ASTElem assignment { get; set; }
		public Parser.vartype type { get; set; }
	}
	[Serializable]
	public class UsingDirective : ASTElem
	{
		public override ElementType elemtype { get; protected set; } = ElementType.UsingDirective;
        public string libname;
	}
	[Serializable]
	public class ForLoop : Containter
	{
		public VarVar indexIterator;
		public int startval = 0;
		public Tuple<Operation, Const> check = Tuple.Create(new Operation() { otype=Operation.operatorType.lessthan}, new Const());
		public int step = 1;
	}
	[Serializable]
	public class WhileLoop : Containter
	{
		public Tuple<Var, Operation, Var> args;
	}
	[Serializable]
	public class ForeachLoop : Containter
	{
		public VarVar iterationvar;
		public Var enumerable;
	}
	[Serializable]
	class Return : ASTElem
	{
		public Var value;
		public string type;
	}
	[Serializable]
	public class ExternCall : ASTElem
	{
		public override ElementType elemtype { get; protected set; } = ElementType.ExternCall;
        public string content { get; set; }
	}
	[Serializable]
	public class FunctionCall : Var
	{
        public override ElementType elemtype { get; protected set; } = ElementType.FunctionCall;
        public string funcName { get; set; }
        public List<string> arguments { get; set; }
		public bool isStandalone { get; set; }
	}
	[Serializable]
	public class Var : ASTElem
	{
        public override ElementType elemtype { get; protected set; } = ElementType.Var;
    }
	[Serializable]
	public class VarVar : Var
	{
        public override ElementType elemtype { get; protected set; } = ElementType.VarVar;
        public string varname { get; set; }
		public string value { get; set; }
	}
	[Serializable]
	public class Const : Var
	{
        public override ElementType elemtype { get; protected set; } = ElementType.Const;
        public string value { get; set; }
	}
	[Serializable]
	public class Containter : ASTElem
	{
        public override ElementType elemtype { get; protected set; } = ElementType.Container;
        public string conname { get; set; }
		public List<ASTElem> contents { get; set; } = new List<ASTElem>();
		public List<string> modifiers { get; set; } = new List<string>();
    }
	[Serializable]
	public class Func : Containter
	{
        public override ElementType elemtype { get; protected set; } = ElementType.Func;
        public List<Tuple<string, string>> arguments { get; set; }
		public string returntype { get; set; }
	}
	[Serializable]
	public class Namespace : Containter
	{
        public override ElementType elemtype { get; protected set; } = ElementType.Namespace;
    }
	[Serializable]
	public class LineOperation : ASTElem
	{
        public override ElementType elemtype { get; protected set; } = ElementType.LineOperation;
		public List<Operation> operations { get; set; } = new List<Operation>();
	}
	[Serializable]
	public class Operation : ASTElem
	{
        public override ElementType elemtype { get; protected set; } = ElementType.Operation;
        public static string[] operators = new string[] { "=", "+", "-", "+=", "--", "<", "<=", ">", ">=", "==" };
		public enum operatorType
		{
			assignment,
			plus,
			minus,
			increment,
			decrement,
			lessthan,
			lessequalthan,
			morethan,
			moreequalthan,
			equal
		}
		public operatorType otype { get; set; }
	}
	[Serializable]
	public class OperationVar : Operation
	{
        public override ElementType elemtype { get; protected set; } = ElementType.OperationVar;
        public string varname { get; set; }
	}
}

