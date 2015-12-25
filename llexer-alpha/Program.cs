using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llexer_alpha {
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
            ASTLex lexer;
            Parser.parse(f.ReadToEnd(), out lexer);

            lexer.writeLexer();

            f.Close();
        }
    }
}
