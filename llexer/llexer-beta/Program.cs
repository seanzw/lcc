using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLexerBeta {
    class Program {
        static void Main(string[] args) {

            if (args.Length < 1) {
                Console.WriteLine("Usage: lexer [lex file]");
                return;
            }

            if (!File.Exists(args[0])) {
                Console.WriteLine(args[0] + " doesn't exist.");
                return;
            }

            Console.WriteLine("Reading from " + args[0]);
            StreamReader f = new StreamReader(args[0]);

            string src = f.ReadToEnd();
            List<Token> tokens = LLexer.Lexer.Instance.Scan(src);
            ASTLex ast = LParser.Parser.Instance.Parse(tokens);
            Console.WriteLine(ast);

            StreamWriter o = new StreamWriter("Lexer.cs");
            o.Write(ast.WriteLexer());

            o.Close();
            f.Close();
        }
    }
}
