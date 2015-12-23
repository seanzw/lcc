using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llexer {

    /**
     * NFA transition table.
     *
     *          Input
     *
     * S
     * t
     * a    Next states.
     * t
     * e
     * s
     *
     */

    public class NFATable {

        public NFATable() {

            map = new CharMap();

            // Initialize the map.
            table = new List<List<HashSet<int>>> {
                new List<HashSet<int>>()
            };

            // Initialize the final states to empty.
            finalStates = new HashSet<int>();

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

            if (!map.isValidInput(input)) {
                return -1;
            }

            if (map.getId(input) == CharMap.NONE) {
                map.newId(input);
                foreach (var s in table) {
                    s.Add(new HashSet<int>());
                }
            }

            table[from][map.getId(input)].Add(to);
            return 0;
        }

        // Add a state to the NFA.
        // Initially there are no edges to this state.
        // Return the state id if success.
        public int addState() {

            List<HashSet<int>> row = new List<HashSet<int>>(map.num);
            for (int i = 0; i < map.num; ++i) {
                row.Add(new HashSet<int>());
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

        /**
         * Print the transitions.
         */
        public override string ToString() {
            string ret = "NFA: \n";

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
                    if (table[i][j].Count() > 0) {
                        ret += "  ";
                        ret += map.idToString(j);
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

        /**
         * Star operation.
         */
        public NFATable star() {

            // Create a new NFA.
            NFATable nfa = new NFATable();
            nfa.addState();
            nfa.addTransition(0, 1, CharMap.EPSILON);

            // Merge nfa with this one.
            NFATable ret = nfa.merge(this);

            // Connect the end of nfa to S1.
            ret.addTransition(1, 2, CharMap.EPSILON);

            // Set the final states of ret.
            ret.setStateFinal(1);

            // Connect the E1 to the end of nfa.
            int offset = 2;
            foreach (int final in finalStates) {
                ret.addTransition(final + offset, 1, CharMap.EPSILON);
            }

            return ret;
        }

        /// <summary>
        /// ? operation.
        ///    reg
        ///   /e  \e
        ///  S ---> E
        /// </summary>
        /// <returns> A new NFATable with "reg?".</returns>
        public NFATable question() {
            // Create a new NFA.
            NFATable nfa = new NFATable();
            nfa.addState();
            nfa.addTransition(0, 1, CharMap.EPSILON);

            // Merge nfa with this one.
            NFATable ret = nfa.merge(this);

            // Connect the end of nfa to S1.
            ret.addTransition(0, 2, CharMap.EPSILON);

            // Set the final states of ret.
            ret.setStateFinal(1);

            // Connect the E1 to the end of nfa.
            int offset = 2;
            foreach (int final in finalStates) {
                ret.addTransition(final + offset, 1, CharMap.EPSILON);
            }

            return ret;
        }
       

        /**
         * Merge two NFAs as AND operation.
         *
         * S1 ----> E1 ----* S2 ----> E2
         */
        public NFATable mergeAnd(NFATable other) {

            NFATable ret = merge(other);

            // Connect the end of the first NFA
            // to the start of the second one with epsilon transition.
            int offset = table.Count();
            foreach (int final in finalStates) {
                ret.addTransition(final, offset, CharMap.EPSILON);
            }

            // Set the final states of the new NFA to the final states of other.
            foreach (int final in other.finalStates) {
                ret.setStateFinal(final + offset);
            }

            return ret;
        }

        /**
         * Merge two NFAs as OR operation.
         *
         * S2 ----> E2
         * *        |
         * |        *
         * S1 ----> E1
         */
        public NFATable mergeOr(NFATable other) {

            NFATable ret = merge(other);

            // Connect the start states together.
            int offset = table.Count();
            ret.addTransition(0, offset, CharMap.EPSILON);

            // Set the final states of the new NFA.
            foreach (int final in finalStates) {
                ret.setStateFinal(final);
            }

            // Connect the end together.
            int E1 = finalStates.ElementAt(0);
            foreach (int final in other.finalStates) {
                ret.addTransition(final + offset, E1, CharMap.EPSILON);
            }

            return ret;
        }

        /**
         * Convert the epsilon-NFA in DFA.
         */
        public DFATable toDFATable() {

            /* Helper function. */
            Func<HashSet<int>, bool> isFinal = x => {
                foreach (int s in x) {
                    if (finalStates.Contains(s)) {
                        return true;
                    }
                }
                return false;
            };

            DFATable dfa = new DFATable();

            Dictionary<HashSet<int>, int> toDFA = new Dictionary<HashSet<int>, int>(HashSet<int>.CreateSetComparer());
            Stack<HashSet<int>> unmarked = new Stack<HashSet<int>>();

            // Initialize the first DFA state.
            HashSet<int> closure = findEpsilonClosure(new HashSet<int> { 0 });
            toDFA.Add(closure, dfa.addState());
            unmarked.Push(closure);
            if (isFinal(closure)) {
                dfa.setStateFinal(toDFA[closure]);
            }

            
            // Build the DFA.
            int epsilon = map[CharMap.EPSILON];
            while (unmarked.Count() > 0) {
                HashSet<int> T = unmarked.Pop();

                for (int i = 0; i < map.num; ++i) {
                    if (i == epsilon) {
                        continue;
                    }

                    // Find move(T, input).
                    int input = map.getInput(i);
                    HashSet<int> move = new HashSet<int>();
                    foreach (int s in T) {
                        foreach (int t in table[s][i]) {
                            move.Add(t);
                        }
                    }

                    // U = epsilon-closure(move).
                    HashSet<int> U = findEpsilonClosure(move);
                    if (!toDFA.ContainsKey(U)) {
                        toDFA.Add(U, dfa.addState());
                        unmarked.Push(U);
                        if (isFinal(U)) {
                            dfa.setStateFinal(toDFA[U]);
                        }
                    }

                    dfa.addTransition(toDFA[T], toDFA[U], input);
                }
                
            }

            return dfa;
        }

        public NFATable copy() {

            NFATable ret = new NFATable();

            // Add the first nfa.
            // Starts from 1 to avoid two 0 state.
            for (int i = 1; i < table.Count(); ++i) {
                ret.addState();
            }
            for (int from = 0; from < table.Count(); ++from) {
                for (int j = 0; j < table[from].Count(); ++j) {
                    int input = map.getInput(j);
                    foreach (int to in table[from][j]) {
                        ret.addTransition(from, to, input);
                    }
                }
            }

            return ret;
        }

        /**
         * Find the epsilon closure of state.
         */
        private HashSet<int> findEpsilonClosure(HashSet<int> states) {

            // Use DFS to find the epsilon closure.
            HashSet<int> closure = new HashSet<int>(states);

            int epsilon = map[CharMap.EPSILON];
            if (epsilon != -1) {
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
            }

            return closure;
        }

        // Helper function to merge two nfas into one.
        // Notice that the final states of the new nfa is undefined.
        private NFATable merge(NFATable other) {

            NFATable ret = copy();

            // Add the second nfa.
            int offset = ret.table.Count();
            for (int i = 0; i < other.table.Count(); ++i) {
                ret.addState();
            }
            for (int from = 0; from < other.table.Count(); ++from) {
                for (int j = 0; j < other.table[from].Count(); ++j) {
                    int input = other.map.getInput(j);
                    foreach (int to in other.table[from][j]) {
                        ret.addTransition(from + offset, to + offset, input);
                    }
                }
            }

            return ret;
        }

        private List<List<HashSet<int>>> table;
        private HashSet<int> finalStates;
        private CharMap map;

    }
}
