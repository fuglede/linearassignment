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
        private readonly double? _initialEpsilon;

        /// <summary>
        /// Initializes a new instance of the <see cref="PseudoflowSolver"/> class.
        /// </summary>
        /// <param name="alpha">The cost-scaling reduction factor.</param>
        /// <param name="initialEpsilon">Initial cost-scaling. If undefined, this will be
        /// calculated to be the largest cost. Set this if you know the ballpark magnitude
        /// of the costs to avoid having to determine the largest cost.</param>
        public PseudoflowSolver(double alpha = 10, double? initialEpsilon = null)
        {
            _alpha = alpha;
            _initialEpsilon = initialEpsilon;
        }

        public Assignment Solve(double[,] cost) =>
            throw new NotImplementedException("The pseudoflow solver can only be used with integer costs");

        public Assignment Solve(int[,] cost)
        {
            // TODO: Allow rectangular inputs
            var nr = cost.GetLength(0);
            var nc = cost.GetLength(1);
            if (nr != nc)
                throw new NotImplementedException("Pseudoflow is only implemented for square matrices");
            var n = nr;

            // To simplify our double-push, first eliminate rows with only a single incident edge.
            // These will then be re-added when we construct our result at the end of the day.
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

            // Initialize cost-scaling to be the configured value if given, and
            // otherwise let it be the largest given cost.
            double epsilon;
            if (_initialEpsilon.HasValue) epsilon = _initialEpsilon.Value;
            else
            {
                epsilon = double.NegativeInfinity;
                for (var i = 0; i < n; i++)
                for (var j = 0; j < n; j++)
                    if (cost[i, j] > epsilon && cost[i, j] != int.MaxValue)
                        epsilon = cost[i, j];
            }

            // Initialize dual variables and assignment variables keeping track of
            // assignments as we move along: col maps a given row to the column
            // it's assigned to, and conversely row maps a given column to its
            // assigned row.
            var u = new double[n];
            var v = new double[n];
            var col = new int[n];
            var row = new int[n];
            while (epsilon >= 1d / n)
            {
                epsilon /= _alpha;
                // A value in -1 in row and col corresponds to "unassigned"
                for (var i = 0; i < n; i++) col[i] = -1;
                for (var j = 0; j < n; j++) row[j] = -1;
                // We also maintain a stack of rows that have not been assigned. We
                // could get this information from the variable col, but being able to
                // just pop the stack to get new unassigned rows is much faster. We put
                // lower numbers at the top of the stack simply because that feels a bit
                // more natural; we could get rid of the Reverse if we wanted to.
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
                    // Perform double-push. The halting condition is that all rows
                    // have been assigned, which corresponds to the stack of unassigned
                    // rows having been emptied.
                    var smallest = double.PositiveInfinity;
                    var j = -1;
                    var secondSmallest = double.PositiveInfinity;
                    var z = -1;
                    var uk = u[k];
                    for (var jp = 0; jp < n; jp++)
                    {
                        var reducedCost = cost[k, jp] - uk - v[jp];
                        if (reducedCost <= secondSmallest)
                        {
                            if (reducedCost <= smallest)
                            {
                                secondSmallest = smallest;
                                smallest = reducedCost;
                                z = j;
                                j = jp;
                            }
                            else
                            {
                                secondSmallest = reducedCost;
                                z = jp;
                            }
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
                    }
                    else
                    {
                        row[j] = k;
                        if (unassigned.Count == 0) break;
                        k = unassigned.Pop();
                    }

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
            return new Assignment(columnAssignment, rowAssignment);
        }

        /// <summary>
        /// Remove a row and a column from an 2D array.
        /// </summary>
        /// <param name="rowToRemove">The index of the row to remove.</param>
        /// <param name="columnToRemove">The index of the column to remove.</param>
        /// <param name="array">The array from which the row and column should be removed.</param>
        /// <returns>A 2D array with the row and column removed, whose number of elements is one
        /// smaller than the input array in each dimension.</returns>
        private static int[,] TrimArray(int rowToRemove, int columnToRemove, int[,] array)
        {
            var result = new int[array.GetLength(0) - 1, array.GetLength(1) - 1];

            for (int i = 0, j = 0; i < array.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < array.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = array[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }
    }
}
