using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using lcc.TypeSystem;

namespace lcc.AST {
    public abstract class Node {

    }

    public abstract class Expr : Node {

    }

    public abstract class ConstExpr : Expr{

    }

    /// <summary>
    /// Arithmetic constant expression
    ///     - integer constant expression
    ///     - float constant expression
    /// </summary>
    public abstract class ConstArithExpr : ConstExpr {
        
    }

    /// <summary>
    /// Integer constant expression.
    /// </summary>
    public sealed class ConstIntExpr : ConstArithExpr {
        public readonly TInteger t;
        public readonly BigInteger value;
        public ConstIntExpr(TInteger t, BigInteger value) {
            this.t = t;
            this.value = value;
        }
    }

    /// <summary>
    /// Float constant expression.
    /// </summary>
    public sealed class ConstFloatExpr : ConstArithExpr {
        public readonly TArithmetic t;
        public readonly double value;
        public ConstFloatExpr(TArithmetic t, double value) {
            this.t = t;
            this.value = value;
        }
    }

    /// <summary>
    /// An address constant is a null pointer, a pointer to an lvalue designating an object of static storage duration,
    /// or a pointer to a function.
    /// </summary>
    public abstract class ConstAddrExpr : ConstExpr {
    }

    /// <summary>
    /// A pointer to an lvalue designating an object of static storage duration.
    /// </summary>
    public sealed class ConstAddrObj : ConstAddrExpr {
        public readonly Entry entry;
        public ConstAddrObj(Entry entry) {
            this.entry = entry;
        }
    }

    public sealed class Declaration : Node {

        public readonly Tuple<T, >
    }
}
