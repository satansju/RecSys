using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace P2_recsys {
  public class MatrixGenerator {
    public Matrix<double> Matrix { get; private set; }
    public double density { get; private set; }

    public MatrixGenerator(int m, int n, double density) {
      this.density = density;

      Random rnd = new Random();

      var elements = new List<Tuple<int, int, double>>();

      for (int i = 0; i < m; i++) {
        for (int j = 0; j < n; j++) {
          elements.Add(new Tuple<int, int, double>(i, j, GenerateRating(rnd)));
        }
      }

      Matrix = Matrix<double>.Build.DenseOfIndexed(m, n, elements);
    }

    double GenerateRating(Random rnd) {
      return (rnd.Next(0, 1000) < (1000 * (1 - density)) ? 0 : rnd.Next(1, 6)); // Generate random rating 0 - 5
    }
  }
}
