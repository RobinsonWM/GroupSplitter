using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace GroupSplitter
{
    /// <summary>
    /// This implementation of <see cref="IPairingScorer"/> scores meetings based on how recently the two people have met.
    /// </summary>
    public class HistoryBasedScorer : IPairingScorer
    {
        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> _rankings;

        /// <summary>
        /// Sets a function that converts a numeric rank into a weighted score.
        /// </summary>
        /// <remarks>
        /// The input to this <see cref="Func{T, TResult}"/> how many distinct other partners the individual as met with
        /// since the previous meeting with the partner.  If the individual has never met with the partner, the input is
        /// equal to the total number of partners that individual has met with.  If the individual has never met with
        /// anyone, the input is 0.
        /// 
        /// This function may use a coefficient or exponent to control the tendency to produce cases where several
        /// individuals have a partner who they have not been grouped with for a long time, but a small number of
        /// individuals have partners they have been grouped with recently.
        /// </remarks>
        public Func<int, double> WeightFunction { private get; set; } = x => 3d * x;

        /// <summary>
        /// Creates a new <see cref="HistoryBasedScorer"/> from a history.
        /// </summary>
        /// <param name="history">Gets or sets all the past <see cref="Occurrence">Occurrences</see> where individuals have met with one another.</param>
        public HistoryBasedScorer(IEnumerable<Occurrence> history)
        {
            _rankings = BuildRankings(history);
        }

        /// <summary>
        /// Calculates a numeric measurement of the merit of having an individual meet with a partner.
        /// </summary>
        /// <param name="individual">The first person of the meeting.</param>
        /// <param name="partner">The other person who will meet.</param>
        /// <returns>
        /// A high number if there is great merit for having <paramref name="individual"/> meet with
        /// <paramref name="partner"/>; otherwise a low number.
        /// </returns>
        public double ScorePairing(string individual, string partner)
        {
            int ranking;
            if (_rankings.TryGetValue(individual, out IReadOnlyDictionary<string, int> rankings))
            {
                if (!rankings.TryGetValue(partner, out ranking))
                {
                    // individual has never met with partner; ranking is one higher than individual's highest ranking
                    ranking = rankings.Count;
                    Debug.Assert(ranking == rankings.Values.Max() + 1, "Bad ranking", "Ranking should be one higher than the individual's highest ranking.  Since the values in the dictionary should be 0,1,2,... the count should be on higher than the highest value.");
                }
                // else individual has met with partner, and a low ranking indicates it was recent
            }
            else
            {
                // individual has never met with anyone
                ranking = 0;
            }

            var score = WeightFunction(ranking);
            return score;
        }

        private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, int>> BuildRankings(IEnumerable<Occurrence> history)
        {
            var allIndividuals = (from occurrence in history
                                  from grouping in occurrence.Groupings
                                  from individual in grouping
                                  select individual)
                              .Distinct();

            var rankings = allIndividuals
                .ToDictionary(
                    x => x,
                    x => BuildRankingsForIndividual(x, history),
                    StringComparer.InvariantCultureIgnoreCase);

            return new ReadOnlyDictionary<string, IReadOnlyDictionary<string, int>>(rankings);
        }

        private static IReadOnlyDictionary<string, int> BuildRankingsForIndividual(string individual, IEnumerable<Occurrence> history)
        {
            var sequence = 0;
            var rankings = (from occurrence in history
                            from grouping in occurrence.Groupings
                            where grouping.Contains(individual)
                            from partner in grouping
                            where partner != individual
                            group occurrence.Date by partner into partnerHistory
                            orderby partnerHistory.Max() descending
                            select new { Partner = partnerHistory.Key, Ranking = sequence++ })
                .ToDictionary(x => x.Partner, x => x.Ranking, StringComparer.InvariantCultureIgnoreCase);

            return new ReadOnlyDictionary<string, int>(rankings);
        }
    }
}
