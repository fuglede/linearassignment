namespace LinearAssignment
{
    public class Solver
    {
        public static Assignment Solve(double[,] cost, ISolver solver = null)
        {
            if (solver == null) solver = new ShortestPath();
            return solver.Solve(cost);
        }

        public static Assignment Solve(int[,] cost, ISolver solver = null)
        {
            if (solver == null) solver = new Pseudoflow();
            return solver.Solve(cost);
        }
    }
}
