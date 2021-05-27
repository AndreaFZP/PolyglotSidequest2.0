using System;
using System.Collections.Generic;
using System.Text;
using static MEF2D.Classes;

namespace MEF2D
{
	public class Sel
	{
		public void showMatrix(List<List<float>> K)
		{
			for (int i = 0; i < K[0].Count; i++)
			{
				Console.Write("[\t");
				for (int j = 0; j < K.Count; j++)
				{
					Console.Write(K[i][j]);
					Console.Write("\t");
				}
				Console.Write("]\n");
			}
		}

		public void showKs(List<List<List<float>>> Ks)
		{
			for (int i = 0; i < Ks.Count; i++)
			{
				Console.Write("K del elemento ");
				Console.Write(i + 1);
				Console.Write(":\n");
				showMatrix(Ks[i]);
				Console.Write("*************************************\n");
			}
		}

		public void showVector(List<float> b)
		{
			Console.Write("[\t");
			for (int i = 0; i < b.Count; i++)
			{
				Console.Write(b[i]);
				Console.Write("\t");
			}
			Console.Write("]\n");
		}

		public void showbs(List<List<float>> bs)
		{
			for (int i = 0; i < bs.Count; i++)
			{
				Console.Write("b del elemento ");
				Console.Write(i + 1);
				Console.Write(":\n");
				showVector(bs[i]);
				Console.Write("*************************************\n");
			}
		}

		public float calculateLocalD(int i, mesh m)
		{
			float D;
			float a;
			float b;
			float c;
			float d;

			element e = m.getElement(i);

			node n1 = m.getNode(e.getNode1() - 1);
			node n2 = m.getNode(e.getNode2() - 1);
			node n3 = m.getNode(e.getNode3() - 1);

			a = n2.getX() - n1.getX();
			b = n2.getY() - n1.getY();
			c = n3.getX() - n1.getX();
			d = n3.getY() - n1.getY();
			D = a * d - b * c;

			return D;
		}

		public float calculateMagnitude(float v1, float v2)
		{
			return Math.Sqrt(Math.Pow(v1, 2) + Math.Pow(v2, 2));
		}

		public float calculateLocalArea(int i, mesh m)
		{
			
			float A;
			float s;
			float a;
			float b;
			float c;
			element e = m.getElement(i);
			node n1 = m.getNode(e.getNode1() - 1);
			node n2 = m.getNode(e.getNode2() - 1);
			node n3 = m.getNode(e.getNode3() - 1);

			a = calculateMagnitude(n2.getX() - n1.getX(), n2.getY() - n1.getY());
			b = calculateMagnitude(n3.getX() - n2.getX(), n3.getY() - n2.getY());
			c = calculateMagnitude(n3.getX() - n1.getX(), n3.getY() - n1.getY());
			s = (a + b + c) / 2;

			A = Math.Sqrt(s * (s - a) * (s - b) * (s - c));
			return A;
		}

		public void calculateLocalA(int i, List<List<float>> A, mesh m)
		{
			element e = m.getElement(i);
			node n1 = m.getNode(e.getNode1() - 1);
			node n2 = m.getNode(e.getNode2() - 1);
			node n3 = m.getNode(e.getNode3() - 1);
			A[0][0] = n3.getY() - n1.getY();
			A[0][1] = n1.getY() - n2.getY();
			A[1][0] = n1.getX() - n3.getX();
			A[1][1] = n2.getX() - n1.getX();
		}

		public void calculateB(List<List<float>> B)
		{
			B[0][0] = -1F;
			B[0][1] = 1F;
			B[0][2] = 0F;
			B[1][0] = -1F;
			B[1][1] = 0F;
			B[1][2] = 1F;
		}

		public List<List<float>> createLocalK(int element, mesh m)
		{
			
			float D;
			float Ae;
			float k = m.getParameter((int)parameters.THERMAL_CONDUCTIVITY);
			List<List<float>> K = new List<List<float>>();
			List<List<float>> A = new List<List<float>>();
			List<List<float>> B = new List<List<float>>();
			List<List<float>> Bt = new List<List<float>>();
			List<List<float>> At = new List<List<float>>();

			D = calculateLocalD(element, m);
			Ae = calculateLocalArea(element, m);

			zeroes(A, 2);
			zeroes(B, 2, 3);
			calculateLocalA(element, A, m);
			calculateB(B);
			transpose(A, At);
			transpose(B, Bt);

			productRealMatrix(k * Ae / (D * D), productMatrixMatrix(Bt, productMatrixMatrix(At, productMatrixMatrix(A, B, 2, 2, 3), 2, 2, 3), 3, 2, 3), K);

			return new List<List<float>>(K);
		}

