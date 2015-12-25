using System;
using System.IO;

namespace llexer_alpha {
    class Tester {

        delegate Func<A, R> Recusive<A, R>(Recusive<A, R> r);
        static public void testRecursive() {

            Recusive<int, int> fibRec = f =>
                n => {
                    if (n > 1) {
                        return f(f)(n - 2) + f(f)(n - 1);
                    } else {
                        return n;
                    }
                };

            Func<int, int> fib = fibRec(fibRec);
            Console.WriteLine(fib(6));
        }

    }
}
