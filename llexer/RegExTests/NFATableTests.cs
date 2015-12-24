using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegEx;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace RegExTests {
    [TestClass]
    public class NFATableTests {
    
        [TestMethod]
        public void nfa_basic_test() {
            int SIZE = 1 << 16;
            int[] map = new int[SIZE];
            for (int i = 0; i < SIZE; ++i) {
                map[i] = -1;
            }
            map['a'] = 0;
            map['b'] = 1;
            List<char[]> revMap = new List<char[]> {
                new char[] {'a'},
                new char[] {'b'}
            };
            NFATable nfa = new NFATable(map, revMap);
            Console.WriteLine(nfa);

            nfa.addState();
            Console.WriteLine(nfa);

            nfa.addTransition(0, 1, new HashSet<char> { 'a', 'b' });
            Console.WriteLine(nfa);

            nfa.setStateFinal(1);
            Console.WriteLine(nfa);

            NFATable star = nfa.star();
            Console.WriteLine(star);

            NFATable nfa2 = new NFATable(map, revMap);
            nfa2.addState();
            nfa2.addTransition(0, 1, new HashSet<char> { 'b' });
            nfa2.setStateFinal(1);
            Console.WriteLine(nfa + nfa2);
            Console.WriteLine(nfa | nfa2);

        }

    }
}

