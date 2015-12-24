using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {
    public class DFATable {

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

        public void addTransition(int from, int to, IEnumerable<char> inputs) {
            foreach (char input in inputs) {
                table[from][map[input]] = to;
            }
        }

        /// <summary>
        /// Add a new state.
        /// There is no edge in this state.
        /// </summary>
        /// <returns> The state ID. </returns>
        public int addState() {
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
        public void setStateFinal(int state) {
            finals.Add(state);
        }

        public DFATable minimize() {
            return removeUnreachable().Hopcroft();
        }

        /// <summary>
        /// Remove unreachable state.
        /// </summary>
        /// <returns></returns>
        public DFATable removeUnreachable() {

            // All the state that can be reached from 0.

            // Find all reachable states.
            Func<int, HashSet<int>> findReachableClosure = x => {
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

            HashSet<int> startClosure = findReachableClosure(0);

            //
            // Helper function to check if this state should be deleted.
            // A state should be deleted when
            // 1. Starting from 0, there is no way to reach it OR
            // 2. Starting from it, there is no way to reach final state.
            //
            Func<int, bool> isDeletable = s => {
                if (!startClosure.Contains(s)) {
                    return true;
                }
                HashSet<int> reachable = findReachableClosure(s);
                return reachable.Intersect(finals).Count() == 0;
            };

            DFATable dfa = new DFATable(map, revMap);
            bool[] deleted = new bool[table.Count()];
            int[] newId = new int[table.Count()];

            // Find all the state to be deleted.
            for (int i = 0; i < table.Count(); ++i) {
                deleted[i] = isDeletable(i);
                if (!deleted[i]) {
                    // Create the new state in the simplified DFA.
                    newId[i] = dfa.addState();
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
                            dfa.addTransition(newId[s], newId[t], revMap[i]);
                        }
                    }
                }
            }

            // Set the final states.
            foreach (int s in finals) {
                if (!deleted[s]) {
                    dfa.setStateFinal(newId[s]);
                }
            }

            return dfa;
        }

        /// <summary>
        /// Use Hopcroft's Algorithm to simplify this DFATable.
        /// </summary>
        /// <returns> A new DFATable. </returns>
        public DFATable Hopcroft() {

            List<HashSet<int>> list = new List<HashSet<int>>();

            Stack<HashSet<int>> W = new Stack<HashSet<int>>();

            // Push finals.
            W.Push(new HashSet<int>(finals));

            // Push Non-finals.
            HashSet<int> non_finals = new HashSet<int>();
            for (int i = 0; i < table.Count(); ++i) {
                if (!finals.Contains(i))
                    non_finals.Add(i);
            }
            if (non_finals.Count() > 0) {
                W.Push(non_finals);
            }

            while (W.Count() > 0) {
                HashSet<int> set = W.Pop();
                int first = set.First();
                HashSet<int> X = new HashSet<int> { first };
                HashSet<int> Y = new HashSet<int>();
                foreach (int s in set.Skip(1)) {
                    bool isDifferent = false;
                    for (int i = 0; i < revMap.Count(); ++i) {
                        if (table[s][i] != table[first][i]) {
                            Y.Add(s);
                            isDifferent = true;
                            break;
                        }
                    }
                    if (!isDifferent) {
                        X.Add(s);
                    }
                }
                if (X.Count() > 0 && Y.Count() > 0) {
                    W.Push(Y);
                    W.Push(X);
                } else {
                    list.Add(set);
                }
            }

            for (int i = 0; i < list.Count(); ++i) {
                if (list[i].Contains(0)) {
                    HashSet<int> tmp = list[0];
                    list[0] = list[i];
                    list[i] = tmp;
                    break;
                }
            }

            // Build the new DFATable.
            int[] newId = new int[table.Count()];
            DFATable dfa = new DFATable(map, revMap);
            for (int i = 0; i < list.Count(); ++i) {
                int x = dfa.addState();
                foreach (int s in list[i]) {
                    newId[s] = x;
                }
                if (finals.Contains(list[i].First())) {
                    dfa.setStateFinal(x);
                }
            }

            for (int i = 0; i < list.Count(); ++i) {
                int s = list[i].First();
                for (int c = 0; c < revMap.Count(); ++c) {
                    if (table[s][c] != -1) {
                        dfa.addTransition(newId[s], newId[table[s][c]], revMap[c]);
                    }
                }
            }
            return dfa;
        }

            //// All the state that can be reached from 0.

            //// Find all reachable states.
            //Func<int, HashSet<int>> findReachableClosure = x => {
            //    Stack<int> stack = new Stack<int>();
            //    HashSet<int> closure = new HashSet<int> { x };
            //    stack.Push(x);
            //    while (stack.Count() > 0) {
            //        int s = stack.Pop();
            //        foreach (int t in table[s]) {
            //            if (t != -1 && !closure.Contains(t)) {
            //                stack.Push(t);
            //                closure.Add(t);
            //            }
            //        }
            //    }
            //    return closure;
            //};

            //HashSet<int> startClosure = findReachableClosure(0);

            ////
            //// Helper function to check if this state should be deleted.
            //// A state should be deleted when
            //// 1. Starting from 0, there is no way to reach it OR
            //// 2. Starting from it, there is no way to reach final state.
            ////
            //Func<int, bool> isDeletable = s => {
            //    if (!startClosure.Contains(s)) {
            //        return true;
            //    }
            //    HashSet<int> reachable = findReachableClosure(s);
            //    return reachable.Intersect(finals).Count() == 0;
            //};

            //DFATable dfa = new DFATable(map, revMap);
            //bool[] deleted = new bool[table.Count()];
            //int[] newId = new int[table.Count()];

            //// Find all the state to be deleted.
            //for (int i = 0; i < table.Count(); ++i) {
            //    deleted[i] = isDeletable(i);
            //    if (!deleted[i]) {
            //        // Create the new state in the simplified DFA.
            //        newId[i] = dfa.addState();
            //    }
            //}

            //// Copy the transitions.
            //for (int s = 0; s < table.Count(); ++s) {
            //    // Is this state deleted?
            //    if (!deleted[s]) {
            //        for (int i = 0; i < table[s].Count(); ++i) {
            //            int t = table[s][i];
            //            // Is t deleted?
            //            if (t != -1 && !deleted[t]) {
            //                dfa.addTransition(newId[s], newId[t], revMap[i]);
            //            }
            //        }
            //    }
            //}

            //// Set the final states.
            //foreach (int s in finals) {
            //    if (!deleted[s]) {
            //        dfa.setStateFinal(newId[s]);
            //    }
            //}

            //return dfa;
        //}

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
                        foreach (char c in revMap[j]) {
                            ret += Utility.print(c);
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
