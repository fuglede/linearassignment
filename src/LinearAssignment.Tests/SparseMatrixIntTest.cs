using System.Collections.Generic;
using Xunit;

namespace LinearAssignment.Tests
{
    public class SparseMatrixIntTest
    {
        [Fact]
        public void SparseMatrixConstructsProperlyForDenseIntegralInput()
        {
            int[,] dense = {{0, 0, 0, 0}, {5, 8, 0, 0}, {0, 0, 3, 0}, {0, 6, 0, 0}};
            var sparse = new SparseMatrixInt(dense, 0);
            Assert.Equal(new List<int> {5, 8, 3, 6}, sparse.A);
            Assert.Equal(new List<int> {0, 0, 2, 3, 4}, sparse.IA);
            Assert.Equal(new List<int> {0, 1, 2, 1}, sparse.CA);
            Assert.Equal(4, sparse.NumRows);
            Assert.Equal(4, sparse.NumColumns);
            Assert.Equal(8, sparse.MaxValue);
        }
    }
}
