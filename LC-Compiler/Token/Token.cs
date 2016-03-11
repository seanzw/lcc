namespace lcc.Token {
    public abstract class Token {
        public readonly int line;

        protected Token(int line) {
            this.line = line;
        }

        /// <summary>
        /// Equals test.
        /// A little reflect to save code...
        /// </summary>
        /// <param name="obj"> Object to be compared. </param>
        /// <returns> True if they are the same type. </returns>
        public override bool Equals(object obj) {
            Token t = obj as Token;
            return t == null ? false : t.line == line
                && GetType().Equals(obj.GetType());
        }

        public override int GetHashCode() {
            return line;
        }

    }
}
