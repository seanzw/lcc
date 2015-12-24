using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DFAState = System.Collections.Generic.HashSet<int>;

namespace RegEx {
    public class NFATable {

        /// <summary>
        /// Construct a new NFA table.
        /// By default, 0 is the start state.
        /// </summary>
        /// <param name="map"> Map from char to class. </param>
        /// <param name="revMap"> Reverse map from class to chars. </param>
        public NFATable(int[] map, List<char[]> revMap) {
            this.map = map;
            this.revMap = revMap;

            // Initialize the table.
            // The last class is for epsilon.
            table = new List<HashSet<int>[]> {
                new HashSet<int>[revMap.Count() + 1]
            };

            for (int i = 0; i < table[0].Count(); ++i) {
                table[0][i] = new HashSet<int>();
            }

            finals = new HashSet<int>();
        }

        public void addTransition(int from, int to, HashSet<char> inputs) {
            foreach (char input in inputs) {
                table[from][map[input]].Add(to);
            }
        }

        public void addEpsilonTransition(int from, int to) {
            table[from][revMap.Count()].Add(to);
        }

        /// <summary>
        /// Add a new state.
        /// There is no edge in this state.
        /// </summary>
        /// <returns> The state ID. </returns>
        public int addState() {
            table.Add(new HashSet<int>[revMap.Count() + 1]);
            int newId = table.Count() - 1;
            for (int i = 0; i < table[newId].Count(); ++i) {
                table[newId][i] = new HashSet<int>();
            }
            return newId;
        }

        /// <summary>
        /// Set one state final.
        /// </summary>
        /// <param name="state"> State ID. </param>
        public void setStateFinal(int state) {
            finals.Add(state);
        }

