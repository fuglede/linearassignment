namespace LinearAssignment
{
    /// <summary>
    /// Represents a solution to the linear assignment problem which also includes a solution
    /// to the dual problem.
    /// </summary>
    public class AssignmentWithDuals : Assignment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssignmentWithDuals"/> class.
        /// </summary>
        public AssignmentWithDuals(int[] columnAssignment, int[] rowAssignment, double[] dualU, double[] dualV) :
            base(columnAssignment, rowAssignment)
        {
            DualU = dualU;
            DualV = dualV;
        }

        /// <summary>
        /// The potential of the rows.
        /// </summary>
        public double[] DualU { get; }

        /// <summary>
        /// The potential of the columns.
        /// </summary>
        public double[] DualV { get; }
    }
}
