
namespace AutomatonApplication.Models
{
    public class NFA
    {
        private static int stateCounter = 0;
        public HashSet<string> States { get; private set; }
        public HashSet<char> Alphabet { get; private set; }
        public Dictionary<(string, char?), HashSet<string>> Transitions { get; private set; }
        public string InitialState { get; private set; }
        public HashSet<string> FinalStates { get; private set; }

        public NFA()
        {
            States = new HashSet<string>();
            Alphabet = new HashSet<char>();
            Transitions = new Dictionary<(string, char?), HashSet<string>>();
            InitialState = GenerateState(); // Generează automat `InitialState`
            States.Add(InitialState); // Adaugă `InitialState` în `States`
            FinalStates = new HashSet<string>();
        }

        private static string GenerateState() => $"s{stateCounter++}";
        private static void DecrementState()
        {
            stateCounter--;
        }

        public static NFA CreateBasic(char symbol)
        {
            var nfa = new NFA();
            var finalState = GenerateState();
            nfa.States.Add(finalState);
            nfa.FinalStates.Add(finalState);
            nfa.Alphabet.Add(symbol);
            nfa.Transitions[(nfa.InitialState, symbol)] = new HashSet<string> { finalState };

            return nfa;
        }

        //public static NFA CreateEmpty()
        //{
        //    var nfa = new NFA();
        //    nfa.FinalStates.Add(nfa.InitialState);
        //    return nfa;
        //}

        public static NFA Concatenate(NFA first, NFA second)
        {
            var result = new NFA();
            result.States.Remove(result.InitialState);
            DecrementState();
            foreach (var nfa in new[] { first, second })
            {
                foreach (var state in nfa.States)
                    result.States.Add(state);
                foreach (var symbol in nfa.Alphabet)
                    result.Alphabet.Add(symbol);
                foreach (var transition in nfa.Transitions)
                    result.Transitions[transition.Key] = new HashSet<string>(transition.Value);
            }
            foreach (var finalState in first.FinalStates)
            {
                if (!result.Transitions.ContainsKey((finalState, null)))
                    result.Transitions[(finalState, null)] = new HashSet<string>();

                result.Transitions[(finalState, null)].Add(second.InitialState);
            }
            result.InitialState = first.InitialState;
            result.FinalStates = new HashSet<string>(second.FinalStates);

            return result;
        }

        public static NFA Union(NFA first, NFA second)
        {
            var result = new NFA();
            foreach (var nfa in new[] { first, second })
            {
                foreach (var state in nfa.States)
                    result.States.Add(state);
                foreach (var symbol in nfa.Alphabet)
                    result.Alphabet.Add(symbol);
                foreach (var transition in nfa.Transitions)
                    result.Transitions[transition.Key] = new HashSet<string>(transition.Value);
            }
            result.Transitions[(result.InitialState, null)] = new HashSet<string>
            {
                first.InitialState,
                second.InitialState
            };
            var newFinalState = GenerateState();
            result.States.Add(newFinalState);
            foreach (var finalState in first.FinalStates)
            {
                if (!result.Transitions.ContainsKey((finalState, null)))
                {
                    result.Transitions[(finalState, null)] = new HashSet<string>();
                }
                result.Transitions[(finalState, null)].Add(newFinalState);
            }

            foreach (var finalState in second.FinalStates)
            {
                if (!result.Transitions.ContainsKey((finalState, null)))
                {
                    result.Transitions[(finalState, null)] = new HashSet<string>();
                }
                result.Transitions[(finalState, null)].Add(newFinalState);
            }
            result.FinalStates = new HashSet<string> { newFinalState };
            return result;
        }

        public static NFA Kleene(NFA nfa)
        {
            var result = new NFA();

            foreach (var state in nfa.States)
            {
                result.States.Add(state);
            }
            foreach (var symbol in nfa.Alphabet)
            {
                result.Alphabet.Add(symbol);
            }
            foreach (var transition in nfa.Transitions)
            {
                result.Transitions[transition.Key] = new HashSet<string>(transition.Value);
            }
            var newFinalState = GenerateState();
            result.States.Add(newFinalState);
            result.Transitions[(result.InitialState, null)] = new HashSet<string>
            {
                nfa.InitialState,
                newFinalState
            };
            foreach (var finalState in nfa.FinalStates)
            {
                if (!result.Transitions.ContainsKey((finalState, null)))
                {
                    result.Transitions[(finalState, null)] = new HashSet<string>();
                }
                result.Transitions[(finalState, null)].Add(nfa.InitialState);
                result.Transitions[(finalState, null)].Add(newFinalState);
            }
            result.FinalStates = new HashSet<string> { newFinalState };
            return result;
        }

        public static NFA Plus(NFA nfa)
        {
            var result = new NFA();

            foreach (var state in nfa.States)
            {
                result.States.Add(state);
            }
            foreach (var symbol in nfa.Alphabet)
            {
                result.Alphabet.Add(symbol);
            }
            foreach (var transition in nfa.Transitions)
            {
                result.Transitions[transition.Key] = new HashSet<string>(transition.Value);
            }
            var newFinalState = GenerateState();
            result.States.Add(newFinalState);
            result.Transitions[(result.InitialState, null)] = new HashSet<string> { nfa.InitialState };
            foreach (var finalState in nfa.FinalStates)
            {
                if (!result.Transitions.ContainsKey((finalState, null)))
                {
                    result.Transitions[(finalState, null)] = new HashSet<string>();
                }
                result.Transitions[(finalState, null)].Add(nfa.InitialState);
                result.Transitions[(finalState, null)].Add(newFinalState);
            }
            result.FinalStates = new HashSet<string> { newFinalState };
            return result;
        }

        public static NFA Optional(NFA nfa)
        {
            var result = new NFA();
            foreach (var state in nfa.States)
            {
                result.States.Add(state);
            }
            foreach (var symbol in nfa.Alphabet)
            {
                result.Alphabet.Add(symbol);
            }
            foreach (var transition in nfa.Transitions)
            {
                result.Transitions[transition.Key] = new HashSet<string>(transition.Value);
            }
            result.FinalStates = new HashSet<string>(nfa.FinalStates);
            result.FinalStates.Add(result.InitialState);
            if (!result.Transitions.ContainsKey((result.InitialState, null)))
            {
                result.Transitions[(result.InitialState, null)] = new HashSet<string>();
            }
            result.Transitions[(result.InitialState, null)].Add(nfa.InitialState);
            return result;
        }




        //public override string ToString() //////////////
        //{
        //    var sb = new StringBuilder();
        //    sb.AppendLine("NFA:");
        //    sb.AppendLine($"Initial State: {InitialState}");
        //    sb.AppendLine("States:");
        //    foreach (var state in States)
        //    {
        //        sb.AppendLine($"  {state}");
        //    }
        //    sb.AppendLine("Alphabet:");
        //    foreach (var symbol in Alphabet)
        //    {
        //        sb.AppendLine($"  {symbol}");
        //    }
        //    sb.AppendLine("Transitions:");
        //    foreach (var transition in Transitions)
        //    {
        //        sb.Append($"  {transition.Key.Item1} --{transition.Key.Item2 ?? 'ε'}--> ");
        //        sb.AppendLine($"{{{string.Join(", ", transition.Value)}}}");
        //    }
        //    sb.AppendLine("Final States:");
        //    sb.AppendLine($"  {string.Join(", ", FinalStates)}");
        //    return sb.ToString();
        //}

    }
}
