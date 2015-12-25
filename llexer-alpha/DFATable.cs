using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llexer_alpha {
    public class DFATable {

        public DFATable() {
            map = new CharMap();
            finalStates = new HashSet<int>();
            table = new List<List<int>>();
        }

        /* Return everything needed to build a DFA. */
        public DFA toDFA() {
            var p = toDFAParameters();
            return new DFA(p.Item1, p.Item2, p.Item3);
        }

        public Tuple<int[, ], bool[], int[], int> toDFAParameters() {
            Func<int[]> buildMap = () => {
                int[] ret = new int[Utility.CHARSETSIZE];
                for (int i = 0; i < Utility.CHARSETSIZE; ++i) {
                    if (map[i] != CharMap.NONE) {
                        ret[i] = map[i];
                    } else {
                        if (i == '\n' || i == Utility.EOF) {
                            ret[i] = -1;
                        } else {
                            ret[i] = map[CharMap.WILD];
                        }
                    }
                }
                return ret;
            };

            // Build transition table and final flags.
            int[,] trans = new int[table.Count(), map.num];
            bool[] final = new bool[table.Count()];
            for (int i = 0; i < table.Count; ++i) {
                for (int j = 0; j < map.num; ++j) {
                    trans[i, j] = table[i][j];
                }
                final[i] = finalStates.Contains(i);
            }

            int[] m = buildMap();

            return new Tuple<int[,], bool[], int[], int>(trans, final, m, map[CharMap.WILD]);
        }

        /**
         * Simplify this DFA.
         */
        public DFATable simplify() {

            HashSet<int> startClosure = new HashSet<int> { 0 };
            {
                Stack<int> stack = new Stack<int>();
                stack.Push(0);

                while (stack.Count() > 0) {
                    int s = stack.Pop();
                    foreach (int t in table[s]) {
                        if (t != -1 && !startClosure.Contains(t)) {
                            stack.Push(t);
                            startClosure.Add(t);
                        }
                    }
                }
            }
            

            /**
             * Helper function to check if this state should be deleted.
             * A state should be deleted when
             * 1. Starting from 0, there is no way to reach it OR
             * 2. Starting from it, there is no way to reach final state.
             */
            Func<int, bool> isDeletable = s => {

                if (!startClosure.Contains(s)) {
                    return true;
                }

                Stack<int> stack = new Stack<int>();
                bool[] visited = new bool[table.Count()];
                for (int i = 0; i < table.Count(); ++i) {
                    visited[i] = false;
                }

                stack.Push(s);
                visited[s] = true;
                while (stack.Count() > 0) {
                    int state = stack.Pop();
                    if (finalStates.Contains(state)) {
                        // This state can reach the final state.
                        return false;
                    }
                    foreach (int t in table[state]) {
                        if (t != -1 && !visited[t]) {
                            stack.Push(t);
                            visited[t] = true;
                        }
                    }
                }
                // This state cannot reach the final state.
                return true;
            };

            DFATable dfa = new DFATable();
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
                        int input = map.getInput(i);
                        int t = table[s][i];
                        // Is t deleted?
                        if (t != -1 && !deleted[t]) {
                            dfa.addTransition(newId[s], newId[t], input);
                        }
                    }
                }
            }

            // Set the final states.
            foreach (int s in finalStates) {
                if (!deleted[s]) {
                    dfa.setStateFinal(newId[s]);
                }
            }

            return dfa;
        }

        /**
         * Print the transitions.
         */
        public override string ToString() {
            string ret = "DFA: \n";

            ret += "Final states: ";
            foreach (int final in finalStates) {
                ret += final;
                ret += ' ';
            }
            ret += '\n';

            for (int i = 0; i < table.Count(); ++i) {
                ret += i;
                ret += "\n";
                for (int j = 0; j < table[i].Count(); ++j) {
                    if (table[i][j] != -1) {
                        ret += "  ";
                        ret += map.idToString(j);
                        ret += ": ";
                        ret += table[i][j];
                        ret += '\n';
                    }
                }
            }

            return ret;
        }

        // Add a transition to the table.
        // Return 0 if succeed.
        public int addTransition(int from, int to, int input) {

            // First of all check from and to are valid states.
            if (from >= table.Count() || from < 0) {
                return -1;
            }

            if (to >= table.Count() || to < 0) {
                return -1;
            }

            // Check if this is a valid input.
            if (!map.isValidInput(input)) {
                return -1;
            }

            if (map[input] == CharMap.NONE) {
                map.newId(input);
                foreach (var s in table) {
                    s.Add(-1);
                }
            }

            // Add the transition into the table.
            table[from][map[input]] = to;
            return 0;
        }

        // Add a state to the NFA.
        // Initially there are no edges to this state.
        // Return the state id if success.
        public int addState() {

            List<int> row = new List<int>(map.num);
            for (int i = 0; i < map.num; ++i) {
                row.Add(-1);
            }

            table.Add(row);
            return table.Count() - 1;
        }

        // Set one state as the final state.
        // Return 0 if succeed.
        public int setStateFinal(int state) {

            if (state >= table.Count() || state < 0) {
                return -1;
            }
            finalStates.Add(state);
            return 0;
        }

        private List<List<int>> table;
        private HashSet<int> finalStates;
        private CharMap map;
    }
}
