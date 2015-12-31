using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;
using LLexer;

namespace lcc.Test {
    static class TestLexer {

        public static void TestKeyword() {
            string src = @"
auto auto break char const 
double enum do inline int long goto 
restrict short volatile register signed unsigned
union struct void typedef switch _Complex _Imaginary
            ";
            List<Token.Token> truth = new List<Token.Token> {
                new T_KEY_AUTO(2),
                new T_KEY_AUTO(2),
                new T_KEY_BREAK(2),
                new T_KEY_CHAR(2),
                new T_KEY_CONST(2),
                new T_KEY_DOUBLE(3),
                new T_KEY_ENUM(3),
                new T_KEY_DO(3),
                new T_KEY_INLINE(3),
                new T_KEY_INT(3),
                new T_KEY_LONG(3),
                new T_KEY_GOTO(3),
                new T_KEY_RESTRICT(4),
                new T_KEY_SHORT(4),
                new T_KEY_VOLATILE(4),
                new T_KEY_REGISTER(4),
                new T_KEY_SIGNED(4),
                new T_KEY_UNSIGNED(4),
                new T_KEY_UNION(5),
                new T_KEY_STRUCT(5),
                new T_KEY_VOID(5),
                new T_KEY_TYPEDEF(5),
                new T_KEY_SWITCH(5),
                new T_KEY__COMPLEX(5),
                new T_KEY__IMAGINARY(5)
            };
            var tokens = Lexer.Instance.Scan(src);
            if (tokens.Count != truth.Count) {
                Console.WriteLine("Unmatched numbers!");
                return;
            }
            for (int i = 0; i < tokens.Count; ++i) {
                if (truth[i].GetType().Equals(tokens[i].GetType())) {
                    if (truth[i].line == tokens[i].line) {
                        continue;
                    }
                }
                Console.WriteLine("Wrong token!");
                return;
            }
            Console.WriteLine("Perfect!");
        }

    }
}
