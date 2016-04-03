namespace Parserc {

    public interface IParserResult<I, out V> {
        V Value { get; }
        ITokenStream<I> Remain { get; }
    }

    public class ParserResult<I, V> : IParserResult<I, V> {

        private readonly V value;

        private readonly ITokenStream<I> remain;

        public ParserResult(V value, ITokenStream<I> remain) {
            this.value = value;
            this.remain = remain;
        }

        public V Value => value;
        public ITokenStream<I> Remain => remain;
    }
}
