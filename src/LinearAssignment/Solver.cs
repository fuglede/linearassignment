using System.Collections.Generic;
using System.Data;

namespace LinearAssignment
{
    /// <summary>
    /// General solver of the linear assignment problem that provides sane choices
    /// of defaults for algorithms. This is the intended entry point for general
    /// purpose usage of the library.
    /// </summary>
    public class Solver
    {
        public static Assignment Solve(double[,] cost, bool maximize = false, ISolver solver = null)
        {
            var transpose = Transpose(ref cost);
            // We handle maximization by changing all signs in the given cost, then
            // minimizing the result. At the end of the day, we also make sure to
            // update the dual variables accordingly.
            var nr = cost.GetLength(0);
            var nc = cost.GetLength(1);
            if (maximize)
            {
                var tmpCost = new double[nr, nc];
                for (var i = 0; i < nr; i++)
                for (var j = 0; j < nc; j++)
                    tmpCost[i, j] = -cost[i, j];
                cost = tmpCost;
            }

            // Ensure that all values are positive
            var min = double.PositiveInfinity;
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

            if (solver == null) solver = new ShortestPathSolver();
            var solution = solver.Solve(cost);

            if (min != 0)
                for (var ip = 0; ip < nr; ip++)
                    solution.DualU[ip] += min;

            if (transpose)
                solution = new Assignment(solution.RowAssignment, solution.ColumnAssignment, solution.DualV, solution.DualU);

            if (maximize) FlipDualSigns(solution.DualU, solution.DualV);

            return solution;
        }

        public static Assignment Solve(int[,] cost, bool maximize = false, ISolver solver = null)
        {
            var transpose = Transpose(ref cost);

            // As in the double case, maximization is handled by flipping the sign; here, we need
            // to take special care when dealing with "infinities".
            var nr = cost.GetLength(0);
            var nc = cost.GetLength(1);
            if (maximize)
            {
                var tmpCost = new int[nr, nc];
                for (var i = 0; i < nr; i++)
                for (var j = 0; j < nc; j++)
                    tmpCost[i, j] = cost[i, j] == int.MinValue ? int.MaxValue : -cost[i, j];
                cost = tmpCost;
            }

            // Ensure that all values are positive
            var min = int.MaxValue;
            for (var i = 0; i < nr; i++)
            for (var j = 0; j < nc; j++)
                if (cost[i, j] < min)
                    min = cost[i, j];

            if (min < 0)
                for (var i = 0; i < nr; i++)
                for (var j = 0; j < nc; j++)
                    cost[i, j] = cost[i, j] == int.MaxValue ? int.MaxValue : cost[i, j] - min;
            else
                min = 0;

            if (solver == null) solver = new PseudoflowSolver();
            var solution = solver.Solve(cost);

            if (min != 0)
                for (var ip = 0; ip < solution.DualU.Length; ip++)
                    solution.DualU[ip] += min;

            if (transpose)
                solution = new Assignment(solution.RowAssignment, solution.ColumnAssignment, solution.DualV, solution.DualU);

            if (maximize) FlipDualSigns(solution.DualU, solution.DualV);
            return solution;
        }

        private static bool Transpose<T>(ref T[,] cost)
        {
            // In our solution, we will assume that nr <= nc. If this isn't the case,
            // we transpose the entire matrix and make sure to fix up the results at
            // the end of the day.
            var nr = cost.GetLength(0);
            var nc = cost.GetLength(1);
            if (nr <= nc) return false;
            var tmp = nc;
            nc = nr;
            nr = tmp;
            var tmpCost = new T[nr, nc];
            for (var i = 0; i < nr; i++)
            for (var j = 0; j < nc; j++)
                tmpCost[i, j] = cost[j, i];
            cost = tmpCost;
            return true;
        }

        private static void FlipDualSigns(IList<double> u, IList<double> v)
        {
            for (var i = 0; i < u.Count; i++) u[i] = -u[i];
            for (var j = 0; j < v.Count; j++) v[j] = -v[j];
        }
    }
}
