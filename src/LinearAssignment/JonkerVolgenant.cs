using System;
using System.Linq;

namespace LinearAssignment
{
    /// <summary>
    /// Solver for the linear assignment problem based on the Jonker--Volgenant algorithm:
    /// 
    ///     R. Jonker and A. Volgenant. A Shortest Augmenting Path Algorithm for
    ///     Dense and Sparse Linear Assignment Problems. *Computing*, 38:325-340
    ///     December 1987.
    ///
    /// This particular implementation is based on a simplified version of the algorithm
    /// described in
    /// 
    ///     DF Crouse. On implementing 2D rectangular assignment algorithms.
    ///     IEEE Transactions on Aerospace and Electronic Systems
    ///     52(4):1679-1696, August 2016
    ///     doi: 10.1109/TAES.2016.140952
    ///
    /// Concretely, this is a C# port of the C++ implementation of the algorithm by Peter
    /// Mahler Larsen included in the Python library scipy.optimize.
    /// </summary>
    public static class JonkerVolgenant
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

            // TODO: Allow negative costs by shifting all values
            if (!skipPositivityTest)
                for (int i = 0; i < nr; i++)
                    for (int j = 0; j < nc; j++)
                        if (cost[i, j] < 0)
                            throw new ArgumentException("All costs must be non-negative", nameof(cost));

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
                    sc[jp] = true;
                    if (double.IsPositiveInfinity(minVal))
                        throw new InvalidOperationException("No feasible solution.");
                    if (y[jp] == -1)
                        sink = jp;
                    else
                        i = y[jp];
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

            return new Assignment(x, y, u, v);
        }
    }
}