        public override string ToString() {
            string ret = "NFATable: \n";

            ret += "Final states: ";
            foreach (int final in finals) {
                ret += final;
                ret += ' ';
            }
            ret += '\n';

            for (int i = 0; i < table.Count(); ++i) {
                ret += i;
                ret += "\n";
                for (int j = 0; j < revMap.Count() + 1; ++j) {
                    if (table[i][j].Count() > 0) {
                        ret += "  ";
                        if (j < revMap.Count()) {
                            foreach (char c in revMap[j]) {
                                ret += Utility.print(c);
                            }
                        } else {
                            // Epsilon edge.
                            ret += "EPSILON";
                        }
                        ret += ": ";
                        foreach (int k in table[i][j]) {
                            ret += k;
                            ret += ' ';
                        }
                        ret += '\n';
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Convert NFATable to DFATable.
        /// </summary>
        /// <returns> A DFATable. </returns>
        public DFATable toDFATable() {

            // Helper function to check if this DFAState is final.
            Func<DFAState, bool> isFinal = x => {
                foreach (int s in x) {
                    if (finals.Contains(s)) {
                        return true;
                    }
                }
                return false;
            };

            DFATable dfa = new DFATable(map, revMap);

            // Map DFAState to the actual state ID in DFATable.
            Dictionary<DFAState, int> dict = new Dictionary<DFAState, int>(DFAState.CreateSetComparer());
            Stack<DFAState> unmarked = new Stack<DFAState>();

            // Initialize the first DFA state.
            DFAState closure = findEpsilonClosure(new HashSet<int> { 0 });
            dict.Add(closure, dfa.addState());
            unmarked.Push(closure);
            if (isFinal(closure)) {
                dfa.setStateFinal(dict[closure]);
            }

            // Build the DFA by simulating NFA.
            while (unmarked.Count() > 0) {
                DFAState T = unmarked.Pop();

                for (int i = 0; i < revMap.Count(); ++i) {

                    // Find move(T, i).
                    DFAState move = new DFAState();
                    foreach (int s in T) {
                        move.UnionWith(table[s][i]);
                    }

                    // U = epsilon-closure(move).
                    DFAState U = findEpsilonClosure(move);
                    if (!dict.ContainsKey(U)) {
                        // This is a new DFAState.
                        dict.Add(U, dfa.addState());
                        unmarked.Push(U);
                        if (isFinal(U)) {
                            dfa.setStateFinal(dict[U]);
                        }
                    }

                    // Add transition from T to U with i.
                    dfa.addTransition(dict[T], dict[U], revMap[i]);
                }
            }
            return dfa.minimize();
        }

        /// <summary>
        /// Star operation.
        /// </summary>
        /// <returns> A new NFATable. </returns>
        public NFATable star() {

            NFATable nfa = new NFATable(map, revMap);
            nfa.addState();
            nfa.addEpsilonTransition(0, 1);

            // Merge.
            NFATable ret = nfa.merge(this);

            ret.addEpsilonTransition(1, 2);
            ret.setStateFinal(1);

            int offset = 2;
            foreach (int final in finals) {
                ret.addEpsilonTransition(final + offset, 1);
            }

            return ret;
        }

        /// <summary>
        /// Match one or more.
        /// </summary>
        /// <returns> A new NFATable. </returns>
        public NFATable ques() {

            NFATable nfa = new NFATable(map, revMap);
            nfa.addState();
            nfa.addEpsilonTransition(0, 1);

            NFATable ret = nfa.merge(this);
            ret.addEpsilonTransition(0, 2);

            ret.setStateFinal(1);

            int offset = 2;
            foreach (int final in finals) {
                ret.addEpsilonTransition(final + offset, 1);
            }

            return ret;
        }

        /// <summary>
        /// Cat two NFATables together.
        /// </summary>
        /// <param name="nfa1"> The first NFATable. </param>
        /// <param name="nfa2"> The second NFATable. </param>
        /// <returns> A new NFATable. </returns>
        public static NFATable operator + (NFATable nfa1, NFATable nfa2) {

            NFATable ret = nfa1.merge(nfa2);
            int offset = nfa1.table.Count();

            // Connect this.final to other.start.
            foreach (int final in nfa1.finals) {
                ret.addEpsilonTransition(final, offset);
            }

            // Set the final states.
            foreach (int final in nfa2.finals) {
                ret.setStateFinal(final + offset);
            }

            return ret;
        }

        /// <summary>
        /// Or operation.
        /// </summary>
        /// <param name="nfa1"> The first NFATable. </param>
        /// <param name="nfa2"> The second NFATable. </param>
        /// <returns> A new NFATable. </returns>
        public static NFATable operator | (NFATable nfa1, NFATable nfa2) {

            NFATable ret = nfa1.merge(nfa2);
            int offset = nfa1.table.Count();
            ret.addEpsilonTransition(0, offset);

            foreach (int final in nfa1.finals) {
                ret.setStateFinal(final);
            }

            int end = nfa1.finals.First();
            foreach (int final in nfa2.finals) {
                ret.addEpsilonTransition(final + offset, end);
            }

            return ret;
        }

        /// <summary>
        /// Merge this nfa with another one
        /// by adding their states at the end of the table.
        /// Notice that the new NFATable has no final state.
        /// </summary>
        /// <param name="other"> NFATable to be merged. </param>
        /// <returns> The new merged NFATable. </returns>
        private NFATable merge(NFATable other) {

            NFATable ret = copy();

            // Add the second nfa.
            int offset = ret.table.Count();
            for (int i = 0; i < other.table.Count(); ++i) {
                ret.addState();
            }
            for (int from = 0; from < other.table.Count(); ++from) {
                for (int i = 0; i < other.table[from].Count(); ++i) {
                    foreach (int to in other.table[from][i]) {
                        ret.table[from + offset][i].Add(to + offset);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Copy this NFATable.
        /// Notice that the new NFATable has no final state.
        /// </summary>
        /// <returns> A new NFATable. </returns>
        private NFATable copy() {

            NFATable ret = new NFATable(map, revMap);

            // Add the first nfa.
            // Starts from 1 to avoid two 0 state.
            for (int i = 1; i < table.Count(); ++i) {
                ret.addState();
            }
            for (int from = 0; from < table.Count(); ++from) {
                for (int i = 0; i < table[from].Count(); ++i) {
                    ret.table[from][i] = new HashSet<int>(table[from][i]);
                }
            }

            return ret;
        }

        /// <summary>
        /// Use DFS to find the epsilon closure.
        /// </summary>
        /// <param name="states"> Initial states. </param>
        /// <returns> Epsilon closure. </returns>
        private HashSet<int> findEpsilonClosure(HashSet<int> states) {

            HashSet<int> closure = new HashSet<int>(states);

            int epsilon = revMap.Count();
            Stack<int> stack = new Stack<int>();
            foreach (int state in states) {
                stack.Push(state);
            }
            while (stack.Count() > 0) {
                int s = stack.Pop();
                foreach (int t in table[s][epsilon]) {
                    if (!closure.Contains(t)) {
                        stack.Push(t);
                        closure.Add(t);
                    }
                }
            }
            return closure;
        }

        /// <summary>
        /// Final states.
        /// </summary>
        private HashSet<int> finals;
        private List<HashSet<int>[]> table;
        private readonly List<char[]> revMap;
        private readonly int[] map;
    }
}
