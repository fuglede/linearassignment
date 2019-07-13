namespace LinearAssignment
{
    public interface ISolver
    {
        Assignment Solve(double[,] cost);
        Assignment Solve(int[,] cost);
    }
}