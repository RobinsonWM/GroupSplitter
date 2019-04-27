namespace GroupSplitter
{
    /// <summary>
    /// This class uses a <see cref="IOccurrenceGenerator"/> and <see cref="IPairingScorer"/> to generate several
    /// <see cref="Occurrence">Occurrences</see> and select the one with the highest score.
    /// </summary>
    public class OccurrencePicker
    {
        private readonly IOccurrenceGenerator _generator;
        private readonly IPairingScorer _scorer;

        /// <summary>
        /// Creates a new <see cref="OccurrencePicker"/> from a given <see cref="IOccurrenceGenerator"/> and <see cref="IPairingScorer"/>.
        /// </summary>
        /// <param name="generator">A <see cref="IOccurrenceGenerator"/> to generate the occurrences to score</param>
        /// <param name="scorer">A <see cref="IPairingScorer"/> to score the occurrences</param>
        public OccurrencePicker(IOccurrenceGenerator generator, IPairingScorer scorer)
        {
            _generator = generator;
            _scorer = scorer;
        }

        /// <summary>
        /// Generates all occurrences and returns the one with the highest score.
        /// </summary>
        /// <returns>The occurrence with the highest score, or <c>null</c> if no occurrences were generated.</returns>
        /// <remarks>If multiple occurrences have the same score, one of them will be returned.</remarks>
        public Occurrence PickBestOccurrence()
        {
            var bestScore = double.MinValue;
            Occurrence bestMatch = null;
            
            foreach (var occurrence in _generator.EnumerateOccurrences())
            {
                var score = _scorer.ScoreOccurrence(occurrence);
                if (score > bestScore)
                {
                    bestMatch = occurrence;
                    bestScore = score;
                }
            }

            return bestMatch;
        }
    }
}
