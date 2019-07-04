using System;
using System.Linq;

namespace LinearAssignment
{
    /// <summary>
    /// Solver for the linear assignment problem based on shortest augmenting paths. Concretely,
    /// we implement the pseudo-code from
    /// 
    ///     DF Crouse. On implementing 2D rectangular assignment algorithms.
    ///     IEEE Transactions on Aerospace and Electronic Systems
    ///     52(4):1679-1696, August 2016
    ///     doi: 10.1109/TAES.2016.140952
    ///
    /// which in turn is based closely on Section 14.4 of
    ///
    ///     Rainer Burkard, Mauro Dell'Amico, Silvano Martello.
    ///     Assignment Problems - Revised Reprint
    ///     Society for Industrial and Applied Mathematics, Philadelphia, 2012
    ///
    /// This is a C# port of the C++ implementation of the algorithm by Peter Mahler Larsen included
    /// in the Python library scipy.optimize. https://github.com/scipy/scipy/pull/10296/
    /// </summary>
    public static class Solver
    {
        public static Assignment Solve(double[,] cost, bool skipPositivityTest = false)
        {
            var nr = cost.GetLength(0);
            var nc = cost.GetLength(1);
            if (nr == 0 || nc == 0)
                return new Assignment(new int[] { }, new int[] { },
                    new double[] { }, new double[] { });

            // TODO: Allow matrices with nr > nc by transposing
            if (nr > nc)
                throw new ArgumentException("Cost can not have more rows than columns.");

            // Ensure that all values are positive as this is required by our search method
            var min = double.PositiveInfinity;
            if (!skipPositivityTest)
            {
                for (var i = 0; i < nr; i++)
                    for (var j = 0; j < nc; j++)
                        if (cost[i, j] < min)
                            min = cost[i, j];

                if (min < 0)
                    for (var i = 0; i < nr; i++)
                        for (var j = 0; j < nc; j++)
                            cost[i, j] -= min;
                else
                    min = 0;
            }

            // Initialize working arrays
            var u = new double[nr];
            var v = new double[nc];
            var shortestPathCosts = new double[nc];
            var path = Enumerable.Repeat(-1, nc).ToArray();
            var x = Enumerable.Repeat(-1, nr).ToArray();
            var y = Enumerable.Repeat(-1, nc).ToArray();
            var sr = new bool[nr];
            var sc = new bool[nc];

            // Find a matching one vertex at a time
            for (var curRow = 0; curRow < nr; curRow++)
            {
                double minVal = 0;
                var i = curRow;
                // Reset working arrays
                var remaining = Enumerable.Repeat(0, nc).ToList();
                var numRemaining = nc;
                for (var jp = 0; jp < nc; jp++)
                {
                    remaining[jp] = jp;
                    shortestPathCosts[jp] = double.PositiveInfinity;
                }
                Array.Clear(sr, 0, sr.Length);
                Array.Clear(sc, 0, sc.Length);

                // Start finding augmenting path
                var sink = -1;
                while (sink == -1)
                {
                    sr[i] = true;
                    var indexLowest = -1;
                    var lowest = double.PositiveInfinity;
                    for (var jk = 0; jk < numRemaining; jk++)
                    {
                        var jl = remaining[jk];
                        var r = minVal + cost[i, jl] - u[i] - v[jl];
                        if (r < shortestPathCosts[jl])
                        {
                            path[jl] = i;
                            shortestPathCosts[jl] = r;
                        }

                        if (shortestPathCosts[jl] < lowest || shortestPathCosts[jl] == lowest && y[jl] == -1)
                        {
                            lowest = shortestPathCosts[jl];
                            indexLowest = jk;
                        }
                    }

                    minVal = lowest;
                    var jp = remaining[indexLowest];
                    if (double.IsPositiveInfinity(minVal))
                        throw new InvalidOperationException("No feasible solution.");
                    if (y[jp] == -1)
                        sink = jp;
                    else
                        i = y[jp];

                    sc[jp] = true;
                    remaining[indexLowest] = remaining[--numRemaining];
                    remaining.RemoveAt(numRemaining);
                }

                if (sink < 0)
                    throw new InvalidOperationException("No feasible solution.");

                // Update dual variables
                u[curRow] += minVal;
                for (var ip = 0; ip < nr; ip++)
                    if (sr[ip] && ip != curRow)
                        u[ip] += minVal - shortestPathCosts[x[ip]];

                for (var jp = 0; jp < nc; jp++)
                    if (sc[jp])
                        v[jp] -= minVal - shortestPathCosts[jp];

                // Augment previous solution
                var j = sink;
                while (true)
                {
                    var ip = path[j];
                    y[j] = ip;
                    var tmp = j;
                    j = x[ip];
                    x[ip] = tmp;
                    if (ip == curRow)
                        break;
                }
            }

            if (!skipPositivityTest && min != 0)
                for (var ip = 0; ip < nr; ip++)
                    u[ip] += min;

            return new Assignment(x, y, u, v);
        }
    }
}