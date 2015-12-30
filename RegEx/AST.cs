using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegEx {
    abstract class ASTRegEx {
        public ASTRegEx() { }

        abstract public string ToString(int level);

        public override string ToString() {
            return ToString(0);
        }

        protected string Tab(int n) {
            string ret = "";
            for (int i = 0; i < n; ++i) {
                ret += "    ";
            }
            return ret;
        }

        /// <summary>
        /// Generate a NFATable.
        /// </summary>
        /// <param name="map"> Map from char to class. </param>
        /// <param name="revMap"> Reverse map from class to chars. </param>
        /// <returns> The NFATable. </returns>
        protected internal abstract NFATable GenNFATable(int[] map, List<char[]> revMap);

        /// <summary>
        /// Build the map between char and its class in the transition table.
        /// 
        /// For any set1 and set2 in the map, with set1 != set2,
        /// join(set1, set2) = empty.
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        protected internal abstract LinkedList<HashSet<char>> BuildCharMap(LinkedList<HashSet<char>> map);
    }

    sealed class ASTExpr : ASTRegEx {

        public ASTExpr() {
            terms = new LinkedList<ASTTerm>();
        }

        public ASTExpr(LinkedList<ASTTerm> terms) {
            this.terms = terms;
        }

        public override string ToString(int level) {
            string str = Tab(level) + "Expression: \n";
            foreach (ASTTerm term in terms) {
                str += term.ToString(level + 1);
            }
            return str;
        }

        /// <summary>
        /// Generate the NFATable.
        /// This should only be called for the root node.
        /// </summary>
        /// <returns> NFATable for this regex. </returns>
        public NFATable ToNFATable() {

            // First generate the char map.
            LinkedList<HashSet<char>> sets = BuildCharMap(new LinkedList<HashSet<char>>());

            // Build the map and revMap.
            int[] map = new int[Const.CHARSIZE];
            for (int i = 0; i < map.Count(); ++i) {
                map[i] = -1;
            }
            List<char[]> revMap = new List<char[]>(sets.Count());
            int classId = 0;
            foreach (var set in sets) {
                foreach (char c in set) {
                    map[c] = classId;
                }
                revMap.Add(set.ToArray());
                classId++;
            }

            // Build the NFATable.
            return GenNFATable(map, revMap);
        }

        protected internal override LinkedList<HashSet<char>> BuildCharMap(LinkedList<HashSet<char>> map) {
            foreach (var term in terms) {
                map = term.BuildCharMap(map);
            }
            return map;
        }

        protected internal override NFATable GenNFATable(int[] map, List<char[]> revMap) {

            NFATable ret = terms.First().GenNFATable(map, revMap);
            foreach (var term in terms.Skip(1)) {
                ret = ret | term.GenNFATable(map, revMap);
            }
            return ret;

        }

        public LinkedList<ASTTerm> terms;
    }

    class ASTTerm : ASTRegEx {

        public ASTTerm() {
            factors = new LinkedList<ASTFactor>();
        }

        public ASTTerm(LinkedList<ASTFactor> factors) {
            this.factors = factors;
        }

        public override string ToString(int level) {
            string str = Tab(level) + "Term: \n";
            foreach (ASTFactor factor in factors) {
                str += factor.ToString(level + 1);
            }
            return str;
        }

        protected internal override LinkedList<HashSet<char>> BuildCharMap(LinkedList<HashSet<char>> map) {
            foreach (var factor in factors) {
                map = factor.BuildCharMap(map);
            }
            return map;
        }

        protected internal override NFATable GenNFATable(int[] map, List<char[]> revMap) {

            NFATable ret = factors.First().GenNFATable(map, revMap);
            foreach (var factor in factors.Skip(1)) {
                ret = ret + factor.GenNFATable(map, revMap);
            }
            return ret;

        }

        public LinkedList<ASTFactor> factors;
    }

    sealed class ASTFactor : ASTRegEx {

        public ASTFactor(ASTRegEx atom, MetaChar meta) {
            this.atom = atom;
            this.meta = meta;
        }

        public override string ToString(int level) {
            string str = Tab(level) + "Factor: " + meta + "\n";
            str += atom.ToString(level + 1);
            return str;
        }

        protected internal override LinkedList<HashSet<char>> BuildCharMap(LinkedList<HashSet<char>> map) {
            return atom.BuildCharMap(map);
        }

        protected internal override NFATable GenNFATable(int[] map, List<char[]> revMap) {
            NFATable ret = atom.GenNFATable(map, revMap);
            switch (meta) {
                case MetaChar.STAR:
                    return ret.Star();
                case MetaChar.PLUS:
                    return ret + ret.Star();
                case MetaChar.QUES:
                    return ret.Ques();
                case MetaChar.NULL:
                default:
                    return ret;
            }
        }

        public enum MetaChar {
            NULL,
            STAR,   /* 0 or more,   greddy. */
            QUES,   /* 0 or 1,      greddy. */
            PLUS    /* 1 or more,   greedy. */
        }

        public ASTRegEx atom;
        public MetaChar meta;
    }

    class ASTCharSet : ASTRegEx {
        public ASTCharSet(HashSet<char> set) {
            this.set = set;
        }

        protected ASTCharSet() { }

        public override string ToString(int level) {
            string str = Tab(level) + "CharSet: ";
            foreach (char c in set) {
                str += Utility.Print(c);
            }
            str += "\n";
            return str;
        }

        protected internal override LinkedList<HashSet<char>> BuildCharMap(LinkedList<HashSet<char>> map) {

            LinkedList<HashSet<char>> ret = new LinkedList<HashSet<char>>();
            HashSet<char> s = new HashSet<char>(set);

            foreach (var t in map) {
                HashSet<char> intersection = new HashSet<char>(s.Intersect(t));
                if (intersection.Count() > 0) {
                    ret.AddLast(intersection);
                    s.ExceptWith(intersection);
                    t.ExceptWith(intersection);
                    if (t.Count() > 0) {
                        ret.AddLast(t);
                    }
                } else {
                    ret.AddLast(t);
                }
            }

            if (s.Count() > 0) {
                ret.AddLast(s);
            }

            return ret;
        }

        protected internal override NFATable GenNFATable(int[] map, List<char[]> revMap) {
            NFATable ret = new NFATable(map, revMap);
            ret.AddState();
            ret.SetStateFinal(1);
            ret.AddTransition(0, 1, set);
            return ret;
        }

        public HashSet<char> set;
    }

    sealed class ASTCharSetWild : ASTCharSet {

        public ASTCharSetWild() {
            set = new HashSet<char>();
            for (int i = Const.FIRSTCHAR; i < Const.CHARSIZE; ++i) {
                set.Add((char)i);
            }
        }

        public override string ToString(int level) {
            string str = Tab(level) + "CharSet: WILD\n";
            return str;
        }

    }

    sealed class ASTCharSetNeg : ASTRegEx {
        public ASTCharSetNeg(HashSet<char> set) {
            this.set = set;
        }

        public override string ToString(int level) {
            string str = Tab(level) + "CharSetNeg: ";
            foreach (char c in set) {
                str += Utility.Print(c);
            }
            str += "\n";
            return str;
        }

        protected internal override LinkedList<HashSet<char>> BuildCharMap(LinkedList<HashSet<char>> map) {

            HashSet<char> pos = inverse();
            ASTCharSet tmp = new ASTCharSet(pos);
            return tmp.BuildCharMap(map);
        }

        protected internal override NFATable GenNFATable(int[] map, List<char[]> revMap) {
            HashSet<char> pos = inverse();
            ASTCharSet tmp = new ASTCharSet(pos);
            return tmp.GenNFATable(map, revMap);
        }

        private HashSet<char> inverse() {
            HashSet<char> pos = new HashSet<char>();
            for (int i = Const.FIRSTCHAR; i < Const.CHARSIZE; ++i) {
                if (!set.Contains((char)i)) {
                    pos.Add((char)i);
                }
            }
            return pos;
        }

        public HashSet<char> set;
    }
}
