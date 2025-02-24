using AutomatonApplication.Models;
using System.Text;


namespace AutomatonApplication.Converters
{
    public class RegexToNFA
    {
        private readonly string regex;
        public RegexToNFA(string regex)
        {
            this.regex = AddConcatenationOperator(regex ?? throw new ArgumentNullException(nameof(regex)));

        }
        private static string AddConcatenationOperator(string input)
        {
            var result = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                result.Append(input[i]);
                if (i < input.Length - 1)
                {
                    char current = input[i];
                    char next = input[i + 1];
                    if ((IsOperand(current) || current == '*' || current == '+' || current == '?' || current == ')') &&
                        (IsOperand(next) || next == '('))
                    {
                        result.Append('.');
                    }
                }

            }
            return result.ToString();
        }
        private static bool IsOperand(char c)
        {
            return !IsOperator(c);
        }
        private static bool IsOperator(char c)
        {
            return c == '|' || c == '.' || c == '*' || c == '+' || c == '(' || c == ')' || c == '?';
        }

        private static int Precedence(char op)
        {
            switch (op)
            {
                case '|':
                    return 1;
                case '.':
                    return 2;
                case '*':
                case '+':
                case '?':
                    return 3;
                default:
                    return 0;
            }
        }
        public string ToPostfix()
        {
            var output = new StringBuilder();
            var operators = new Stack<char>();
            foreach (char c in regex)
            {
                if (IsOperand(c))
                {
                    output.Append(c);
                }
                else if (c == '(')
                {
                    operators.Push(c);
                }
                else if (c == ')')
                {
                    while (operators.Count > 0 && operators.Peek() != '(')
                    {
                        output.Append(operators.Pop());
                    }
                    operators.Pop();
                }
                else
                {
                    while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(c))
                    {
                        output.Append(operators.Pop());
                    }
                    operators.Push(c);
                }
            }

            while (operators.Count > 0)
            {
                output.Append(operators.Pop());
            }

            return output.ToString();
        }

        public NFA BuildNFAFromPostfix(string postfix)
        {
            var stack = new Stack<NFA>();
            foreach (char c in postfix)
            {
                if (IsOperand(c))
                {
                    stack.Push(NFA.CreateBasic(c));
                }
                else
                {
                    switch (c)
                    {
                        case '.':
                            var nfa2 = stack.Pop();
                            var nfa1 = stack.Pop();
                            stack.Push(NFA.Concatenate(nfa1, nfa2));
                            break;
                        case '|':
                            nfa2 = stack.Pop();
                            nfa1 = stack.Pop();
                            stack.Push(NFA.Union(nfa1, nfa2));
                            break;
                        case '*':
                            stack.Push(NFA.Kleene(stack.Pop()));
                            break;
                        case '+':
                            stack.Push(NFA.Plus(stack.Pop()));
                            break;
                        case '?':
                            stack.Push(NFA.Optional(stack.Pop()));
                            break;

                    }
                }
            }
            return stack.Pop();
        }
        public override string ToString()
        {
            return $"Processed Regex: {regex}";
        }
    }
}
