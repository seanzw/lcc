namespace Parserc {

    /// <summary>
    /// Wrap string as a token stream.
    /// </summary>
    public class CharStream : ITokenStream<char> {

        public CharStream(string src) {
            this.src = src;
            idx = 0;
        }

        private CharStream(CharStream other) {
            src = other.src;
            idx = other.idx;
        }

        public ITokenStream<char> Copy() {
            return new CharStream(this);
        }

        public bool More() {
            return idx < src.Length;
        }

        public char Next() { 
            if (More()) {
                return src[idx++];
            } else {
                return default(char);
            }
        }

        private int idx;
        private readonly string src;
    }
}
