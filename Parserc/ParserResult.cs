namespace Parserc {
    public class ParserResult<I, V> {

        public readonly V value;

        public readonly ITokenStream<I> remain;

        public ParserResult(V value, ITokenStream<I> remain) {
            this.value = value;
            this.remain = remain;
        }
    }

    public static class ResultAux {

        public static bool Succeed<I, V>(ParserResult<I, V>[] result) {
            return result.Length > 0;
        }

        public static ParserResult<I, V>[] Failure<I, V>() {
            return new ParserResult<I, V>[] { };
        }

    }
}
