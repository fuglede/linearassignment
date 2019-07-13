namespace LinearAssignment
{
    /// <summary>
    /// General solver of the linear assignment problem that provides sane choices
    /// of defaults for algorithms. This is the intended entry point for general
    /// purpose usage of the library.
    /// </summary>
    public class Solver
    {
        public static Assignment Solve(double[,] cost, ISolver solver = null)
        {
            if (solver == null) solver = new ShortestPathSolver();
            return solver.Solve(cost);
        }

        public static Assignment Solve(int[,] cost, ISolver solver = null)
        {
            if (solver == null) solver = new PseudoflowSolver();
            return solver.Solve(cost);
        }
    }
}
