using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LinearAssignment.Tests
{
    public class SolverTest
    {
        [Theory]
        [MemberData(nameof(TestDataNegative))]
        public void SolveHandlesNegativeCosts(
            double[,] cost,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment,
            double[] expectedDualU,
            double[] expectedDualV)
        {
            var solution = Solver.Solve(cost);
            Assert.IsAssignableFrom<AssignmentWithDuals>(solution);
            var solutionWithDuals = (AssignmentWithDuals)solution;
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
            Assert.Equal(expectedDualU, solutionWithDuals.DualU);
            Assert.Equal(expectedDualV, solutionWithDuals.DualV);
        }

        public static IEnumerable<object[]> TestDataNegative => new[]
        {
            new object[]
            {
                new double[,] {{6, 6, 4, 7}, {5, 4, -3, -3}, {5, 3, 0, 6}},
                new[] {1, 3, 2},
                new[] {-1, 0, 2, 1},
                new double[] {6, -3, 2},
                new double[] {0, 0, -2, 0}
            },
            new object[]
            {
                new double[,] {{6, 5, 5}, {6, 4, 3}, {4, -3, 0}, {7, -3, 6}},
                new[] {-1, 0, 2, 1},
                new[] {1, 3, 2},
                new double[] {0, 0, -2, 0},
                new double[] {6, -3, 2}
            },
            new object[]
            {
                new[,]
                {
                    {-10, double.PositiveInfinity, double.PositiveInfinity},
                    {double.PositiveInfinity, double.PositiveInfinity, -19},
                    {double.PositiveInfinity, -13, double.PositiveInfinity}
                },
                new[] {0, 2, 1},
                new[] {0, 2, 1},
                new double[] {-10, -19, -13},
                new double[] {0, 0, 0}
            }
        };

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SolveLeavesCostInputUnchanged(bool maximize)
        {
            var cost = new double[,] { { 6, 6, 4, 7 }, { 5, 4, -3, -3 }, { 5, 3, 0, 6 } };
            var costCopy = cost.Clone() as double[,];
            Solver.Solve(cost, maximize);
            Assert.All(Enumerable.Range(0, cost.GetLength(0)),
                i => Assert.All(Enumerable.Range(0, cost.GetLength(1)), j => Assert.Equal(costCopy[i, j], cost[i, j])));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SolveLeavesCostInputUnchangedAfterTransposing(bool maximize)
        {
            var cost = new double[,] { { 6, 5, 5 }, { 6, 4, 3 }, { 4, -3, 0 }, { 7, -3, 6 } };
            var costCopy = cost.Clone() as double[,];
            Solver.Solve(cost, maximize);
            Assert.All(Enumerable.Range(0, cost.GetLength(0)),
                i => Assert.All(Enumerable.Range(0, cost.GetLength(1)), j => Assert.Equal(costCopy[i, j], cost[i, j])));
        }

        [Theory]
        [MemberData(nameof(TestDataMaximize))]
        public void SolveGivesExpectedResultWhenMaximizingWithFloatingPointCost(
            double[,] cost,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment,
            double[] expectedDualU,
            double[] expectedDualV)
        {
            var solution = Solver.Solve(cost, maximize: true);
            Assert.IsAssignableFrom<AssignmentWithDuals>(solution);
            var solutionWithDuals = (AssignmentWithDuals)solution;
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
            Assert.Equal(expectedDualU, solutionWithDuals.DualU);
            Assert.Equal(expectedDualV, solutionWithDuals.DualV);
        }

        public static IEnumerable<object[]> TestDataMaximize => new[]
        {
            new object[]
            {
                new double[,] {{400, 150, 400}, {400, 450, 600}, {300, 225, 300}},
                new[] {0, 2, 1},
                new[] {0, 2, 1},
                new double[] {325, 525, 225},
                new double[] {75, 0, 75}
            },
            new object[]
            {
                new double[,] {{400, 150, 400, 1}, {400, 450, 600, 2}, {300, 225, 300, 3}},
                new[] {0, 2, 1},
                new[] {0, 2, 1, -1},
                new double[] {325, 525, 225},
                new double[] {75, 0, 75, 0}
            },
            new object[]
            {
                new double[,] {{10, 10, 8}, {9, 8, 1}, {9, 7, 4}},
                new[] {2, 1, 0},
                new[] {2, 1, 0},
                new double[] {8, 6, 6},
                new double[] {3, 2, 0}
            },
            new object[]
            {
                new double[,] {{10, 10, 8, 11}, {9, 8, 1, 1}, {9, 7, 4, 10}},
                new[] {1, 0, 3},
                new[] {1, 0, -1, 2},
                new double[] {10, 9, 9},
                new double[] {0, 0, 0, 1}
            },
            new object[]
            {
                new double[,] {{6, 6, 4, 7}, {5, 4, -3, -3}, {5, 3, 0, 6}},
                new[] {1, 0, 3},
                new[] {1, 0, -1, 2},
                new double[] {6, 5, 5},
                new double[] {0, 0, 0, 1}
            },
            new object[]
            {
                new double[,] {{6, 5, 5}, {6, 4, 3}, {4, -3, 0}, {7, -3 , 6}},
                new[] {1, 0, -1, 2},
                new[] {1, 0, 3},
                new double[] {0, 0, 0, 1},
                new double[] {6, 5, 5}
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
                    {10, double.NegativeInfinity, double.NegativeInfinity},
                    {double.NegativeInfinity, double.NegativeInfinity, 1},
                    {double.NegativeInfinity, 7, double.NegativeInfinity}
                },
                new[] {0, 2, 1},
                new[] {0, 2, 1},
                new double[] {10, 1, 7},
                new double[] {0, 0, 0}
            },
            new object[]
            {
                new[,]
                {
                    {double.NegativeInfinity, 11, double.NegativeInfinity},
                    {11, 1, 10},
                    {double.NegativeInfinity, 7, 12}
                },
                new[] {1, 0, 2},
                new[] {1, 0, 2},
                new double[] {11, 11, 12},
                new double[] {0, 0, 0}
            },
            new object[]
            {
                new[,]
                {
                    {-10, double.NegativeInfinity, double.NegativeInfinity},
                    {double.NegativeInfinity, double.NegativeInfinity, -19},
                    {double.NegativeInfinity, -13, double.NegativeInfinity}
                },
                new[] {0, 2, 1},
                new[] {0, 2, 1},
                new double[] {-10, -19, -13},
                new double[] {0, 0, 0}
            }
        };

        [Theory]
        [MemberData(nameof(TestDataMaximizeInteger))]
        public void SolveGivesExpectedResultWhenMaximizingWithIntegerCost(
            int[,] cost,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment)
        {
            var solution = Solver.Solve(cost, maximize: true);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
        }

        public static IEnumerable<object[]> TestDataMaximizeInteger => new[]
        {
            new object[]
            {
                new[,] {{400, 150, 400}, {400, 450, 600}, {300, 225, 300}},
                new[] {0, 2, 1},
                new[] {0, 2, 1}
            },
            new object[]
            {
                new[,] {{10, 10, 8}, {9, 8, 1}, {9, 7, 4}},
                new[] {2, 1, 0},
                new[] {2, 1, 0}
            },
            new object[]
            {
                new int[,] {{ }, { }},
                new int[] { },
                new int[] { }
            },
            new object[]
            {
                new[,]
                {
                    {10, int.MinValue, int.MinValue},
                    {int.MinValue, int.MinValue, 1},
                    {int.MinValue, 7, int.MinValue}
                },
                new[] {0, 2, 1},
                new[] {0, 2, 1}
            },
            new object[]
            {
                new[,]
                {
                    {int.MinValue, 11, int.MinValue},
                    {11, 1, 10},
                    {int.MinValue, 7, 12}
                },
                new[] {1, 0, 2},
                new[] {1, 0, 2}
            },
            new object[]
            {
                new[,]
                {
                    {-10, int.MinValue, int.MinValue},
                    {int.MinValue, int.MinValue, -19},
                    {int.MinValue, -13, int.MinValue}
                },
                new[] {0, 2, 1},
                new[] {0, 2, 1}
            }
        };
    }
}
