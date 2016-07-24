using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.SyntaxTree;
using T = lcc.Token.Token;
using lcc.AST;
using P = lcc.Parser.Parser;
using LLexer;
using Parserc;

namespace lcc {
    class Program {
        static void Main(string[] args) {
            foreach (var fn in args) {
                if (!File.Exists(fn)) {
                    Console.WriteLine("cannot find file: {0}", fn);
                }
                string src = File.ReadAllText(fn);

                /// Stage 1: lexer.
                ReadOnlyCollection<T> tokens;
                try {
                    tokens = new ReadOnlyCollection<T>(Lexer.Instance.Scan(src));
                } catch (LexerException e) {
                    Console.WriteLine(e.Message);
                    return;
                }

                
                /// Stage 2: parser.
                Parser.Env.Clear();
                var stream = new TokenStream<T>(tokens);
                var results = P.TranslationUnit().End()(stream);
                if (results.Count() == 0) {
                    Console.WriteLine("Parser Error: parsing failed");
                    return;
                }
                if (results.Count() > 1) {
                    Console.WriteLine("Parser Error: ambiguity");
                    return;
                }

                /// Stage 3: type check and build ast.
                AST.Node ast = null;
                try {
                    ast = results.First().Value.ToAST(new SyntaxTree.Env());
                } catch (Error e) {
                    Console.WriteLine(e.Message);
                    return;
                }

                /// Stage 4: code generation and write to file.
                var gen = new X86Gen();
                ast.ToX86(gen);
                if (fn.Length > 2 && fn.Substring(fn.Length - 2) == ".c") {
                    File.WriteAllText(fn.Substring(0, fn.Length - 2) + ".s", gen.ToString());
                } else {
                    File.WriteAllText(fn + ".s", gen.ToString());
                }
            }
        }
    }
}
