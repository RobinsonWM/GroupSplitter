using System.Collections.Generic;
using System.Linq;

namespace GroupSplitter
{
    /// <summary>
    /// This static class exposese extension methods to faciliate using a <see cref="IPairingScorer"/> to score constructs larger
    /// than a pairing of two individuals.
    /// </summary>
    public static class ScorerExtensions
    {
        /// <summary>
        /// Uses a <see cref="IPairingScorer"/> to score all groupings of an entire <see cref="Occurrence"/>
        /// </summary>
        /// <param name="scorer">The <see cref="IPairingScorer"/> to use for scoring the <see cref="Occurrence"/>.</param>
        /// <param name="occurrence">The <see cref="Occurrence"/> to score</param>
        /// <returns></returns>
        public static double ScoreOccurrence(this IPairingScorer scorer, Occurrence occurrence)
        {
            var scores = from grouping in occurrence.Groupings
                         select scorer.ScoreGrouping(grouping);

            return scores.Sum(x => x);
        }

        /// <summary>
        /// Uses a <see cref="IPairingScorer"/> to score an entire <see cref="Occurrence.Groupings">Grouping</see> of individuals.
        /// </summary>
        /// <param name="scorer">The <see cref="IPairingScorer"/> to use for scoring the grouping.</param>
        /// <param name="grouping">The <see cref="Occurrence.Groupings">Grouping</see> to score</param>
        /// <returns>A score for the entire grouping</returns>
        /// <remarks>
        /// The score will be calculated from the perspective of each individual.  For example, if a <paramref name="grouping"/>
        /// includes individuals A and B, the result will be the score with A as the individual and B as the partner plus the score
        /// with B as the individual and A as the partner.
        /// </remarks>
        public static double ScoreGrouping(this IPairingScorer scorer, IEnumerable<string> grouping)
        {
            var scores = from individual in grouping
                         from partner in grouping
                         where partner != individual
                         select scorer.ScorePairing(individual, partner);

            return scores.Sum(x => x);
        }
    }
}
