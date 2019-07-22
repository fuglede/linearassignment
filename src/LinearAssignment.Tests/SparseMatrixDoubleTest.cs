using System.Collections.Generic;
using Xunit;

namespace LinearAssignment.Tests
{
    public class SparseMatrixDoubleTest
    {
        [Fact]
        public void SparseMatrixConstructsProperlyForDenseIntegralInput()
        {
            double[,] dense = { { 0, 0, 0, 0 }, { 5, 8, 0, 0 }, { 0, 0, 3, 0 }, { 0, 6, 0, 0 } };
            var sparse = new SparseMatrixDouble(dense, 0);
            Assert.Equal(sparse.A, new List<double> { 5, 8, 3, 6 });
            Assert.Equal(sparse.IA, new List<int> { 0, 0, 2, 3, 4 });
            Assert.Equal(sparse.CA, new List<int> { 0, 1, 2, 1 });
        }
    }
}