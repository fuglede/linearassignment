using System;
using System.Collections.Generic;
using Xunit;

namespace LinearAssignment.Tests
{
    public class ShortestPathSolverTest
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
            var solver = new ShortestPathSolver();
            var solution = solver.Solve(cost);
            Assert.IsAssignableFrom<AssignmentWithDuals>(solution);
            var solutionWithDuals = (AssignmentWithDuals) solution;
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
            Assert.Equal(expectedDualU, solutionWithDuals.DualU);
            Assert.Equal(expectedDualV, solutionWithDuals.DualV);
        }

        [Theory]
        [MemberData(nameof(TestDataSparse))]
        public void SolveSparseGivesExpectedResult(
            double[,] dense,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment)
        {
            var solver = new ShortestPathSolver();
            var cost = new SparseMatrixDouble(dense);
            var solution = solver.Solve(cost);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
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
            },
            new object[]
            {
                new[,]
                {
                    {double.PositiveInfinity, 11, double.PositiveInfinity},
                    {11, 1, 10},
                    {double.PositiveInfinity, 7, 12}
                },
                new[] {1, 0, 2},
                new[] {1, 0, 2},
                new double[] {21, 11, 13},
                new double[] {0, -10, -1}
            }
        };

        public static IEnumerable<object[]> TestDataSparse => new[]
        {
            new object[]
            {
                new double[,] {{400, 150, 400}, {400, 450, 600}, {300, 225, 300}},
                new[] {1, 0, 2},
                new[] {1, 0, 2}
            },
            new object[]
            {
                new double[,] {{10, 10, 8}, {9, 8, 1}, {9, 7, 4}},
                new[] {0, 2, 1},
                new[] {0, 2, 1}
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
                new[] {0, 2, 1}
            },
            new object[]
            {
                new[,]
                {
                    {double.PositiveInfinity, 11, double.PositiveInfinity},
                    {11, 1, 10},
                    {double.PositiveInfinity, 7, 12}
                },
                new[] {1, 0, 2},
                new[] {1, 0, 2}
            }
        };

        [Fact]
        public void SolveThrowsWhenNoFeasibleSolutionExists()
        {
            var cost = new[,] {{double.PositiveInfinity}};
            var solver = new ShortestPathSolver();
            Assert.Throws<InvalidOperationException>(() => solver.Solve(cost));
        }

        [Fact]
        public void SparseAndDenseSolversGiveSameResult()
        {
            var rng = new Random(42);
            const int numberOfRuns = 100;
            const int numRows = 400;
            const int numCols = 600;

            for (var run = 0; run < numberOfRuns; run++)
            {
                var dense = new double[numRows, numCols];
                for (var i = 0; i < numRows; i++)
                for (var j = 0; j < numCols; j++)
                    dense[i, j] = rng.NextDouble();

                var sparse = new SparseMatrixDouble(dense);

                var solver = new ShortestPathSolver();
                var denseResult = solver.Solve(dense);
                var sparseResult = solver.Solve(sparse);
                // Check that all assignments agree. In principle, they don't have to if more than
                // one optimal solution exists, but this is somewhat unlikely when we're dealing
                // with random doubles.
                for (var i = 0; i < numRows; i++)
                    Assert.Equal(denseResult.ColumnAssignment[i], sparseResult.ColumnAssignment[i]);
                for (var j = 0; j < numCols; j++)
                    Assert.Equal(denseResult.RowAssignment[j], sparseResult.RowAssignment[j]);
            }
        }
    }
}