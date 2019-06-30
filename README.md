# Linear assignment problem in .NET
[![Build status](https://travis-ci.org/fuglede/linearassignment.svg?branch=master)](https://travis-ci.org/fuglede/linearassignment)

This repository includes a pure C# solver for the rectangular [linear assignment problem](https://en.wikipedia.org/wiki/Assignment_problem), also known as the minimum weight full matching problem for bipartite graphs.


## The problem

Concretely, the problem we solve is the following: Let *G* = (*V*, *E*) be a graph and assume that *V* is the disjoint union of two sets *X* and *Y*, with |*X*| ≤ |*Y*|, and so that for every (*i*, *j*) ∈ *E* we have *i* ∈ *X*, *j* ∈ *Y*. A full matching *M* ⊆ *E* is a subset of edges with the property that every vertex in *X* belongs to exactly one edge in *M*, and every vertex in *Y* belongs to at most one edge in *M*. Let *c* : *E* → **R** be any real function. Our goal is to find a full matching *M* minimizing Σ<sub>*e* ∈ *M*</sub> *c*(*e*).


## Method

The method we use is a modified version of the Jonker--Volgenant algorithm as described in

    DF Crouse. On implementing 2D rectangular assignment algorithms.
    IEEE Transactions on Aerospace and Electronic Systems
    52(4):1679-1696, August 2016
    doi: 10.1109/TAES.2016.140952

and ported from the C++ implementation in [`scipy.optimize`](https://docs.scipy.org/doc/scipy/reference/generated/scipy.optimize.linear_sum_assignment.html).

The algorithm is a greatly simplified version of the original algorithm as described in

    R. Jonker and A. Volgenant. A Shortest Augmenting Path Algorithm for
    Dense and Sparse Linear Assignment Problems. *Computing*, 38:325-340
    December 1987.
    
Here, most of the computational time is spent on initialization and setting up a useful solution prior to searching for shortest augmenting paths, where in the method at hand, all initialization is skipped, and we jump straight to augmentation.


## Installation

The package is available from the public [NuGet Gallery](https://www.nuget.org/packages/LinearAssignment/).


## Example

With the notation above, assume that *X* consists of 3 vertices, and that *Y* consists of 4 vertices. Then the function *c* may be described by some `double[3, 4]`, each of whose elements is a pair (*i*, *j*) of elements of *X* and *Y* respectively, in which we use `double.PositiveInfinity` to indicate that a given pair is not connected by an edge. Let us assume that such a *c* is given, and let us assume that each vertex in *X* have edges incident with each vertex in *Y*, except for, say, the last vertex in *X* not being connected to the last vertex in *Y*. Then the minimum weight matching may be obtained as follows:

```cs
var cost = new[,] {{400, 150, 400, 1}, {400, 450, 600, 2}, {300, 225, 300, double.PositiveInfinity}};
var res = JonkerVolgenant.Solve(cost).ColumnAssignment;
```

The result is `{1, 3, 2}` indicating that the three vertices of *X* are matched with the second, fourth, and third vertex in *Y* respectively (noting the zero-indexing).
