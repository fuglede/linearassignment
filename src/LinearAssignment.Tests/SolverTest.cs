﻿using LinearAssignment;
using System;
using System.Collections.Generic;
using Xunit;

namespace LinearAssignment.Tests
{
    public class SolverTest
    {
        [Theory]
        [MemberData(nameof(TestDataMinimize))]
        public void SolveGivesExpectedResultWhenMinimizing(
            double[,] cost,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment,
            double[] expectedDualU,
            double[] expectedDualV)
        {
            var solution = Solver.Solve(cost);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
            Assert.Equal(expectedDualU, solution.DualU);
            Assert.Equal(expectedDualV, solution.DualV);
        }

        /// <summary>
        /// Include tests from the Python library scipy.optimize.
        /// </summary>
        public static IEnumerable<object[]> TestDataMinimize => new[]
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
                new double[,] {{6, 6, 4, 7}, {5, 4, -3, -3}, {5, 3, 0, 6}},
                new[] {1, 3, 2},
                new[] {-1, 0, 2, 1},
                new double[] {6, -3, 2},
                new double[] {0, 0, -2, 0}
            },
            new object[]
            {
                new double[,] {{6, 5, 5}, {6, 4, 3}, {4, -3, 0}, {7, -3 , 6}},
                new[] {-1, 0, 2, 1},
                new[] {1, 3, 2},
                new double[] {0, 0, -2, 0},
                new double[] {6, -3, 2}
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
        [MemberData(nameof(TestDataMaximize))]
        public void SolveGivesExpectedResultWhenMaximizing(
            double[,] cost,
            int[] expectedColumnAssignment,
            int[] expectedRowAssignment,
            double[] expectedDualU,
            double[] expectedDualV)
        {
            var solution = Solver.Solve(cost, true);
            Assert.Equal(expectedColumnAssignment, solution.ColumnAssignment);
            Assert.Equal(expectedRowAssignment, solution.RowAssignment);
            Assert.Equal(expectedDualU, solution.DualU);
            Assert.Equal(expectedDualV, solution.DualV);
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

        [Fact]
        public void SolveThrowsWhenNoFeasibleSolutionExists()
        {
            var cost = new[,] {{double.PositiveInfinity}};
            Assert.Throws<InvalidOperationException>(() => Solver.Solve(cost));
        }
    }
}