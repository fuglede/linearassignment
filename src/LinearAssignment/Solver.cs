namespace LinearAssignment
{
    public class Solver
    {
        public static Assignment Solve(double[,] cost, bool maximize = false, bool skipPositivityTest = false) =>
            ShortestPath.Solve(cost, maximize, skipPositivityTest);

        // TODO: Align interfaces between double and int case
        public static Assignment Solve(int[,] cost) =>
            Pseudoflow.Solve(cost);
    }
}
