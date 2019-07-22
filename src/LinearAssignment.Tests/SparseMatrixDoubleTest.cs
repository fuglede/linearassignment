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
            Assert.Equal(new List<double> { 5, 8, 3, 6 }, sparse.A);
            Assert.Equal(new List<int> { 0, 0, 2, 3, 4 }, sparse.IA);
            Assert.Equal(new List<int> { 0, 1, 2, 1 }, sparse.CA);
            Assert.Equal(4, sparse.NumRows);
            Assert.Equal(4, sparse.NumColumns);
            Assert.Equal(8, sparse.MaxValue);
        }

        [Fact]
        public void SparseMatrixConstructsProperlyWhenConstructedFromIntegralSparseMatrix()
        {
            var sparseInt = new SparseMatrixInt(
                new List<int> { 5, 8, 3, 6 },
                new List<int> { 0, 0, 2, 3, 4 },
                new List<int> { 0, 1, 2, 1 },
                4);
            var sparseDouble = new SparseMatrixDouble(sparseInt);
            Assert.Equal(new List<double> { 5, 8, 3, 6 }, sparseDouble.A);
            Assert.Equal(new List<int> { 0, 0, 2, 3, 4 }, sparseDouble.IA);
            Assert.Equal(new List<int> { 0, 1, 2, 1 }, sparseDouble.CA);
            Assert.Equal(4, sparseDouble.NumRows);
            Assert.Equal(4, sparseDouble.NumColumns);
            Assert.Equal(8, sparseDouble.MaxValue);
        }
    }
}