using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace llexer {
    class Program {
        static void Main(string[] args) {

            StreamReader f = new StreamReader(args[0]);
            var tokens = llexer.Main.scan(f.ReadToEnd());
            foreach (var token in tokens) {
                Console.WriteLine(token);
            }

        }
    }
}
