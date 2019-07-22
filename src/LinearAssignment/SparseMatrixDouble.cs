using System.Collections.Generic;
using System.Linq;

namespace LinearAssignment
{
    /// <summary>
    /// Represents a sparse matrix in compressed sparse row (CSR) format whose elements
    /// are doubles.
    /// </summary>
    public class SparseMatrixDouble
    {
        public SparseMatrixDouble(double[,] dense, double empty = double.PositiveInfinity)
        {
            A = new List<double>();
            IA = new List<int>();
            CA = new List<int>();
            IA.Add(0);
            int nonInfinite = 0;
            var nr = dense.GetLength(0);
            var nc = dense.GetLength(1);
            for (int i = 0; i < nr; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    double entry = dense[i, j];
                    if (entry != empty)
                    {
                        nonInfinite++;
                        A.Add(entry);
                        CA.Add(j);
                        if (entry > _max)
                            _max = entry;
                    }
                }
                IA.Add(nonInfinite);
            }

            NumRows = nr;
            NumColumns = nc;
        }

        private double _max = double.PositiveInfinity;
        public double MaxValue => _max != double.PositiveInfinity ? _max : _max = A.Max();
        public List<double> A { get; }
        public List<int> IA { get; }
        public List<int> CA { get; }

        public int NumRows { get; }
        public int NumColumns { get; }
    }
}
