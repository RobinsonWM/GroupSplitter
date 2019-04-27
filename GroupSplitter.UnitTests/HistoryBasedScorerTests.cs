using NUnit.Framework;
using System;

namespace GroupSplitter.UnitTests
{
    [TestFixture]
    public class HistoryBasedScorerTests
    {
        [Test]
        public void TestScorePairing()
        {
            const string a = "A";
            const string b = "B";
            const string c = "C";
            const string d = "D";
            const string e = "E";
            const string f = "F";

            var history = new[] {
                new Occurrence(DateTime.UtcNow.AddDays(-1), new[]{
                    new[] { a, b },
                    new[] { c, d },
                    new[] { e, f }
                }),
                new Occurrence(DateTime.UtcNow.AddDays(-2), new[]{
                    new[] { a, f },
                    new[] { c, b },
                    new[] { e, d }
                }),
                new Occurrence(DateTime.UtcNow.AddDays(-3), new[]{
                    new[] { a, d },
                    new[] { c, f },
                    new[] { e, b }
                })
            };


            int? rank = null; // we use this variable to capture the input to WeightFunction so we can check its value
            var scorer = new HistoryBasedScorer(history)
            {
                WeightFunction = i =>
                {
                    rank = i;
                    return i * 2d;
                }
            };


            Assert.That(scorer.ScorePairing(a, b), Is.Zero);
            Assert.That(rank, Is.Zero);

            Assert.That(scorer.ScorePairing(a, f), Is.EqualTo(2d));
            Assert.That(rank, Is.EqualTo(1d));

            Assert.That(scorer.ScorePairing(a, d), Is.EqualTo(4d));
            Assert.That(rank, Is.EqualTo(2d));

            Assert.That(scorer.ScorePairing(a, a), Is.EqualTo(6d));
            Assert.That(rank, Is.EqualTo(3d));

            Assert.That(scorer.ScorePairing(a, c), Is.EqualTo(6d));
            Assert.That(rank, Is.EqualTo(3d));

            Assert.That(scorer.ScorePairing("foo", a), Is.Zero);
            Assert.That(rank, Is.Zero);
            Assert.That(scorer.ScorePairing("foo", "bar"), Is.Zero);
            Assert.That(rank, Is.Zero);
        }
    }
}
