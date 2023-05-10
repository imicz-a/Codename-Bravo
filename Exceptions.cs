
namespace Train
{
    abstract class exception
    {
        public string obj;
        public uint line;
    }
    class SyntaxError : exception
    {
        public SyntaxError(string obj, uint line)
        {
            this.obj = obj;
            this.line = line;
        }
        public override string ToString()
        {
            return "Syntax error in line " + line.ToString() + ": " + obj;
        }
    }
}