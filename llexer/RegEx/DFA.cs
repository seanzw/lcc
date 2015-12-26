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

        public DFA(int[,] table, bool[] final, int[] range, int[] value) {
            this.table = table;
            this.final = final;
            map = expandMap(range, value);
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

        public Tuple<int[], int[]> shrinkMap() {

            LinkedList<int> range = new LinkedList<int>();
            LinkedList<int> value = new LinkedList<int>();

            value.AddLast(map[0]);
            for (int i = 1; i < Const.CHARSIZE; ++i) {
                if (map[i] != map[i - 1]) {
                    range.AddLast(i - 1);
                    value.AddLast(map[i]);
                }
            }
            range.AddLast(Const.CHARSIZE - 1);

            return new Tuple<int[], int[]>(range.ToArray(), value.ToArray());
        }

        private int[] expandMap(int[] range, int[] value) {

            int[] ret = new int[Const.CHARSIZE];
            for (int i = 0; i <= range[0]; ++i) {
                ret[i] = value[0];
            }
            for (int i = 1; i < range.Count(); ++i) {
                for (int j = range[i - 1] + 1; j <= range[i]; ++j) {
                    ret[j] = value[i];
                }
            }

            return ret;
        }


        public readonly int[,] table;
        public readonly bool[] final;
        public readonly int[] map;

        /// <summary>
        /// Current state.
        /// </summary>
        private int state;

        private const int FAILURE_STATE = -1;
        private const int SUCCESS_STATE = -2;
    }
}
