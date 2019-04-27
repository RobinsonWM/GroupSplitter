using Moq;
using NUnit.Framework;
using System;

namespace GroupSplitter.UnitTests
{
    [TestFixture]
    public class ScorerExtensionsTests
    {
        [Test]
        public void TestScoreGrouping_Pair()
        {
            const string a = "A";
            const string b = "B";
            const double aWithB = 5d;
            const double bWithA = 7d;
            const double expectedScore = aWithB + bWithA;
            var grouping = new[] { a, b };

            var scorer = new Mock<IPairingScorer>(MockBehavior.Strict);
            scorer.Setup(x => x.ScorePairing(a, b))
                .Returns(aWithB);
            scorer.Setup(x => x.ScorePairing(b, a))
                .Returns(bWithA);

            var actualScore = scorer.Object.ScoreGrouping(grouping);

            Assert.That(actualScore, Is.EqualTo(expectedScore));
        }

        [Test]
        public void TestScoreGrouping_Three()
        {
            const string a = "A";
            const string b = "B";
            const string c = "C";
            const double aWithB = 5d;
            const double aWithC = 3d;
            const double bWithA = 7d;
            const double bWithC = 2d;
            const double cWithA = 8d;
            const double cWithB = 9d;
            const double expectedScore = aWithB + aWithC + bWithA + bWithC + cWithA + cWithB;
            var grouping = new[] { a, b, c };

            var scorer = new Mock<IPairingScorer>(MockBehavior.Strict);
            scorer.Setup(x => x.ScorePairing(a, b))
                .Returns(aWithB);
            scorer.Setup(x => x.ScorePairing(a, c))
                .Returns(aWithC);
            scorer.Setup(x => x.ScorePairing(b, a))
                .Returns(bWithA);
            scorer.Setup(x => x.ScorePairing(b, c))
                .Returns(bWithC);
            scorer.Setup(x => x.ScorePairing(c, a))
                .Returns(cWithA);
            scorer.Setup(x => x.ScorePairing(c, b))
                .Returns(cWithB);

            var actualScore = scorer.Object.ScoreGrouping(grouping);

            Assert.That(actualScore, Is.EqualTo(expectedScore));
        }

        [Test]
        public void TestScoreOccurrence()
        {
            const string a = "A";
            const string b = "B";
            const string c = "C";
            const string d = "D";
            const double aWithB = 5d;
            const double bWithA = 7d;
            const double cWithD = 2d;
            const double dWithC = 1d;
            const double expectedScore = aWithB + bWithA + cWithD + dWithC;
            var groupings = new[] {
                new[] { a,b },
                new[] { c,d }
            };
            var occurrence = new Occurrence(DateTimeOffset.Now, groupings);

            var scorer = new Mock<IPairingScorer>(MockBehavior.Strict);
            scorer.Setup(x => x.ScorePairing(a, b))
                .Returns(aWithB);
            scorer.Setup(x => x.ScorePairing(b, a))
                .Returns(bWithA);
            scorer.Setup(x => x.ScorePairing(c, d))
                .Returns(cWithD);
            scorer.Setup(x => x.ScorePairing(d, c))
                .Returns(dWithC);

            var actualScore = scorer.Object.ScoreOccurrence(occurrence);

            Assert.That(actualScore, Is.EqualTo(expectedScore));
        }
    }
}
