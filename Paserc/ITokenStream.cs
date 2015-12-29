using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parserc {
    public interface ITokenStream<T> {

        /// <summary>
        /// Generate a copy of this stream.
        /// This is used for backup.
        /// </summary>
        /// <returns></returns>
        ITokenStream<T> Copy();

        bool More();

        T Next();
    }
}
