using System;
using System.Collections.Generic;
using Xunit;

namespace LinearAssignment.Tests
{
    public class PseudoflowSolverTest
    {
        [Theory]
        [MemberData(nameof(TestDataMinimize))]
        public void SolveGivesExpectedResultWhenMinimizing(
            int[,] cost,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment)
        {
            var solver = new PseudoflowSolver();
            var solution = solver.Solve(cost);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
        }

        /// <summary>
        /// Include tests from the Python library scipy.optimize.
        /// </summary>
        public static IEnumerable<object[]> TestDataMinimize => new[]
        {
            new object[]
            {
                new int[,] {{400, 150, 400}, {400, 450, 600}, {300, 225, 300}},
                new[] {1, 0, 2},
                new[] {1, 0, 2}
            },
            new object[]
            {
                new int[,] {{10, 10, 8}, {9, 8, 1}, {9, 7, 4}},
                new[] {0, 2, 1},
                new[] {0, 2, 1}
            },
            new object[]
            {
                new[,]
                {
                    {10, int.MaxValue, int.MaxValue},
                    {int.MaxValue, int.MaxValue, 1},
                    {int.MaxValue, 7, int.MaxValue}
                },
                new[] {0, 2, 1},
                new[] {0, 2, 1}
            },
            new object[]
            {
                new[,]
                {
                    {int.MaxValue, 11, int.MaxValue},
                    {11, 1, 10},
                    {int.MaxValue, 7, 12}
                },
                new[] {1, 0, 2},
                new[] {1, 0, 2}
            }
        };

        [Fact]
        public void SolveThrowsWhenNoFeasibleSolutionExists()
        {
            var cost = new[,] { { int.MaxValue } };
            var solver = new PseudoflowSolver();
            Assert.Throws<InvalidOperationException>(() => solver.Solve(cost));
        }
    }
}