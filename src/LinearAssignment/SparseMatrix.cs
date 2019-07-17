using System;
using System.Collections.Generic;
using System.Linq;

namespace LinearAssignment
{
    /// <summary>
    /// Represents a sparse matrix in compressed sparse row (CSR) format.
    /// </summary>
    public class SparseMatrix
    {
        public SparseMatrix(int[,] dense, int empty = int.MaxValue)
        {
            A = new List<int>();
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
                    int entry = dense[i, j];
                    if (!entry.Equals(empty))
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

        private int _max = int.MinValue;
        public int MaxValue => _max != int.MaxValue ? _max : _max = A.Max();
        public List<int> A { get; }
        public List<int> IA { get; }
        public List<int> CA { get; }

        public int NumRows { get; }
        public int NumColumns { get; }
    }
}
