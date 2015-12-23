using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llexer {
    public class Utility {

        public static readonly int CHARSETSIZE = 1 << 16;
        public static readonly int NFATABLESIZE = CHARSETSIZE + 2;
        public static readonly int DFATABLESIZE = CHARSETSIZE;
        public static readonly int WILD = CHARSETSIZE;
        public static readonly int EPSILON = CHARSETSIZE + 1;

        public static readonly int EOF = 0x1A;

        public static string inputToString(int input) {
            if (input == EPSILON) {
                return "EPSILON";
            } else if (input == WILD) {
                return "WILD";
            } else if (input == '\n') {
                return "\\n";
            } else {
                return new string((char)input, 1);
            }
        }
        
        [Conditional("DEBUG")]
        public static void DEBUG(string msg) {
            Console.WriteLine(msg);
        }
    }
}
