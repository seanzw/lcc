﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.Token;
using LLexer;

namespace lcc.Test {
    static class TestLexer {

        private static void Aux(string src, List<Token.Token> truth) {
            var tokens = Lexer.Instance.Scan(src);
            if (tokens.Count != truth.Count) {
                Console.WriteLine("Unmatched numbers!");
                return;
            }
            for (int i = 0; i < tokens.Count; ++i) {
                if (truth[i].GetType().Equals(tokens[i].GetType())) {
                    if (truth[i].line == tokens[i].line) {
                        if ((tokens[i] as T_IDENTIFIER) != null && (truth[i] as T_IDENTIFIER) != null) {
                            var i1 = truth[i] as T_IDENTIFIER;
                            var i2 = tokens[i] as T_IDENTIFIER;
                            if (i1.name == i2.name) {
                                continue;
                            }
                        } else if ((tokens[i] as T_CONST_INT) != null && (truth[i] as T_CONST_INT) != null) {
                            var i1 = truth[i] as T_CONST_INT;
                            var i2 = tokens[i] as T_CONST_INT;
                            if (i1.n == i2.n && i1.suffix == i2.suffix && i1.text == i2.text) {
                                continue;
                            }
                        } else if ((tokens[i] as T_CONST_FLOAT) != null && (truth[i] as T_CONST_FLOAT) != null) {
                            var i1 = truth[i] as T_CONST_FLOAT;
                            var i2 = tokens[i] as T_CONST_FLOAT;
                            if (i1.n == i2.n && i1.suffix == i2.suffix && i1.text == i2.text) {
                                continue;
                            }
                        } else {
                            continue;
                        }
                    }
                }
                Console.WriteLine("Wrong token!");
                return;
            }
            Console.WriteLine("Perfect!");
        }

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
            Aux(src, truth);
        }

        public static void TestIdentifier() {
            string src = @"
int a char b double _a int a0 what c_0_CA_0
            ";
            List<Token.Token> truth = new List<Token.Token> {
                new T_KEY_INT(2),
                new T_IDENTIFIER(2, "a"),
                new T_KEY_CHAR(2),
                new T_IDENTIFIER(2, "b"),
                new T_KEY_DOUBLE(2),
                new T_IDENTIFIER(2, "_a"),
                new T_KEY_INT(2),
                new T_IDENTIFIER(2, "a0"),
                new T_IDENTIFIER(2, "what"),
                new T_IDENTIFIER(2, "c_0_CA_0")
            };
            Aux(src, truth);
        }

        public static void TestConstantInt() {
            string src = @"
0123 012u 043l 076ll 054Ull 045Lu
0x123u 0x234l 0XFELL
123u
            ";
            List<Token.Token> truth = new List<Token.Token> {
                new T_CONST_INT(2, "123", 8),
                new T_CONST_INT(2, "12u", 8),
                new T_CONST_INT(2, "43l", 8),
                new T_CONST_INT(2, "76ll", 8),
                new T_CONST_INT(2, "54Ull", 8),
                new T_CONST_INT(2, "45Lu", 8),
                new T_CONST_INT(3, "123u", 16),
                new T_CONST_INT(3, "234l", 16),
                new T_CONST_INT(3, "FELL", 16),
                new T_CONST_INT(4, "123u", 10)
            };
            Aux(src, truth);
        }
        public static void TestConstantFloat() {
            string src = @"
1.23e+4f .34f 1.23 .45 2e4l
0x1f.34p12f 0X23p28l
            ";
            List<Token.Token> truth = new List<Token.Token> {
                new T_CONST_FLOAT(2, "1.23e+4f", 10),
                new T_CONST_FLOAT(2, ".34f", 10),
                new T_CONST_FLOAT(2, "1.23", 10),
                new T_CONST_FLOAT(2, ".45", 10),
                new T_CONST_FLOAT(2, "2e4l", 10),
                new T_CONST_FLOAT(3, "1f.34p12f", 16),
                new T_CONST_FLOAT(3, "23p28l", 16)
            };
            Aux(src, truth);
        }

    }
}