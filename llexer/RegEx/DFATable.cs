using System;
using System.Collections.Generic;
using System.Linq;
using NDState = System.Collections.Generic.HashSet<int>;    // Nondistinguishable state.
using System.Text;
using System.Threading.Tasks;

namespace RegEx {
    class DFATable {

        /// <summary>
        /// Constructor of DFATable.
        /// By default there is no state in the table.
        /// </summary>
        /// <param name="map"> Map from char to class. </param>
        /// <param name="revMap"> Reverse map from class to chars. </param>
        public DFATable(int[] map, List<char[]> revMap) {
            this.map = map;
            this.revMap = revMap;

            // Initialize the table.
            table = new List<int[]>();

            finals = new HashSet<int>();
        }

        public void AddTransition(int from, int to, IEnumerable<char> inputs) {
            foreach (char input in inputs) {
                table[from][map[input]] = to;
            }
        }

        /// <summary>
        /// Add a new state.
        /// There is no edge in this state.
        /// </summary>
        /// <returns> The state ID. </returns>
        public int AddState() {
            table.Add(new int[revMap.Count()]);
            int newId = table.Count() - 1;
            for (int i = 0; i < table[newId].Count(); ++i) {
                table[newId][i] = -1;
            }
            return newId;
        }

        /// <summary>
        /// Set one state final.
        /// </summary>
        /// <param name="state"> State ID. </param>
        public void SetStateFinal(int state) {
            finals.Add(state);
        }

        /// <summary>
        /// Minimize this DFATable.
        /// </summary>
        /// <returns></returns>
        public DFATable Minimize() {
            return RemoveUnreachable().Hopcroft();
        }

        /// <summary>
        /// Generate a DFA.
        /// </summary>
        /// <returns> The DFA. </returns>
        public DFA ToDFA() {

            // Generate the table.
            int[,] table = new int[this.table.Count(), revMap.Count()];
            for (int i = 0; i < this.table.Count(); ++i) {
                for (int j = 0; j < revMap.Count(); ++j) {
                    table[i, j] = this.table[i][j];
                }
            }

            // Generate final flags.
            bool[] final = new bool[this.table.Count()];
            for (int i = 0; i < final.Count(); ++i) {
                final[i] = false;
            }
            foreach (int i in this.finals) {
                final[i] = true;
            }

            // Generate the DFA.
            return new DFA(table, final, map);
        }

        /// <summary>
        /// Remove unreachable state.
        /// </summary>
        /// <returns></returns>
        private DFATable RemoveUnreachable() {

            // All the state that can be reached from 0.

            // Find all reachable states.
            Func<int, HashSet<int>> FindReachableClosure = x => {
                Stack<int> stack = new Stack<int>();
                HashSet<int> closure = new HashSet<int> { x };
                stack.Push(x);
                while (stack.Count() > 0) {
                    int s = stack.Pop();
                    foreach (int t in table[s]) {
                        if (t != -1 && !closure.Contains(t)) {
                            stack.Push(t);
                            closure.Add(t);
                        }
                    }
                }
                return closure;
            };

            HashSet<int> startClosure = FindReachableClosure(0);

            //
            // Helper function to check if this state should be deleted.
            // A state should be deleted when
            // 1. Starting from 0, there is no way to reach it OR
            // 2. Starting from it, there is no way to reach final state.
            //
            Func<int, bool> IsDeletable = s => {
                if (!startClosure.Contains(s)) {
                    return true;
                }
                HashSet<int> reachable = FindReachableClosure(s);
                return reachable.Intersect(finals).Count() == 0;
            };

            DFATable dfa = new DFATable(map, revMap);
            bool[] deleted = new bool[table.Count()];
            int[] newId = new int[table.Count()];

            // Find all the state to be deleted.
            for (int i = 0; i < table.Count(); ++i) {
                deleted[i] = IsDeletable(i);
                if (!deleted[i]) {
                    // Create the new state in the simplified DFA.
                    newId[i] = dfa.AddState();
                }
            }

            // Copy the transitions.
            for (int s = 0; s < table.Count(); ++s) {
                // Is this state deleted?
                if (!deleted[s]) {
                    for (int i = 0; i < table[s].Count(); ++i) {
                        int t = table[s][i];
                        // Is t deleted?
                        if (t != -1 && !deleted[t]) {
                            dfa.AddTransition(newId[s], newId[t], revMap[i]);
                        }
                    }
                }
            }

            // Set the final states.
            foreach (int s in finals) {
                if (!deleted[s]) {
                    dfa.SetStateFinal(newId[s]);
                }
            }

            return dfa;
        }

