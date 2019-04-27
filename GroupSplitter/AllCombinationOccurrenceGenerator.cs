using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupSplitter
{
    /// <summary>
    /// This implementation of <see cref="IOccurrenceGenerator"/> generates all possible <see cref="Occurrence">Occurrences</see> from a list of individuals.
    /// </summary>
    public class AllCombinationOccurrenceGenerator : IOccurrenceGenerator
    {
        /// <summary>
        /// Gets or sets the number of individuals who should be in each <see cref="Occurrence.Groupings">Grouping</see>.
        /// </summary>
        /// <remarks>
        /// If the <see cref="IReadOnlyList{T}.Count"/> of <see cref="Individuals"/> is not divisible by this number, one grouping will be larger and contain the remainder.
        /// </remarks>
        public int TargetGroupingCount { get; set; } = 2;

        /// <summary>
        /// Gets or sets the list of individuals who should be grouped.
        /// </summary>
        public IReadOnlyList<string> Individuals { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Occurrence.Date"/> to use for the generated for the occurrences.
        /// </summary>
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// A list of sets of individuals who do not need to meet.
        /// </summary>
        /// <remarks>
        /// This property can be used to specify subsets of individuals who already see each other regularly and do not need to be grouped.
        /// Individuals from these sets can still be be part of a larger <see cref="Occurrence.Grouping"/>grouping</see> when people from outside the set are included.
        /// </remarks>
        public IEnumerable<ISet<string>> ExemptMeetings { get; set; } = Enumerable.Empty<ISet<string>>();

        /// <summary>
        /// Enumerates all possible <see cref="Occurrence">Occurrences</see> that group <see cref="Individuals"/>.
        /// </summary>
        /// <returns>All possible <see cref="Occurrence">Occurrences</see> that group <see cref="Individuals"/>.</returns>
        public IEnumerable<Occurrence> EnumerateOccurrences()
        {
            // this implementation is wrong and should be fixed. https://github.com/RobinsonWM/GroupSplitter/issues/1
            // it seems to work well for targetGroupSize = 2
            for (int h = 0; h < Individuals.Count; h++)
            {
                var individuals = Individuals.ToArray();
                for (int i = h; i < Individuals.Count; i++)
                {
                    for (int j = 0; j < individuals.Length; j++)
                    {
                        var swap = individuals[j];
                        individuals[j] = individuals[i];
                        individuals[i] = swap;

                        var occurrence = BuildOccurrenceFromAdjacentElements(individuals);
                        if (!DoesOccurrenceContainExemptMeetings(occurrence))
                        {
                            yield return occurrence;
                        }
                    }
                }
            }
        }

        private bool DoesOccurrenceContainExemptMeetings(Occurrence occurrence)
        {
            var exemptGroupings = from exemptMeeting in ExemptMeetings
                                  from grouping in occurrence.Groupings
                                  where exemptMeeting.IsSupersetOf(grouping) // a grouping is exempt if _all_ the individuals in it exist in the same exemptMeeting
                                  select grouping;

            var doesOccurrenceContainExemptMeetings = exemptGroupings.Any();
            return doesOccurrenceContainExemptMeetings;
        }

        private Occurrence BuildOccurrenceFromAdjacentElements(IList<string> individuals)
        {
            var groupings = new List<List<string>>(individuals.Count / TargetGroupingCount)
                { new List<string>(TargetGroupingCount) };

            for (int i = 0; i < individuals.Count; i++)
            {
                if (groupings[groupings.Count - 1].Count >= TargetGroupingCount
                    && individuals.Count - i >= TargetGroupingCount) // the last grouping is larger than the others if there are not enough individuals to fill a grouping
                {
                    groupings.Add(new List<string>(TargetGroupingCount));
                }

                groupings[groupings.Count - 1].Add(individuals[i]);
            }

            return new Occurrence(Date, groupings);
        }
    }
}
