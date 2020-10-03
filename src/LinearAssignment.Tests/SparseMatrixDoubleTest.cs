using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LinearAssignment.Tests
{
    public class SparseMatrixDoubleTest
    {
        [Fact]
        public void SparseMatrixConstructsProperlyForDenseIntegralInput()
        {
            double[,] dense = {{0, 0, 0, 0}, {5, 8, 0, 0}, {0, 0, 3, 0}, {0, 6, 0, 0}};
            var sparse = new SparseMatrixDouble(dense, 0);
            Assert.Equal(new List<double> {5, 8, 3, 6}, sparse.A);
            Assert.Equal(new List<int> {0, 0, 2, 3, 4}, sparse.IA);
            Assert.Equal(new List<int> {0, 1, 2, 1}, sparse.CA);
            Assert.Equal(4, sparse.NumRows);
            Assert.Equal(4, sparse.NumColumns);
            Assert.Equal(8, sparse.MaxValue);
        }

        [Fact]
        public void SparseMatrixConstructsProperlyWhenConstructedFromIntegralSparseMatrix()
        {
            var sparseInt = new SparseMatrixInt(
                new List<int> {5, 8, 3, 6},
                new List<int> {0, 0, 2, 3, 4},
                new List<int> {0, 1, 2, 1},
                4);
            var sparseDouble = new SparseMatrixDouble(sparseInt);
            Assert.Equal(new List<double> {5, 8, 3, 6}, sparseDouble.A);
            Assert.Equal(new List<int> {0, 0, 2, 3, 4}, sparseDouble.IA);
            Assert.Equal(new List<int> {0, 1, 2, 1}, sparseDouble.CA);
            Assert.Equal(4, sparseDouble.NumRows);
            Assert.Equal(4, sparseDouble.NumColumns);
            Assert.Equal(8, sparseDouble.MaxValue);
        }

        [Theory]
        [MemberData(nameof(TransposeData))]
        public void TransposeCalculatesTransposeAndIsAnInvolution(
            List<double> A, List<int> IA, List<int> CA, int numColumns,
            List<double> At, List<int> IAt, List<int> CAt)
        {
            var matrix = new SparseMatrixDouble(A.ToList(), IA.ToList(), CA.ToList(), numColumns);
            var transpose = matrix.Transpose();
            var doubleTranspose = transpose.Transpose();

            Assert.Equal(At, transpose.A);
            Assert.Equal(IAt, transpose.IA);
            Assert.Equal(CAt, transpose.CA);

            Assert.Equal(A, doubleTranspose.A);
            Assert.Equal(IA, doubleTranspose.IA);
            Assert.Equal(CA, doubleTranspose.CA);
        }

        public static IEnumerable<object[]> TransposeData => new[]
        {
            new object[]
            {
                // [[x, 3, 5], [1, 2, x], [-4, x, x]]
                new List<double> {3, 5, 1, 2, -4},
                new List<int> {0, 2, 4, 5},
                new List<int> {1, 2, 0, 1, 0},
                3,
                // [[x, 1, -4], [3, 2, x], [5, x, x]]
                new List<double> {1, -4, 3, 2, 5},
                new List<int> {0, 2, 4, 5},
                new List<int> {1, 2, 0, 1, 0}
            },
            new object[]
            {
                // [[x, x, 5, 1], [x, 2, x, 1], [-4, x, 3, x]]
                new List<double> {5, 1, 2, 1, -4, 3},
                new List<int> {0, 2, 4, 6},
                new List<int> {2, 3, 1, 3, 0, 2},
                4,
                // [[x, x, -4], [x, 2, x], [5, x, 3], [1, 1, x]]
                new List<double> {-4, 2, 5, 3, 1, 1},
                new List<int> {0, 1, 2, 4, 6},
                new List<int> {2, 1, 0, 2, 0, 1}
            },
            new object[]
            {
                // [[x, x, x], [x, x, x]]
                new List<double>(),
                new List<int> {0, 0, 0},
                new List<int>(),
                3,
                // [[x, x], [x, x], [x, x]]
                new List<double>(),
                new List<int> {0, 0, 0, 0},
                new List<int>()
            },
            new object[]
            {
                // [[], [], []]
                new List<double>(),
                new List<int> {0, 0, 0, 0},
                new List<int>(),
                0,
                // [[]]
                new List<double>(),
                new List<int> {0},
                new List<int>()
            }
        };
    }
}