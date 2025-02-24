using System.Text;

namespace AutomatonApplication.Models
{
    public class DFA
    {
        public HashSet<string> States { get; }
        public HashSet<char> Alphabet { get; }
        public Dictionary<(string, char), string> Transitions { get; }
        public string InitialState { get; }
        public HashSet<string> FinalStates { get; }

        public DFA(
            HashSet<string> states,
            HashSet<char> alphabet,
            Dictionary<(string, char), string> transitions,
            string initialState,
            HashSet<string> finalStates)
        {
            States = states;
            Alphabet = alphabet;
            Transitions = transitions;
            InitialState = initialState;
            FinalStates = finalStates;

            if (!VerifyAutomaton())
                throw new ArgumentException("Invalid automaton configuration");
        }

        public bool VerifyAutomaton()
        {
            try
            {
                if (!States.Any())  // Verifică dacă mulțimea starilor este vidă
                {
                    return false;
                }
                if (!Alphabet.Any())  // Verifică dacă mulțimea alfabetului de intrare este vidă
                {
                    return false;
                }
                if (!States.Contains(InitialState))   // Verifică dacă starea inițială nu face parte din mulțimea starilor
                {
                    return false;
                }
                HashSet<(string, char)> seen = new HashSet<(string, char)>();                 // nedeterminist
                foreach (var key in Transitions.Keys)
                {
                    if (!seen.Add(key))
                    {
                        return false;
                    }
                }
                foreach (var state in States)  // Verifică dacă multimea starilor și multimea alfabetului de intrare au un elem comun
                {
                    foreach (var character in Alphabet)
                    {
                        if (state == character.ToString())
                        {
                            return false;
                        }

                    }
                }
                if (!Transitions.Keys.Any(transition => transition.Item1 == InitialState))  //verifica daca nu exista nicio tranzitie care sa inceapa cu starea initiala
                {
                    return false;
                }
                if (Transitions.Keys.Any(transition => !States.Contains(transition.Item1) || !Alphabet.Contains(transition.Item2)))  //verifica daca exista o tranzitie care contine alt simbol decat cele din multimea starilor sau multimea alfabetului
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("DFA:");
            sb.AppendLine($"Initial State: {InitialState}");
            sb.AppendLine("States:");
            foreach (var state in States)
            {
                sb.AppendLine($"  {state}");
            }
            sb.AppendLine("Alphabet:");
            foreach (var symbol in Alphabet)
            {
                sb.AppendLine($"  {symbol}");
            }
            sb.AppendLine("Transitions:");
            foreach (var transition in Transitions)
            {
                sb.AppendLine($"  {transition.Key.Item1} --{transition.Key.Item2}--> {transition.Value}");
            }
            sb.AppendLine("Final States:");
            sb.AppendLine($"  {string.Join(", ", FinalStates)}");
            return sb.ToString();
        }
        public bool CheckWord(string? word)
        {
            if (string.IsNullOrEmpty(word))
            {  // Caz special pentru șirul vid
                return FinalStates.Contains(InitialState);
            }
            string currentState = InitialState;
            foreach (char symbol in word)
            {
                if (!Alphabet.Contains(symbol))
                {
                    return false;
                }

                if (!Transitions.TryGetValue((currentState, symbol), out string? nextState))
                {
                    return false;
                }
                currentState = nextState;
            }
            return FinalStates.Contains(currentState);
        }

    }
}