		public float calculateLocalJ(int i, mesh m)
		{
			float J;
			float a;
			float b;
			float c;
			float d;
			element e = m.getElement(i);
			node n1 = m.getNode(e.getNode1() - 1);
			node n2 = m.getNode(e.getNode2() - 1);
			node n3 = m.getNode(e.getNode3() - 1);

			a = n2.getX() - n1.getX();
			b = n3.getX() - n1.getX();
			c = n2.getY() - n1.getY();
			d = n3.getY() - n1.getY();
			J = a * d - b * c;

			return J;
		}

		public List<float> createLocalb(int element, mesh m)
		{
			List<float> b = new List<float>();

			float Q = m.getParameter((int)parameters.HEAT_SOURCE);
			float J;
			float b_i;
			J = calculateLocalJ(element, new mesh(m));

			b_i = Q * J / 6;
			b.Add(b_i);
			b.Add(b_i);
			b.Add(b_i);

			return new List<float>(b);
		}

		public void crearSistemasLocales(mesh m, List<List<List<float>>> localKs, List<List<float>> localbs)
		{
			for (int i = 0; i < m.getSize(ELEMENTS); i++)
			{
				localKs.Add(createLocalK(i, m));
				localbs.Add(createLocalb(i, m));
			}
		}

		public void assemblyK(element e, List<List<float>> localK, List<List<float>> K)
		{
			int index1 = e.getNode1() - 1;
			int index2 = e.getNode2() - 1;
			int index3 = e.getNode3() - 1;

			K[index1][index1] += localK[0][0];
			K[index1][index2] += localK[0][1];
			K[index1][index3] += localK[0][2];
			K[index2][index1] += localK[1][0];
			K[index2][index2] += localK[1][1];
			K[index2][index3] += localK[1][2];
			K[index3][index1] += localK[2][0];
			K[index3][index2] += localK[2][1];
			K[index3][index3] += localK[2][2];
		}

		public void assemblyb(element e, List<float> localb, List<float> b)
		{
			int index1 = e.getNode1() - 1;
			int index2 = e.getNode2() - 1;
			int index3 = e.getNode3() - 1;

			b[index1] += localb[0];
			b[index2] += localb[1];
			b[index3] += localb[2];
		}

		public void ensamblaje(mesh m, List<List<List<float>>> localKs, List<List<float>> localbs, List<List<float>> K, List<float> b)
		{
			for (int i = 0; i < m.getSize(ELEMENTS); i++)
			{
				element e = m.getElement(i);
				assemblyK(new element(e), new List<List<List<float>>>(localKs[i]), K);
				assemblyb(new element(e), new List<List<float>>(localbs[i]), b);
			}
		}

		public void applyNeumann(mesh m, List<float> b)
		{
			for (int i = 0; i < m.getSize(NEUMANN); i++)
			{
				condition c = m.getCondition(i, NEUMANN);
				b[c.getNode1() - 1] += c.getValue();
			}
		}

		public void applyDirichlet(mesh m, List<List<float>> K, List<float> b)
		{
			for (int i = 0; i < m.getSize(DIRICHLET); i++)
			{
				condition c = m.getCondition(i, DIRICHLET);
				int index = c.getNode1() - 1;

				K.RemoveAt(index);
				b.RemoveAt(index);

				for (int row = 0; row < K.Count; row++)
				{
					float cell = K[row][index];
					K[row].Remove(K[row].GetEnumerator() + index);
					b[row] += -1 * c.getValue() * cell;
				}
			}
		}

		public void calculate(List<List<float>> K, List<float> b, List<float> T)
		{
			Console.Write("Iniciando calculo de respuesta...\n");
			List<List<float>> Kinv = new List<List<float>>();
			Console.Write("Calculo de inversa...\n");
			inverseMatrix(K, Kinv);
			Console.Write("Calculo de respuesta...\n");
			productMatrixVector(Kinv, b, T);
		}

	}
}
