using System.Collections.Generic;

namespace GroupSplitter
{
    /// <summary>
    /// This interface describes classes that generate multiple instances of <see cref="Occurrence"/>.
    /// </summary>
    public interface IOccurrenceGenerator
    {
        /// <summary>
        /// Enumerates multiple instances of <see cref="Occurrence"/>.
        /// </summary>
        /// <remarks>Implementations are not guaranteed to be thread-safe for parallel LINQ.</remarks>
        /// <returns>Multiple instances of <see cref="Occurrence"/></returns>
        IEnumerable<Occurrence> EnumerateOccurrences();
    }
}
