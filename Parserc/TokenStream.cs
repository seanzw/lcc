using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parserc {
    public class TokenStream<T> : ITokenStream<T> {

        public TokenStream(ReadOnlyCollection<T> tokens) {
            this.tokens = tokens;
            idx = 0;
        }

        /// <summary>
        /// Shallow copy.
        /// </summary>
        /// <param name="other"></param>
        private TokenStream(TokenStream<T> other) {
            tokens = other.tokens;
            idx = other.idx + 1;
        }

        public ITokenStream<T> Tail() {
            return new TokenStream<T>(this);
        }

        public bool More() {
            return idx < tokens.Count();
        }

        public T Head() {
            if (More()) {
                return tokens[idx];
            } else {
                return default(T);
            }
        }

        private int idx;
        private readonly ReadOnlyCollection<T> tokens;

    }
}
