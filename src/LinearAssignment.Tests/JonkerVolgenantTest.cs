using System;
using System.Collections.Generic;
using Xunit;

namespace LinearAssignment.Tests
{
    public class JonkerVolgenantTest
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void SolveGivesExpectedResult(
            double[,] cost,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment,
            double[] expectedDualU,
            double[] expectedDualV)
        {
            var solution = JonkerVolgenant.Solve(cost);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
            Assert.Equal(expectedDualU, solution.DualU);
            Assert.Equal(expectedDualV, solution.DualV);
        }

        /// <summary>
        /// Include tests from the Python library scipy.optimize.
        /// </summary>
        public static IEnumerable<object[]> TestData => new[]
        {
            new object[]
            {
                new double[,] {{400, 150, 400}, {400, 450, 600}, {300, 225, 300}},
                new[] {1, 0, 2},
                new[] {1, 0, 2},
                new double[] {225, 400, 300},
                new double[] {0, -75, 0}
            },
            new object[]
            {
                new double[,] {{400, 150, 400, 1}, {400, 450, 600, 2}, {300, 225, 300, 3}},
                new[] {1, 3, 2},
                new[] {-1, 0, 2, 1},
                new double[] {225, 299, 300},
                new double[] {0, -75, 0, -297}
            },
            new object[]
            {
                new double[,] {{10, 10, 8}, {9, 8, 1}, {9, 7, 4}},
                new[] {0, 2, 1},
                new[] {0, 2, 1},
                new double[] {10, 4, 7},
                new double[] {0, 0, -3}
            },
            new object[]
            {
                new double[,] {{10, 10, 8, 11}, {9, 8, 1, 1}, {9, 7, 4, 10}},
                new[] {1, 3, 2},
                new[] {-1, 0, 2, 1},
                new double[] {10, 1, 6},
                new double[] {0, 0, -2, 0}
            },
            new object[]
            {
                new double[,] {{ }, { }},
                new int[] { },
                new int[] { },
                new double[] { },
                new double[] { }
            },
            new object[]
            {
                new[,]
                {
                    {10, double.PositiveInfinity, double.PositiveInfinity},
                    {double.PositiveInfinity, double.PositiveInfinity, 1},
                    {double.PositiveInfinity, 7, double.PositiveInfinity}
                },
                new[] {0, 2, 1},
                new[] {0, 2, 1},
                new double[] {10, 1, 7},
                new double[] {0, 0, 0}
            }
        };

        [Fact]
        public void SolveThrowsOnInputWithMoreRowsThanColumns()
        {
            var cost = new[,] {{1d},{2d}};
            Assert.Throws<ArgumentException>(() => JonkerVolgenant.Solve(cost));
        }

        [Fact]
        public void SolveThrowsOnNegativeInput()
        {
            var cost = new[,] {{-1d}};
            Assert.Throws<ArgumentException>(() => JonkerVolgenant.Solve(cost));
        }

        [Fact]
        public void SolveThrowsWhenNoFeasibleSolutionExists()
        {
            var cost = new[,] {{double.PositiveInfinity}};
            Assert.Throws<InvalidOperationException>(() => JonkerVolgenant.Solve(cost));
        }
    }
}