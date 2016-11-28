using System.Threading;

namespace Spark2Razor.Rules
{
    public class EachArguments
    {
        private static int _index = -1;
        private readonly string _variable;

        public static bool InTest { get; set; }

        public EachArguments(EachExpression expression, string text)
        {
            IsFirst = text.Contains(expression.Variable + "IsFirst");
            IsOdd = text.Contains(expression.Variable + "IsOdd");
            IsEven = text.Contains(expression.Variable + "IsEven");
            HasIndex = text.Contains(expression.Variable + "Index");

            if (IsFirst || IsOdd || IsEven || HasIndex)
            {
                if (!InTest || _index < 0)
                {
                    Interlocked.Increment(ref _index);
                }

                _variable = $"__i{_index}";

                Initialization = $"\r\n@{{ var {_variable} = 0; }}";
                Increment = $"\r\n\t{_variable}++;";
            }

            if (HasIndex)
            {
                Declaration += DeclareVariable(expression.Variable, "Index", $"{_variable}");
            }

            if (IsFirst)
            {
                Declaration += DeclareVariable(expression.Variable, "IsFirst", $"({_variable} == 0)");
            }

            if (IsEven)
            {
                Declaration += DeclareVariable(expression.Variable, "IsEven", $"({_variable} % 2 == 0)");
            }

            if (IsOdd)
            {
                Declaration += DeclareVariable(expression.Variable, "IsOdd", $"({_variable} % 2 == 1)");
            }
        }

        private string DeclareVariable(string variable, string suffix, string expression)
        {
            return $"\r\n\tvar {variable}{suffix} = {expression};";
        }

        public bool IsEven { get; }
        public bool HasIndex { get; }
        public bool IsOdd { get; }
        public bool IsFirst { get; }
        public string Initialization { get; private set; }
        public string Declaration { get; }
        public string Increment { get; private set; }
    }
}
