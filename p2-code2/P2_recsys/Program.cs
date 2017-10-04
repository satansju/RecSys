using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Data.SqlClient;

namespace P2_recsys {
  class Program {
    static void Main(string[] args) {

      // Connect to Database
      // ConnectDatabase();
      int columns, rows;
      if (args.Length >= 2) {
        columns = Int32.Parse(args[0]);
        rows = Int32.Parse(args[1]);
      } else {
        //Skriv matrix størrelse ind
        Console.WriteLine("Colums");
	      columns = Int32.Parse(Console.ReadLine());
	      Console.WriteLine("Rows");
	      rows = Int32.Parse(Console.ReadLine());
	    }
      var generateTime = System.Diagnostics.Stopwatch.StartNew();

      var matrix = new MatrixGenerator(rows, columns, 1).Matrix;
      var matrix2 = new MatrixGenerator(rows, columns, 1).Matrix;

      double density = 0;

      for (int i = 0; i < rows; i++) {
        for (int j = 0; j < columns; j++) {
          if (matrix[i, j] < 0.0) {
            density += 1;
          }
        }
      };

      Console.WriteLine("Matrix Density: " + ((1 - density / (columns * rows)) * 100).ToString("0.00") + " %");
      generateTime.Stop();

      Console.WriteLine($"Generate time: {generateTime.Elapsed.TotalSeconds.ToString("0.00")} sec");
      var watch = System.Diagnostics.Stopwatch.StartNew();

      var svd = matrix.Svd(true);

      watch.Stop();
      Console.WriteLine("SVD Execution Time: " + watch.Elapsed.TotalSeconds.ToString("0.00") + " sec!");


      var timing = System.Diagnostics.Stopwatch.StartNew();
      // real data likely to come from a text file or SQL
      double[][] rawData = matrix.ToRowArrays();

      // Antallet af clusters der inddeles i.
      int numClusters = 3;

      Cluster cluster = new Cluster(rawData, numClusters); // this is it

      cluster = cluster.Update();
      Console.WriteLine("\nK-means clustering complete\n");

      Console.WriteLine("Final clustering in internal form:\n");
      cluster.ShowVector();

      Console.WriteLine("Raw data by cluster:\n");
      cluster.ShowClustered(decimals: 1);

      Console.WriteLine("\nEnd k-means clustering demo\n");
      timing.Stop();
      Console.WriteLine("Cluster Execution Time: " + timing.Elapsed.TotalSeconds.ToString("0.00") + " sec!");
      Console.ReadKey();
    }

    public static void ConnectDatabase() {
      SqlConnection myConnection = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=RecommenderSystems;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
      try {
        myConnection.Open();
        Console.WriteLine("Success!");
      } catch (SqlException ex) {
        Console.WriteLine("Connection failed!" + ex.Message);
      }
    }
  }
}
