using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace GroupSplitter.UnitTests
{
    [TestFixture]
    public class OccurrencePickerTests
    {
        [Test]
        public void TestPickBestOccurrence()
        {
            const int occurrenceCount = 10;
            const int groupingsPerOccurrence = 5;
            const int individualsPerGrouping = 2;

            // build several meaningless occurrences to for the mock generator to return
            var occurrences = Enumerable
                .Range(0, occurrenceCount)
                .Select(_ => new Occurrence(DateTimeOffset.UtcNow,
                 Enumerable.Range(0, groupingsPerOccurrence)
                 .Select(__ => Enumerable.Range(0, individualsPerGrouping)
                 .Select(___ => Guid.NewGuid().ToString())
                 .ToList()).ToList())).ToList();

            // arbitrarily pick one to be the expected best occurrence
            var expectedOccurrence = occurrences[3];

            var generator = new Mock<IOccurrenceGenerator>(MockBehavior.Strict);
            generator.Setup(x => x.EnumerateOccurrences()).Returns(occurrences);

            // build a mock IPairingScorer that gives high scores to expectedOccurrence
            var scorer = new Mock<IPairingScorer>(MockBehavior.Strict);
            scorer.Setup(x => x.ScorePairing(It.IsIn(expectedOccurrence.Groupings.SelectMany(i => i)), It.IsAny<string>()))
                .Returns(50d);
            scorer.Setup(x => x.ScorePairing(It.IsNotIn(expectedOccurrence.Groupings.SelectMany(i => i)), It.IsAny<string>()))
                .Returns(1d);

            var picker = new OccurrencePicker(generator.Object, scorer.Object);
            var actualOccurrence = picker.PickBestOccurrence();

            Assert.That(actualOccurrence, Is.EqualTo(expectedOccurrence), "The correct occurrence should be selected");
        }

        [Test]
        public void TestIntegration()
        {
            const string a = "A";
            const string b = "B";
            const string c = "C";
            const string d = "D";

            var history = new[] {
                new Occurrence(DateTime.UtcNow.AddDays(-1), new[]{
                    new[] { a, b },
                    new[] { c, d }
                }),
                new Occurrence(DateTime.UtcNow.AddDays(-2), new[]{
                    new[] { a, c },
                    new[] { b, d }
                })
            };

            var scorer = new HistoryBasedScorer(history);

            var generator = new AllCombinationOccurrenceGenerator
            {
                Individuals = new[] { a, b, c, d },
                TargetGroupingCount = 2
            };

            var picker = new OccurrencePicker(generator, scorer);

            var occurrence = picker.PickBestOccurrence();

            Assert.That(occurrence.Groupings.ToList(), Has.Count.EqualTo(2));
            Assert.That(occurrence.Groupings.Any(x => x.Contains(a) && x.Contains(d)), $"{a} should meet with {d}");
            Assert.That(occurrence.Groupings.Any(x => x.Contains(b) && x.Contains(c)), $"{b} should meet with {c}");
        }
    }
}
