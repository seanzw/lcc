using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parserc {

    public static class Recursion {

        public delegate Func<A, R> Recursive<A, R>(Recursive<A, R> r);

        public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f) {
            Recursive<A, R> rec = r => a => f(r(r))(a);
            return rec(rec);
        }
    }


}
