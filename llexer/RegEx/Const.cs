using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {
    class Const {

        public const char NONE = (char)0;
        public const char FIRSTCHAR = (char)1;
        public const int CHARSIZE = 1 << 16;
    }

    class Utility {
        public static string print(char c) {
            switch (c) {
                default:
                    return new string(c, 1);
            }
        }
    }
}
