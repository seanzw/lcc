using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llexer_alpha {
    /*****************************************************************

        This class maps char to its group id in the transition table.
        It contanis all 2 ^ 16 unicode, plus EPSILON and WILD.

        EPSILON: used in NFATable.
        WILD: wild match except '\n' and EOF

    *****************************************************************/
    class CharMap {

        public CharMap() {
            num = 0;
            map = new int[SIZE];
            for (int i = 0; i < SIZE; ++i) {
                map[i] = NONE;
            }
        }

        public CharMap(CharMap other) {
            num = other.num;
            map = new int[SIZE];
            for (int i = 0; i < SIZE; ++i) {
                map[i] = other.map[i];
            }
        }

        public bool isValidInput(int input) {
            return input >= 0 && input < SIZE;
        }

        /* Indexer. */
        public int this[int input] {
            get {
                return map[input];
            }
        }

        /* Get the group id of the input. */
        public int getId(int input) {
            return map[input];
        }

        /*****************************************************************
            Allocate a new id for this char. 
            This should only be called when getId(input) = -1.
            Returns the new id.  
        ******************************************************************/
        public int newId(int input) {
            map[input] = num;
            num = num + 1;
            return num - 1;
        }

        /* Get the input for this id. */
        public int getInput(int id) {
            return Array.FindIndex(map, x => x == id);
        }

        public string idToString(int id) {
            return Utility.inputToString(getInput(id));
        }

        private int[] map;

        public int num {
            get;
            private set;
        }

        public static readonly int SIZE = (1 << 16) + 2;
        public static readonly int WILD = 1 << 16;
        public static readonly int EPSILON = WILD + 1;
        public static readonly int NONE = -1;
        public static readonly int EOF = 0x1A;
    }
}
