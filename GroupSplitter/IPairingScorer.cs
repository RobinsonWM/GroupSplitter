namespace GroupSplitter
{
    /// <summary>
    /// This interface describes classes that assign a score to a meeting of one individual with a partner.
    /// </summary>
    public interface IPairingScorer
    {
        /// <summary>
        /// Calculates a numeric measurement of the merit of having an individual meet with a partner.
        /// </summary>
        /// <param name="individual">The first person of the meeting.</param>
        /// <param name="partner">The other person who will meet.</param>
        /// <returns>
        /// A high number if there is great merit for having <paramref name="individual"/> meet with
        /// <paramref name="partner"/>; otherwise a low number.
        /// </returns>
        double ScorePairing(string individual, string partner);
    }
}
