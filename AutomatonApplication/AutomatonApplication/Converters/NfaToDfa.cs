using AutomatonApplication.Models;

namespace AutomationApplication.Converters
{
    public class NfaToDfa
    {
        private readonly NFA nfa;

        public NfaToDfa(NFA nfa)
        {
            this.nfa = nfa ?? throw new ArgumentNullException(nameof(nfa));
        }

        public DFA Convert()
        {
            var dfaStates = new Dictionary<HashSet<string>, string>(new HashSetComparer());
            var dfaTransitions = new Dictionary<(string, char), string>();
            var dfaFinalStates = new HashSet<string>();
            var dfaAlphabet = nfa.Alphabet;
            var initialStateClosure = EpsilonClosure(new HashSet<string> { nfa.InitialState });  // epsilon inchiderea starii initiale
            var dfaInitialState = CreateDfaStateName(dfaStates, initialStateClosure);
            if (nfa.FinalStates.Overlaps(initialStateClosure))
            {
                dfaFinalStates.Add(dfaInitialState);
            }
            var unprocessedStates = new Queue<HashSet<string>>();   // coada pentru procesarea starilor DFA
            unprocessedStates.Enqueue(initialStateClosure);
            while (unprocessedStates.Count > 0) // procesam toate starile DFA
            {
                var currentState = unprocessedStates.Dequeue();
                var currentStateName = dfaStates[currentState];
                foreach (var symbol in dfaAlphabet)
                {
                    var reachableStates = new HashSet<string>();// calculam toate starile accesibile prin simbol
                    foreach (var state in currentState)
                    {
                        if (nfa.Transitions.TryGetValue((state, symbol), out var targets))
                        {
                            reachableStates.UnionWith(targets);
                        }
                    }
                    var closure = EpsilonClosure(reachableStates);// aplicam epsilon inchiderea
                    if (closure.Count > 0)
                    {
                        if (!dfaStates.ContainsKey(closure))// verif daca aceasta stare exista deja în DFA
                        {
                            var newStateName = CreateDfaStateName(dfaStates, closure); // cream o stare noua în DFA
                            unprocessedStates.Enqueue(closure);
                            if (closure.Overlaps(nfa.FinalStates))// ver daca aceasta trebuie sa fie finala
                            {
                                dfaFinalStates.Add(newStateName);
                            }
                        }
                        dfaTransitions[(currentStateName, symbol)] = dfaStates[closure]; //adaug tranzitia în DFA
                    }
                }
            }

            return new DFA(
                new HashSet<string>(dfaStates.Values),
                dfaAlphabet,
                dfaTransitions,
                dfaInitialState,
                dfaFinalStates);
        }

        private HashSet<string> EpsilonClosure(HashSet<string> states)
        {
            var closure = new HashSet<string>(states);
            var stack = new Stack<string>(states);

            while (stack.Count > 0)
            {
                var state = stack.Pop();
                if (nfa.Transitions.TryGetValue((state, null), out var epsilonTargets))
                {
                    foreach (var target in epsilonTargets)
                    {
                        if (closure.Add(target))
                        {
                            stack.Push(target);
                        }
                    }
                }
            }
            return closure;
        }

        private string CreateDfaStateName(Dictionary<HashSet<string>, string> dfaStates, HashSet<string> nfaStates)
        {
            var stateName = $"q{dfaStates.Count}";
            dfaStates[nfaStates] = stateName;
            return stateName;
        }

        private class HashSetComparer : IEqualityComparer<HashSet<string>>
        {
            public bool Equals(HashSet<string>? x, HashSet<string>? y)
            {
                if (x == null || y == null)
                {
                    return x == null && y == null;
                }

                return x.SetEquals(y);
            }
            public int GetHashCode(HashSet<string> obj)
            {
                return obj.Aggregate(0, (hash, item) => hash ^ item.GetHashCode());
            }
        }
    }
}
