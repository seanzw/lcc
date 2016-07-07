using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public abstract class Node {
        public virtual void CGen(X86Gen gen) {
            throw new NotImplementedException();
        }
    }
    
}