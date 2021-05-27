using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MEF2D
{
    public class Math_tools
    {
		public void zeroes(List<List<float>> M, int n)
		{
			for (int i = 0; i < n; i++)
			{
				List<float> row = new List<float>(n);
				M.Add(row);
			}
		}

		public void zeroes(List<List<float>> M, int n, int m)
		{
			for (int i = 0; i < n; i++)
			{
				List<float> row = new List<float>(m);
				M.Add(row);
			}
		}

		public void zeroes(List<float> v, int n)
		{
			for (int i = 0; i < n; i++)
			{
				v.Add(0.0F);
			}
		}

		public void copyMatrix(List<List<float>> A, List<List<float>> copy)
		{
			zeroes(copy, A.Count);
			for (int i = 0; i < A.Count; i++)
			{
				for (int j = 0; j < A[0].Count; j++)
				{
					copy[i][j] = A[i][j];
				}
			}
		}

		public float calculateMember(int i, int j, int r, List<List<float>> A, List<List<float>> B)
		{
			float member = 0F;
			for (int k = 0; k < r; k++)
			{
				member += A[i][k] * B[k][j];
			}
			return member;
		}

		public List<List<float>> productMatrixMatrix(List<List<float>> A, List<List<float>> B, int n, int r, int m)
		{
			List<List<float>> R = new List<List<float>>();

			zeroes(R, n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					R[i][j] = calculateMember(i, j, r, new List<List<float>>(A), new List<List<float>>(B));
				}
			}

			return new List<List<float>>(R);
		}

		public void productMatrixVector(List<List<float>> A, List<float> v, List<float> R)
		{
			for (int f = 0; f < A.Count; f++)
			{
				float cell = 0.0F;
				for (int c = 0; c < v.Count; c++)
				{
					cell += A[f][c] * v[c];
				}
				R[f] += cell;
			}
		}

		public void productRealMatrix(float real, List<List<float>> M, List<List<float>> R)
		{
			zeroes(R, M.Count);
			for (int i = 0; i < M.Count; i++)
			{
				for (int j = 0; j < M[0].Count; j++)
				{
					R[i][j] = real * M[i][j];
				}
			}
		}

		public void getMinor(List<List<float>> M, int n, int j)
		{
			M.RemoveAt(n);
			for (int i = 0; i < M.Count; i++)
			{
				//M[i].RemoveAt(M[i].GetEnumerator() + j);
			}
		}

		public float determinant(List<List<float>> M)
		{
			if (M.Count == 1)
			{
				return M[0][0];
			}
			else
			{
				float det = 0.0F;
				for (int i = 0; i < M[0].Count; i++)
				{
					List<List<float>> minor = new List<List<float>>();
					copyMatrix(new List<List<float>>(M), minor);
					getMinor(minor, 0, i);
					det += (float)Math.Pow(-1, i) * M[0][i] * determinant(new List<List<float>>(minor));
				}
				return det;
			}
		}

		public void cofactors(List<List<float>> M, List<List<float>> Cof)
		{
			zeroes(Cof, M.Count());
			for (int i = 0; i < M.Count(); i++)
			{
				for (int j = 0; j < M[0].Count; j++)
				{
					List<List<float>> minor = new List<List<float>>();
					copyMatrix(M, minor);
					getMinor(minor, i, j);
					Cof[i][j] = (float)(Math.Pow(-1, i + j) * determinant(minor));

				}
			}
		}

		public void transpose(List<List<float>> M, List<List<float>> T)
		{
			zeroes(T, M[0].Count, M.Count);
			for (int i = 0; i < M.Count; i++)
			{
				for (int j = 0; j < M[0].Count; j++)
				{
					T[j][i] = M[i][j];
				}
			}

		}

		public void inverseMatrix(List<List<float>> M, List<List<float>> Minv)
		{
			Console.Write("Iniciando calculo de inversa...\n");
			List<List<float>> Cof = new List<List<float>>();
			List<List<float>> Adj = new List<List<float>>();
			Console.Write("Calculo de determinante...\n");
			float det = determinant(M);
			if (det == 0F)
			{
				Environment.Exit(1);
			}
			Console.Write("Iniciando calculo de cofactores...\n");
			cofactors(new List<List<float>>(M), Cof);
			Console.Write("Calculo de adjunta...\n");
			transpose(new List<List<float>>(Cof), Adj);
			Console.Write("Calculo de inversa...\n");
			productRealMatrix(1 / det, Adj, Minv);
		}
	}
}
