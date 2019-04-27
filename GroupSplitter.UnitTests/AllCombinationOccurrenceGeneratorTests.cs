using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupSplitter.UnitTests
{
    [TestFixture]
    public class AllCombinationOccurrenceGeneratorTests
    {
        [TestCase(10, 2)]
        [TestCase(11, 2)]
        [TestCase(9, 3)]
        [TestCase(11, 3)]
        public void TestEnumerateOccurrences_GroupingSize(int individualCount, int targetGroupingCount)
        {
            var individuals = Enumerable
                .Range(0, individualCount)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();
            var generator = new AllCombinationOccurrenceGenerator
            {
                Individuals = individuals,
                TargetGroupingCount = targetGroupingCount
            };

            var occurrences = generator.EnumerateOccurrences().ToList();

            if (individualCount % targetGroupingCount == 0)
            {
                foreach (var occurrence in occurrences)
                {
                    Assert.That(occurrence.Groupings, Has.All.Count.EqualTo(targetGroupingCount));
                }
            }
            else
            {
                foreach (var occurrence in occurrences)
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(occurrence.Groupings.Take(occurrence.Groupings.Count() - 1), Has.All.Count.EqualTo(targetGroupingCount));
                        Assert.That(occurrence.Groupings.Last(), Has.Count.EqualTo(targetGroupingCount + individualCount % targetGroupingCount), "The last grouping should hold the remainder");
                    });
                }
            }
        }

        [TestCase(10, 2)]
        [TestCase(11, 2)]
        [TestCase(9, 3)]
        [TestCase(11, 3)]
        public void TestEnumerateOccurrences_AllPairings(int individualCount, int targetGroupingCount)
        {
            var individuals = Enumerable
                .Range(0, individualCount)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();
            var generator = new AllCombinationOccurrenceGenerator
            {
                Individuals = individuals,
                TargetGroupingCount = targetGroupingCount
            };

            var occurrences = generator.EnumerateOccurrences().ToList();

            foreach (var individual in individuals)
            {
                foreach (var partner in individuals)
                {
                    if (individual != partner)
                    {
                        Assert.That(occurrences.Any(o => o.Groupings.Any(g =>
                          g.Contains(individual) && g.Contains(partner))),
                          $"At least one occurrence should group {individual} with {partner}");
                    }
                }
            }

            Assert.That(occurrences, Has.All.Property(nameof(Occurrence.Groupings)).All.Unique, "No one should ever meet with themself");
        }

        [TestCase(9, 3, IgnoreReason = "This test fails because of a bug in the implementation.  The implementation should be fixed.")]
        [TestCase(11, 3)]
        [TestCase(12, 4)]
        [TestCase(15, 4)]
        public void TestEnumerateOccurrences_AllTrios(int individualCount, int targetGroupingCount)
        {
            var individuals = Enumerable
                .Range(0, individualCount)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();
            var generator = new AllCombinationOccurrenceGenerator
            {
                Individuals = individuals,
                TargetGroupingCount = targetGroupingCount
            };

            var occurrences = generator.EnumerateOccurrences().ToList();

            foreach (var individual in individuals)
            {
                foreach (var partnerA in individuals)
                {
                    if (individual != partnerA)
                    {
                        foreach (var partnerB in individuals)
                        {
                            if (individual != partnerB && partnerA != partnerB)
                            {
                                Assert.That(occurrences.Any(o => o.Groupings.Any(g =>
                                  g.Contains(individual) && g.Contains(partnerA) && g.Contains(partnerB))),
                                  $"At least one occurrence should group {individual} with {partnerA} and {partnerB}");
                            }
                        }
                    }
                }
            }

            Assert.That(occurrences, Has.All.Property(nameof(Occurrence.Groupings)).All.Unique, "No one should ever meet with themself");
        }

        [Test]
        public void TestEnumerateOccurrences_Exemptions([Values(10, 13, 20, 21)]int individualCount)
        {
            var individuals = Enumerable
                .Range(0, individualCount)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();

            // these exemptions are just arbitrary
            var exemptA1 = individuals[1];
            var exemptA2 = individuals[4];
            var exemptA3 = individuals[2];
            var exemptB1 = individuals[6];
            var exemptB2 = individuals[9];

            var generator = new AllCombinationOccurrenceGenerator
            {
                Individuals = individuals,
                TargetGroupingCount = 2,
                ExemptMeetings = new[]
                {
                    new HashSet<string>{ exemptA1, exemptA2, exemptA3 },
                    new HashSet<string>{ exemptB1,exemptB2 }
                }
            };

            var occurrences = generator.EnumerateOccurrences().ToList();

            Assert.That(!occurrences.Any(o => o.Groupings.Where(g => g.Count() == 2).Any(g =>
                g.Contains(exemptA1) && g.Contains(exemptA2))), $"{exemptA1} should not meet with {exemptA2}");
            Assert.That(!occurrences.Any(o => o.Groupings.Where(g => g.Count() == 2).Any(g =>
              g.Contains(exemptA1) && g.Contains(exemptA3))), $"{exemptA1} should not meet with {exemptA3}");
            Assert.That(!occurrences.Any(o => o.Groupings.Where(g => g.Count() == 2).Any(g =>
              g.Contains(exemptA2) && g.Contains(exemptA3))), $"{exemptA2} should not meet with {exemptA3}");
            Assert.That(!occurrences.Any(o => o.Groupings.Where(g => g.Count() == 2).Any(g =>
              g.Contains(exemptB1) && g.Contains(exemptB2))), $"{exemptB1} should not meet with {exemptB2}");
        }

        public void TestEnumerateOccurrences_Date()
        {
            var expectedDate = DateTimeOffset.UtcNow;
            var individuals = Enumerable
                .Range(0, 5)
                .Select(_ => Guid.NewGuid().ToString())
                .ToList();
            var generator = new AllCombinationOccurrenceGenerator
            {
                Individuals = individuals,
                TargetGroupingCount = 2,
                Date = expectedDate
            };

            var occurrences = generator.EnumerateOccurrences().ToList();

            Assert.That(occurrences, Has.All.Property(nameof(Occurrence.Date)).EqualTo(expectedDate));
        }
    }
}
