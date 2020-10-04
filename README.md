# Linear assignment problem solver in .NET

[![Build status](https://travis-ci.org/fuglede/linearassignment.svg?branch=master)](https://travis-ci.org/fuglede/linearassignment)
[![.NET Core](https://github.com/fuglede/linearassignment/workflows/.NET%20Core/badge.svg)](https://github.com/fuglede/linearassignment/actions)


This repository includes a pure C# solver for the rectangular [linear assignment problem](https://en.wikipedia.org/wiki/Assignment_problem), also known as the [minimum weight full matching](https://en.wikipedia.org/wiki/Maximum_weight_matching) for [bipartite graphs](https://en.wikipedia.org/wiki/Bipartite_graph).

## The problem

Concretely, the problem we solve is the following: Let *G* = (*V*, *E*) be a [graph](https://en.wikipedia.org/wiki/Graph_(discrete_mathematics)) and assume that *V* is the disjoint union of two sets *X* and *Y*, with |*X*| ≤ |*Y*|, and that for every (*i*, *j*) ∈ *E* we have *i* ∈ *X*, *j* ∈ *Y*. A full matching *M* ⊆ *E* is a subset of edges with the property that every vertex in *X* is incident to exactly one edge in *M*, and every vertex in *Y* is incident to at most one edge in *M*. Let *c* : *E* → **R** be any real function. Our goal is to find a full matching *M* minimizing Σ<sub>*e* ∈ *M*</sub> *c*(*e*).

## Method

We provide a few different methods for solving the problem. One is based on shortest augmenting paths: At each step of the algorithm, the matching *M* is expanded by using Dijkstra's algorithm to find the shortest augmenting path from a given non-matched element of *X* to a non-matched element of *Y*, and the weights of the graph are updated according to a primal-dual method. We follow the pseudo-code laid out in

    DF Crouse. On implementing 2D rectangular assignment algorithms.
    IEEE Transactions on Aerospace and Electronic Systems
    52(4):1679-1696, August 2016
    doi: 10.1109/TAES.2016.140952

which in turn is based closely on Section 4.4 of

    Rainer Burkard, Mauro Dell'Amico, Silvano Martello.
    Assignment Problems - Revised Reprint
    Society for Industrial and Applied Mathematics, Philadelphia, 2012

Our algorithm for this method is ported from the C++ implementation in [`scipy.optimize`](https://docs.scipy.org/doc/scipy/reference/generated/scipy.optimize.linear_sum_assignment.html).

For sparse inputs, i.e. inputs for which |*E*| is less than |*X*| |*Y*|, we include an implementation of LAPJVsp, which is also based on shortest augmenting paths. This algorithm is described in

    R. Jonker and A. Volgenant. A Shortest Augmenting Path Algorithm for
    Dense and Sparse Linear Assignment Problems. *Computing*, 38:325-340
    December 1987.

and our implementation is a port of the Pascal code [available here](http://www.assignmentproblems.com/LAPJV.htm).

We also include a different method based on the pseudoflow approach to solving minimum cost flow problems. This is closely based on Section 4.6.4 of Assignment Problems, which in turn is based on the cost-scaling assignment (CSA) approach of

    A.V. Goldberg and R. Kennedy.
    An efficient cost scaling algorithm for the assignment problem.
    Math. Program., 71:153–177, 1995

## Installation

The package is available from the public [NuGet Gallery](https://www.nuget.org/packages/LinearAssignment/).

## Example

With the notation above, assume that *X* consists of 3 vertices, and that *Y* consists of 4 vertices. Then the function *c* may be described by some `double[3, 4]` (or `int[3, 4]` if all weights are integral), each of whose elements is a pair (*i*, *j*) of elements of *X* and *Y* respectively, in which we use `double.PositiveInfinity` to indicate that a given pair is not connected by an edge. Let us assume that such a *c* is given, and let us assume that each vertex in *X* have edges incident with each vertex in *Y*, except for, say, the last vertex in *X* not being connected to the last vertex in *Y*. Then the minimum weight matching may be obtained as follows:

```cs
var cost = new[,] {{400, 150, 400, 1}, {400, 450, 600, 2}, {300, 225, 300, double.PositiveInfinity}};
var res = Solver.Solve(cost).ColumnAssignment;
```

The result is `{1, 3, 2}` indicating that the three vertices of *X* are matched with the second, fourth, and third vertex in *Y* respectively (noting the zero-indexing).

In addition to being able to use `double.PositiveInfinity` to represent missing edges, it is possible to provide the input cost matrix in [compressed sparse row](https://en.wikipedia.org/wiki/Sparse_matrix#Compressed_sparse_row_(CSR,_CRS_or_Yale_format)) format; depending on the problem structure, this can lead to large performance improvements for the solvers when inputs are sparse.

```cs
// Represent [[2.5, missing, -1], [1, 2, missing]]
var cost = new SparseMatrixDouble(
    new List<double> {2.5, -1, 1, 2},
    new List<int> {0, 2, 4},
    new List<int> {0, 2, 0, 1},
    3);
// Or if the input is already densely represented:
// var dense = new[,] {{2.5, double.PositiveInfinity, -1}, {1, 2, double.PositiveInfinity}};
// var cost = new SparseMatrixDouble(dense);
Solver.Solve(cost);
```