        /// <summary>
        /// Use Hopcroft's Algorithm to simplify this DFATable.
        /// 
        /// https://en.wikipedia.org/wiki/DFA_minimization
        /// </summary>
        /// <returns> A new DFATable. </returns>
        private DFATable Hopcroft() {

            // The list of new states.
            List<NDState> P = new List<NDState>();

            // The stack to be processed.
            Stack<NDState> W = new Stack<NDState>();

            // Push finals and non-finals.
            // P := { F, NF };
            // W := { F };
            {
                NDState F = new NDState(finals);
                W.Push(F);
                P.Add(F);

                NDState NF = new NDState();
                for (int i = 0; i < table.Count(); ++i) {
                    if (!F.Contains(i))
                        NF.Add(i);
                }
                if (NF.Count() > 0) {
                    P.Add(NF);
                }
            }

            while (W.Count() > 0) {
                NDState A = W.Pop();
                for (int c = 0; c < revMap.Count(); ++c) {

                    // X is the set of states for which a transition on c
                    // leads to a state in A.
                    NDState X = new NDState();
                    for (int i = 0; i < table.Count(); ++i) {
                        if (A.Contains(table[i][c])) {
                            X.Add(i);
                        }
                    }

                    int PSize = P.Count();
                    for (int i = 0; i < PSize; ++i) {
                        NDState Y = P[i];
                        NDState I = new NDState(X.Intersect(Y));
                        if (I.Count() > 0 && I.Count() < Y.Count()) {

                            // Replace Y with intersec(X, Y) and Y \ X.
                            Y.ExceptWith(I);
                            P.Add(I);

                            // If Y is in W, replace Y with intersec(X, Y) and Y \ X.
                            // Notice that since object is reference, Y in W has already changed.
                            if (W.Contains(Y)) {
                                W.Push(I);
                            } else {
                                W.Push(I);
                                W.Push(Y);
                            }
                        }
                    }
                }
            }

            // Make sure that 0 is the start state.
            for (int i = 0; i < P.Count(); ++i) {
                if (P[i].Contains(0)) {
                    NDState tmp = P[0];
                    P[0] = P[i];
                    P[i] = tmp;
                    break;
                }
            }

            // Build the new DFATable.
            int[] newId = new int[table.Count()];
            DFATable dfa = new DFATable(map, revMap);
            for (int i = 0; i < P.Count(); ++i) {
                int x = dfa.AddState();
                foreach (int s in P[i]) {
                    newId[s] = x;
                }
                if (finals.Contains(P[i].First())) {
                    dfa.SetStateFinal(x);
                }
            }

            // Copy the transition.
            for (int i = 0; i < P.Count(); ++i) {
                int s = P[i].First();
                for (int c = 0; c < revMap.Count(); ++c) {
                    if (table[s][c] != -1) {
                        dfa.AddTransition(newId[s], newId[table[s][c]], revMap[c]);
                    }
                }
            }
            return dfa;
        }

        public override string ToString() {
            string ret = "DFATable: \n";

            ret += "Final states: ";
            foreach (int final in finals) {
                ret += final;
                ret += ' ';
            }
            ret += '\n';

            for (int i = 0; i < table.Count(); ++i) {
                ret += i;
                ret += "\n";
                for (int j = 0; j < revMap.Count(); ++j) {
                    if (table[i][j] != -1) {
                        ret += "  ";
                        if (revMap[j].Count() > 100) {
                            ret += "WILD";
                        } else {
                            foreach (char c in revMap[j]) {
                                ret += Utility.Print(c);
                            }
                        }
                        ret += ": ";
                        ret += table[i][j];
                        ret += '\n';
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Final states.
        /// </summary>
        private HashSet<int> finals;

        private List<int[]> table;
        private readonly List<char[]> revMap;
        private readonly int[] map;
    }
}
