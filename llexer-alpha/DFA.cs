using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llexer {
    public class DFA {

        public enum Status {
            RUNNING,
            FAILED,
            SUCCEED
        };

        public DFA(int[,] table, bool[] final, int[] map) {
            this.table = table;
            this.final = final;
            this.map = map;
            reset();
        }

        /* Reset the DFA to start state. */
        public void reset() {
            state = 0;
        }

        /* Get the current status of this DFA. */
        public Status status() {
            switch (state) {
                case SUCCESS_STATE:
                    return Status.SUCCEED;
                case FAILURE_STATE:
                    return Status.FAILED;
                default:
                    return Status.RUNNING;
            }
        }

        /* Feed an input to this DFA. */
        public void scan(int input) {
            
            switch (state) {
                case SUCCESS_STATE:
                    state = FAILURE_STATE;
                    break;
                case FAILURE_STATE:
                    break;
                default:
                    if (map[input] == -1 || table[state, map[input]] == -1) {
                        state = final[state] ? SUCCESS_STATE : FAILURE_STATE;
                    } else {
                        state = table[state, map[input]];
                    }
                    break;
            }
        }

        private readonly int[,] table;
        private readonly bool[] final;
        private readonly int[] map;

        // Current state.
        private int state;

        private const int FAILURE_STATE = -1;
        private const int SUCCESS_STATE = -2;

    }
}
