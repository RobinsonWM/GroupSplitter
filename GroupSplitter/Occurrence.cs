using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupSplitter
{
    /// <summary>
    /// Describes an immutable occurrence when multiple groupings of individuals meet together on a specific date.
    /// </summary>
    public class Occurrence
    {
        /// <summary>
        /// The date on which the individuals will meet.
        /// </summary>
        public DateTimeOffset Date { get; }

        /// <summary>
        /// Each element in this <see cref="IEnumerable{T}"/> represents a group of individuals who will meet.
        /// </summary>
        public IEnumerable<IEnumerable<string>> Groupings { get; }

        /// <summary>
        /// Instantiates a new <see cref="Occurrence"/> from the given date and groupings.
        /// </summary>
        /// <param name="date">The date on which the individuals will meet.</param>
        /// <param name="groupings">Each element in this <see cref="IEnumerable{T}"/> represents a group of individuals who will meet.</param>
        public Occurrence(DateTimeOffset date, IEnumerable<IEnumerable<string>> groupings)
        {
            Date = date;
            Groupings = groupings.Select(x => x.ToList());
        }
    }
}
