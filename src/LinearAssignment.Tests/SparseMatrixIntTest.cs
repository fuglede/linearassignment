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

        [Theory]
        [MemberData(nameof(TransposeData))]
        public void TransposeCalculatesTransposeAndIsAnInvolution(
            List<int> A, List<int> IA, List<int> CA, int numColumns,
            List<int> At, List<int> IAt, List<int> CAt)
        {
            var matrix = new SparseMatrixInt(A, IA, CA, numColumns);
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
                new List<int> {3, 5, 1, 2, -4},
                new List<int> {0, 2, 4, 5},
                new List<int> {1, 2, 0, 1, 0},
                3,
                // [[x, 1, -4], [3, 2, x], [5, x, x]]
                new List<int> {1, -4, 3, 2, 5},
                new List<int> {0, 2, 4, 5},
                new List<int> {1, 2, 0, 1, 0}
            },
            new object[]
            {
                // [[x, x, 5, 1], [x, 2, x, 1], [-4, x, 3, x]]
                new List<int> {5, 1, 2, 1, -4, 3},
                new List<int> {0, 2, 4, 6},
                new List<int> {2, 3, 1, 3, 0, 2},
                4,
                // [[x, x, -4], [x, 2, x], [5, x, 3], [1, 1, x]]
                new List<int> {-4, 2, 5, 3, 1, 1},
                new List<int> {0, 1, 2, 4, 6},
                new List<int> {2, 1, 0, 2, 0, 1}
            },
            new object[]
            {
                // [[x, x, x], [x, x, x]]
                new List<int>(),
                new List<int> {0, 0, 0},
                new List<int>(),
                3,
                // [[x, x], [x, x], [x, x]]
                new List<int>(),
                new List<int> {0, 0, 0, 0},
                new List<int>()
            },
            new object[]
            {
                // [[], [], []]
                new List<int>(),
                new List<int> {0, 0, 0, 0},
                new List<int>(),
                0,
                // [[]]
                new List<int>(),
                new List<int> {0},
                new List<int>()
            }
        };
    }
}
