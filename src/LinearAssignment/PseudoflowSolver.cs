using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAssignment
{
    /// <summary>
    /// Solver for the linear assignment problem based on the pseudoflow approach to solving
    /// minimum cost flow problems. This is closely based on Section 4.6.4 of
    ///
    ///     Rainer Burkard, Mauro Dell'Amico, Silvano Martello.
    ///     Assignment Problems - Revised Reprint
    ///     Society for Industrial and Applied Mathematics, Philadelphia, 2012
    ///
    /// which in turn is based on the cost-scaling assignment (CSA) approach of
    ///
    ///     A.V. Goldberg and R. Kennedy.
    ///     An efficient cost scaling algorithm for the assignment problem.
    ///     Math. Program., 71:153–177, 1995
    ///
    /// in which the push-relabel step is performed using the "double push" algorithm. We adopt
    /// a few minor changes to the pseudo-code of Burkard--Dell'Amico--Martello in the interest
    /// of improving performance. In particular, we index row and column assignments by the vertices.
    ///
    /// Note that no attempt is made to detect whether feasible solutions exist. As such,
    /// the solver may run forever if no feasible solutions exist.
    /// </summary>
    public class PseudoflowSolver : ISolver
    {
        private readonly double _alpha;

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoflowSolver"/> class.
        /// </summary>
        /// <param name="alpha">The cost-scaling reduction factor.</param>
        public PseudoflowSolver(double alpha = 10) =>
            _alpha = alpha;

        public Assignment Solve(double[,] cost) =>
            throw new NotImplementedException("The pseudoflow solver can only be used with integer costs");

        public Assignment Solve(int[,] cost)
        {
            // TODO: Allow maximization
            // TODO: Allow rectangular inputs
            var nr = cost.GetLength(0);
            var nc = cost.GetLength(1);
            if (nr == 0 || nc == 0)
                return new Assignment(new int[] { }, new int[] { },
                    new double[] { }, new double[] { });
            if (nr != nc)
                throw new NotImplementedException("Pseudoflow is only implemented for square matrices");
            var n = nr;

            // To simplify our double-push, first eliminate rows with only a single incident edge
            var skippedAssignments = new Dictionary<int, int>();
            var assignableRows = Enumerable.Range(0, n).ToList();
            var assignableColumns = Enumerable.Range(0, n).ToList();
            trim:
            for (var i = 0; i < n; i++)
            {
                var hasEdge = false;
                var hasMultipleEdges = false;
                var column = -1;
                for (var j = 0; j < n && !hasMultipleEdges; j++)
                {
                    if (cost[i, j] != int.MaxValue)
                    {
                        hasMultipleEdges = hasEdge;
                        hasEdge = true;
                        column = j;
                    }
                }
                if (!hasEdge)
                    throw new InvalidOperationException("No feasible solution exists.");
                if (!hasMultipleEdges)
                {
                    skippedAssignments[assignableRows[i]] = assignableColumns[column];
                    n--;
                    assignableRows.Remove(assignableRows[i]);
                    assignableColumns.Remove(assignableColumns[column]);
                    cost = TrimArray(i, column, cost);
                    goto trim;
                }
            }

            // Initialize epsilon to be the largest cost
            var epsilon = double.NegativeInfinity;
            for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                    if (cost[i, j] > epsilon && cost[i, j] != int.MaxValue)
                        epsilon = cost[i, j];

            // Initialize assignment variable x and the dual variables
            var u = new double[n];
            var v = new double[n];
            var col = new int[n];
            var row = new int[n];
            while (epsilon >= 1d / n)
            {
                epsilon /= _alpha;
                for (var i = 0; i < n; i++) col[i] = -1;
                for (var j = 0; j < n; j++) row[j] = -1;
                var unassigned = new Stack<int>(Enumerable.Range(1, n - 1).Reverse());
                var k = 0;
                for (var i = 0; i < n; i++)
                {
                    for (var j = 0; j < n; j++)
                    {
                        var c = cost[i, j] - v[j];
                        if (c < u[i])
                            u[i] = c;
                    }
                }

                while (true)
                {
                    if (!DoublePush(col, row, cost, ref k, epsilon, u, v, unassigned))
                        break;
                }
            }

            // Re-add the assignments we fixed in the beginning.
            var columnAssignment = new int[n + skippedAssignments.Count];
            var rowAssignment = new int[n + skippedAssignments.Count];
            foreach (var preassigned in skippedAssignments)
            {
                columnAssignment[preassigned.Key] = preassigned.Value;
                rowAssignment[preassigned.Value] = preassigned.Key;
            }

            for (var i = 0; i < n; i++)
            {
                columnAssignment[assignableRows[i]] = assignableColumns[col[i]];
                rowAssignment[assignableColumns[col[i]]] = assignableRows[i];
            }
            return new Assignment(columnAssignment, rowAssignment, u, v);
        }

        private static int[,] TrimArray(int rowToRemove, int columnToRemove, int[,] originalArray)
        {
            var result = new int[originalArray.GetLength(0) - 1, originalArray.GetLength(1) - 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }

        private static bool DoublePush(
            int[] col, int[] row, int[,] cost, ref int k, double epsilon, double[] u, double[] v, Stack<int> unassigned)
        {
            var n = u.Length;
            var smallest = double.PositiveInfinity;
            var j = -1;
            var secondSmallest = double.PositiveInfinity;
            var z = -1;
            var uk = u[k];
            for (var jp = 0; jp < n; jp++)
            {
                var reducedCost = cost[k, jp] - uk - v[jp];
                if (reducedCost <= smallest)
                {
                    secondSmallest = smallest;
                    smallest = reducedCost;
                    z = j;
                    j = jp;
                }
                else if (reducedCost <= secondSmallest)
                {
                    secondSmallest = reducedCost;
                    z = jp;
                }
            }

            col[k] = j;
            // TODO: Detect infeasibility by investigating dual updates.
            u[k] = cost[k, z] - v[z];

            if (row[j] != -1)
            {
                var i = row[j];
                row[j] = k;
                v[j] = cost[k, j] - u[k] - epsilon;
                col[i] = -1;
                k = i;
                return true;
            }

            row[j] = k;
            if (unassigned.Count == 0) return false;
            k = unassigned.Pop();
            return true;
        }
    }
}
