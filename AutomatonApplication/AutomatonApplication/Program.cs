using AutomationApplication.Converters;
using AutomatonApplication.Converters;
using AutomatonApplication.Models;

namespace AutomatonApplication
{
    class Program
    {
        public static string ReadRegexFromFile()
        {
            const string fileName = "regex.txt";
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException($"File not found");
            }
            string content = File.ReadAllText(fileName).Trim();
            return content;
        }
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=====================================");
                Console.WriteLine("Regular Expression to DFA Converter");
                Console.WriteLine("=====================================\n");

                string regexString = ReadRegexFromFile();

                RegularExpression regex;
                try
                {
                    regex = new RegularExpression(regexString);
                    Console.WriteLine("Regular expression is valid!");

                }
                catch (ArgumentException exception)
                {
                    Console.WriteLine($"{exception.Message}");
                    return;
                }

                var converterNfa = new RegexToNFA(regexString); 
                var postfix = converterNfa.ToPostfix();
                var nfa = converterNfa.BuildNFAFromPostfix(postfix);

                var converterDfa = new NfaToDfa(nfa);
                var dfa = converterDfa.Convert();

                RunMainMenu(regex, dfa);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn unexpected error occurred {ex.Message}");
                if (ex is FileNotFoundException)
                {
                    Console.WriteLine("\nPlease ensure 'regex.txt' exists in the application directory.");
                }
            }


        }
        private static void RunMainMenu(RegularExpression regex, DFA dfa)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("\nMenu:");
                    Console.WriteLine("1. Display regular expression");
                    Console.WriteLine("2. Display automaton");
                    Console.WriteLine("3. Check word");
                    Console.WriteLine("4. Exit");
                    Console.Write("\nChoose an option: ");

                    string choice = Console.ReadLine()?.Trim() ?? "";
                    Console.WriteLine();

                    switch (choice)
                    {
                        case "1":
                            DisplayRegularExpression(regex);
                            break;

                        case "2":
                            DisplayAutomaton(dfa);
                            break;

                        case "3":

                            CheckWord(dfa);
                            break;

                        case "4":
                            Console.WriteLine("Thank you for using the application!");
                            return;

                        default:
                            Console.WriteLine("Invalid option! Please try again.");
                            break;

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nError: {ex.Message}");
                }
            }
        }
        private static void DisplayRegularExpression(RegularExpression regex)
        {
            Console.WriteLine("Regular Expression:");
            Console.WriteLine($"  {regex.Expression}");
            var alphabet = regex.GetAlphabet();
            Console.WriteLine("\nAlphabet:");
            Console.WriteLine($"  {{{string.Join(", ", alphabet)}}}");
        }
        private static void DisplayAutomaton(DFA dfa)
        {
            string automatonString = dfa.ToString();
            Console.WriteLine(automatonString);
            try
            {
                File.WriteAllText("automaton.txt", automatonString);
                Console.WriteLine("\nAutomaton has been saved to 'automaton.txt'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nWarning: Could not save automaton to file: {ex.Message}");
            }
        }
        private static void CheckWord(DFA dfa)
        {
            while (true)
            {
                Console.Write("Enter word to check (press Enter for a null input): ");
                string? word = Console.ReadLine();

                try
                {
                    bool isAccepted = dfa.CheckWord(word);
                    Console.WriteLine($"Word '{word}' is {(isAccepted ? "accepted" : "not accepted")} by the automaton.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking word: {ex.Message}");
                }

                Console.WriteLine("\nWould you like to check another word? (y/n): ");
                string response = Console.ReadLine()?.Trim().ToLower() ?? "n"; // Convertim null în "n" pentru a opri bucla

                if (response != "y" && response != "yes")
                {
                    break;  // Ieși din buclă dacă răspunsul nu este "y" sau "yes"
                }
            }
        }
    }
}
