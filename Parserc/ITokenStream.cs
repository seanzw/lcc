using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parserc {
    public interface ITokenStream<out T> {

        /// <summary>
        /// Return true if this stream is not empty.
        /// </summary>
        /// <returns> Boolean. </returns>
        bool More();

        /// <summary>
        /// Get the next element in the stream.
        /// </summary>
        /// <returns> T. </returns>
        T Head();

        /// <summary>
        /// Get the rest of the stream, without head.
        /// This will make a new stream.
        /// </summary>
        /// <returns> ITokenStream. </returns>
        ITokenStream<T> Tail();
    }
}
