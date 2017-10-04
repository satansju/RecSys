using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using MathNet.Numerics.LinearAlgebra;

namespace P2_recsys.Tests
{
	public class Tests
	{
		[SetUp]
		public void BeforeEachTest()
		{
		}

		[Test]
		public void MatrixCanGenerateValidMatrix()
		{
			MatrixGenerator matrix = new MatrixGenerator(10, 20, 1.0);

			Assert.AreEqual(matrix.density, 1.0);
			Assert.IsInstanceOf(typeof(Matrix<double>), matrix.Matrix);
		}

		[Test]
		public void MatrixGeneratorDoesNotThrowException()
		{
			Assert.DoesNotThrow(() => {
				var matrix = new MatrixGenerator(10, 20, 1.0);
			});
		}
	}
}
