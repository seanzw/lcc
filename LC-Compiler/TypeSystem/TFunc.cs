using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lcc.TypeSystem {
    public sealed class TFunc : TUnqualified {

        public TFunc(T ret, IEnumerable<T> parameters, bool isEllipis) {
            this.ret = ret;
            this.parameters = parameters;
            this.isEllipis = isEllipis;
            isDefined = false;
        }

        public override bool IsFunc => true;
        public override bool IsComplete => true;
        public override bool IsDefined => isDefined;

        public override void DefFunc() {
            if (isDefined) throw new InvalidOperationException("Can't define a function which is already defined.");
            else isDefined = true;
        }

        public override int Bits { get { throw new InvalidOperationException("Can't take bits of func designator!"); } } 

        public override bool Equals(object obj) {
            return Equals(obj as TFunc);
        }

        public bool Equals(TFunc t) {
            return t != null && t.ret.Equals(ret) && t.parameters.SequenceEqual(parameters);
        }

        public override int GetHashCode() {
            return ret.GetHashCode();
        }

        /// <summary>
        /// For two function types to be compatible, both shall specify compatible returns types.
        /// Moreover, the parameter type lists, if both are present, shall agree in the number of parameters
        /// and in use of the ellipsis terminator.
        /// TODO: Support old style function type.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Compatible(TUnqualified other) {
            if (other.IsFunc) {
                TFunc f = other as TFunc;
                if (ret.Compatible(f.ret) && isEllipis == f.isEllipis && parameters.Count() == f.parameters.Count()) {
                    return parameters.Zip(f.parameters, (p, q) => p.Compatible(q)).Aggregate(true, (x, y) => x && y);
                }
            }
            return false;
        }

        public override TUnqualified Composite(TUnqualified other) {
            throw new NotImplementedException();
        }

        public override string ToString() {
            string paramsStr = parameters.Aggregate("", (str, param) => str == "" ? param.ToString() : str + ", " + param.ToString());
            if (isEllipis) {
                paramsStr = paramsStr == "" ? "..." : paramsStr + ", ...";
            }
            return string.Format("({1}) -> {0}", ret, paramsStr);
        }

        public readonly T ret;
        public readonly IEnumerable<T> parameters;
        public readonly bool isEllipis;

        /// <summary>
        /// Whether the definition of this function has been detected.
        /// </summary>
        private bool isDefined;
    }
}
