
namespace AutomatonApplication.Models
{

    public class RegularExpression

    {
        private readonly string expression;
        private static readonly HashSet<char> OperatorChars = new HashSet<char> { '*', '+', '|', '(', ')', '?' };

        public RegularExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentException("Expression cannot pe empty");
            }
            this.expression = expression;
            if (!IsValid())
            {
                throw new ArgumentException("Invalid regular expression");
            }
        }
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(expression))
            {
                return false;
            }
            try
            {
                int parantheses = 0;
                for (int i = 0; i < expression.Length; i++)
                {
                    char c = expression[i];
                    if (c == '(')
                    {
                        parantheses++;
                    }
                    else if (c == ')')
                    {
                        parantheses--;
                        if (parantheses < 0)
                        {
                            return false;
                        }
                    }
                    if (c == '.')
                    {
                        return false;
                    }

                }
                return parantheses == 0;
            }
            catch
            {
                return false;
            }
        }

        public string Expression
        {
            get { return expression; }
        }

        public HashSet<char> GetAlphabet()
        {
            var alphabet = new HashSet<char>();
            for (int i = 0; i < expression.Length; i++)
            {
                char c = expression[i];
                if (!OperatorChars.Contains(c))
                {
                    alphabet.Add(c);
                }
            }
            return alphabet;
        }
    }
}
