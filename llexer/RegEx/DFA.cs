using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {

    public class DFA {

        public enum Status {
            RUN,
            FAILED,
            SUCCEED
        }

        public DFA(int[,] table, bool[] final, int[] map) {
            this.table = table;
            this.final = final;
            this.map = map;
            reset();
        }

        /// <summary>
        /// Reset this DFA to the start state.
        /// </summary>
        public void reset() {
            state = 0;
        }

        /// <summary>
        /// Check the status.
        /// </summary>
        /// <returns> Returns the status. </returns>
        public Status status() {
            switch (state) {
                case SUCCESS_STATE:
                    return Status.SUCCEED;
                case FAILURE_STATE:
                    return Status.FAILED;
                default:
                    return Status.RUN;
            }
        }

        public void scan(char c) {
            switch (state) {
                case SUCCESS_STATE:
                    state = FAILURE_STATE;
                    break;
                case FAILURE_STATE:
                    break;
                default:
                    if (map[c] == -1 || table[state, map[c]] == -1) {
                        state = final[state] ? SUCCESS_STATE : FAILURE_STATE;
                    } else {
                        state = table[state, map[c]];
                    }
                    break;
            }
        }

        private readonly int[,] table;
        private readonly bool[] final;
        private readonly int[] map;

        /// <summary>
        /// Current state.
        /// </summary>
        private int state;

        private const int FAILURE_STATE = -1;
        private const int SUCCESS_STATE = -2;
    }
}
