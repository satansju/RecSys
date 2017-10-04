/*
 * K-Means clustering
 * index of return is tuple ID, cell is cluster 
 * ex: [2 1 0 0 2 2] means tuple 0 is cluster 2, tuple 1 is cluster 1, tuple 2 is cluster 0, tuple 3 is cluster 0, etc.
 * an alternative clustering DS to save space is to use the .NET BitArray class
 */

using System;
namespace P2_recsys {
  public class Cluster {
    bool changed = true; // was there a change in at least one cluster assignment?
    bool success = true; // were all means able to be computed? (no zero-count clusters)

    int numClusters = 2;

    public double[][] data;
    public double[][] rawData;
    public double[][] means;

    public int[] clustering;

    public Cluster(double[][] rawData, int numClusters) {
      this.rawData = rawData;
      this.numClusters = numClusters;
      data = Normalize(rawData); // so large values don't dominate

      Random random = new Random();
      clustering = new int[data.Length];

      for (int i = 0; i < numClusters; ++i) // make sure each cluster has at least one tuple
        clustering[i] = i;
      for (int i = numClusters; i < clustering.Length; ++i)
        clustering[i] = random.Next(0, numClusters); // other assignments random

      means = Allocate(data[0].Length); // small convenience
    }

    public Cluster Update() {
      int maxCount = data.Length * 10;
      int ct = 0;
      while (changed == true && success == true && ct < maxCount) {
        ++ct;
        success = UpdateMeans();
        changed = UpdateClustering();
      }
      return this;
    }

    double[][] Normalize(double[][] rawData) {
      // normalize raw data by computing (x - mean) / stddev
      // primary alternative is min-max:
      // v' = (v - min) / (max - min)

      // make a copy of input data
      double[][] result = new double[rawData.Length][];
      for (int i = 0; i < rawData.Length; ++i) {
        result[i] = new double[rawData[i].Length];
        Array.Copy(rawData[i], result[i], rawData[i].Length);
      }

      for (int j = 0; j < result[0].Length; ++j) // each col
      {
        double colSum = 0.0;
        for (int i = 0; i < result.Length; ++i)
          colSum += result[i][j];
        double mean = colSum / result.Length;
        double sum = 0.0;
        for (int i = 0; i < result.Length; ++i)
          sum += (result[i][j] - mean) * (result[i][j] - mean);
        double sd = sum / result.Length;
        for (int i = 0; i < result.Length; ++i)
          result[i][j] = (result[i][j] - mean) / sd;
      }
      return result;
    }

    double[][] Allocate(int numColumns) {
      // convenience matrix allocator for Cluster()
      double[][] result = new double[numClusters][];
      for (int k = 0; k < numClusters; ++k)
        result[k] = new double[numColumns];
      return result;
    }

    bool UpdateMeans() {
      // returns false if there is a cluster that has no tuples assigned to it
      // parameter means[][] is really a ref parameter

      // check existing cluster counts
      // can omit this check if InitClustering and UpdateClustering
      // both guarantee at least one tuple in each cluster (usually true)
      int numClust = means.Length;
      int[] clusterCounts = new int[numClust];
      for (int i = 0; i < data.Length; ++i) {
        int cluster = clustering[i];
        ++clusterCounts[cluster];
      }

      for (int k = 0; k < numClust; ++k)
        if (clusterCounts[k] == 0)
          return false; // bad clustering. no change to means[][]

      // update, zero-out means so it can be used as scratch matrix 
      for (int k = 0; k < means.Length; ++k)
        for (int j = 0; j < means[k].Length; ++j)
          means[k][j] = 0.0;

      for (int i = 0; i < data.Length; ++i) {
        int cluster = clustering[i];
        for (int j = 0; j < data[i].Length; ++j)
          means[cluster][j] += data[i][j]; // accumulate sum
      }

      for (int k = 0; k < means.Length; ++k)
        for (int j = 0; j < means[k].Length; ++j)
          means[k][j] /= clusterCounts[k]; // danger of div by 0
      return true;
    }

    bool UpdateClustering() {
      // (re)assign each tuple to a cluster (closest mean)
      // returns false if no tuple assignments change OR
      // if the reassignment would result in a clustering where
      // one or more clusters have no tuples.

      int numClust = means.Length;
      changed = false;

      int[] newClustering = new int[clustering.Length]; // proposed result
      Array.Copy(clustering, newClustering, clustering.Length);

      double[] distances = new double[numClust]; // distances from curr tuple to each mean

      for (int i = 0; i < data.Length; ++i) // walk thru each tuple
      {
        for (int k = 0; k < numClust; ++k)
          distances[k] = Distance(data[i], means[k]); // compute distances from curr tuple to all k means

        int newClusterID = MinIndex(distances); // find closest mean ID
        if (newClusterID != newClustering[i]) {
          changed = true;
          newClustering[i] = newClusterID; // update
        }
      }

      if (changed == false)
        return false; // no change so bail and don't update clustering[][]

      // check proposed clustering[] cluster counts
      int[] clusterCounts = new int[numClusters];
      for (int i = 0; i < data.Length; ++i) {
        int cluster = newClustering[i];
        ++clusterCounts[cluster];
      }

      for (int k = 0; k < numClusters; ++k)
        if (clusterCounts[k] == 0)
          return false; // bad clustering. no change to clustering[][]

      Array.Copy(newClustering, clustering, newClustering.Length); // update
      return true; // good clustering and at least one change
    }

    double Distance(double[] tuple, double[] mean) {
      // Euclidean distance between two vectors for UpdateClustering()
      // consider alternatives such as Manhattan distance
      double sumSquaredDiffs = 0.0;
      for (int j = 0; j < tuple.Length; ++j)
        sumSquaredDiffs += Math.Pow((tuple[j] - mean[j]), 2);
      return Math.Sqrt(sumSquaredDiffs);
    }

    int MinIndex(double[] distances) {
      // index of smallest value in array
      // helper for UpdateClustering()
      int indexOfMin = 0;
      double smallDist = distances[0];
      for (int k = 0; k < distances.Length; ++k) {
        if (distances[k] < smallDist) {
          smallDist = distances[k];
          indexOfMin = k;
        }
      }
      return indexOfMin;
    }

    #region Debugging Methods
    public void ShowData(int decimals, bool indices, bool newLine) {
      for (int i = 0; i < data.Length; ++i) {
        if (indices) Console.Write(i.ToString().PadLeft(3) + " ");
        for (int j = 0; j < data[i].Length; ++j) {
          if (data[i][j] >= 0.0) Console.Write(" ");
          Console.Write(data[i][j].ToString("F" + decimals) + " ");
        }
        Console.WriteLine("");
      }
      if (newLine) Console.WriteLine("");
    }

    public void ShowVector() {
      for (int i = 0; i < clustering.Length; ++i)
        Console.Write(clustering[i] + " ");
        Console.WriteLine("\n");
    }

    public void ShowClustered(int decimals) {
      for (int k = 0; k < numClusters; ++k) {
        Console.WriteLine("===================");
        for (int i = 0; i < data.Length; ++i) {
          int clusterID = clustering[i];
          if (clusterID != k) continue;
          Console.Write(i.ToString().PadLeft(3) + " ");
          for (int j = 0; j < rawData[i].Length; ++j) {
            if (rawData[i][j] >= 0.0) Console.Write(" ");
            Console.Write(data[i][j].ToString("F" + decimals) + " ");
          }
          Console.WriteLine("");
        }
        Console.WriteLine("===================");
      }
    }
    #endregion
  }
}
