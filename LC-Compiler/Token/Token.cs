namespace lcc.Token {
    abstract class Token {
        public readonly int line;
        protected Token(int line) {
            this.line = line;
        }
    }
}
