namespace LinearAssignment
{
    /// <summary>
    /// Represents a solution to the linear assignment problem.
    /// </summary>
    public readonly ref struct Assignment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Assignment"/> struct.
        /// </summary>
        public Assignment(int[] columnAssignment, int[] rowAssignment, double[] dualU, double[] dualV)
        {
            ColumnAssignment = columnAssignment;
            RowAssignment = rowAssignment;
            DualU = dualU;
            DualV = dualV;
        }

        /// <summary>
        /// The collection of columns assigned to each row. That is, if this
        /// is {0, 3, 2}, that means that the three rows of a given problem
        /// have been assigned to the first, fourth and third column respectively.
        /// </summary>
        public int[] ColumnAssignment { get; }

        /// <summary>
        /// The collection of rows assigned to each column. That is, if this
        /// is {2, 0, 1}, that means that the three columns of a given problem
        /// have been assigned to the third, first and second row respectively.
        /// In the case of non-square costs, a value of -1 indicates that no
        /// row has been assigned to the column.
        /// </summary>
        public int[] RowAssignment { get; }

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
