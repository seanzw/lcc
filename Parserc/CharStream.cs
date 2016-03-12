namespace Parserc {

    /// <summary>
    /// Wrap string as a token stream.
    /// </summary>
    public class CharStream : ITokenStream<char> {

        public CharStream(string src) {
            this.src = src;
            idx = 0;
        }

        /// <summary>
        /// Used for tail.
        /// Notice the idx of the new stream is incremented with one.
        /// </summary>
        /// <param name="other"></param>
        private CharStream(CharStream other) {
            src = other.src;
            idx = other.idx + 1;
        }

        public bool More() {
            return idx < src.Length;
        }

        public char Head() { 
            if (More()) {
                return src[idx];
            } else {
                return default(char);
            }
        }

        public ITokenStream<char> Tail() {
            return new CharStream(this);
        }

        private int idx;
        private readonly string src;
    }
}
