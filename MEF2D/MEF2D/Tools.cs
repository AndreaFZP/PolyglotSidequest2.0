using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using static MEF2D.Classes;

namespace MEF2D
{
    public class Tools
    {
		public void obtenerDatos(File file, int nlines, int n, int mode, item[] item_list)
		{
			if (nlines == (int)lines.DOUBLELINE)
			{
				string line = File.ReadAllText("file");
			}

			for (int i = 0; i < n; i++)
			{
				switch (mode)
				{
					case (int)modes.INT_FLOAT:
						int e0;
						float r0;
						file >> e0 >> r0;
						item_list[i].setValues((int)indicators.NOTHING, (int)indicators.NOTHING, (int)indicators.NOTHING, e0, (int)indicators.NOTHING, (int)indicators.NOTHING, r0);
						break;
					case (int)modes.INT_FLOAT_FLOAT:
						int e;
						float r;
						float rr;
						file >> e >> r >> rr;
						item_list[i].setValues(e, r, rr, (int)indicators.NOTHING, (int)indicators.NOTHING, (int)indicators.NOTHING, (int)indicators.NOTHING);
						break;
					case (int)modes.INT_INT_INT_INT:
						int e1;
						int e2;
						int e3;
						int e4;
						file >> e1 >> e2 >> e3 >> e4;
						item_list[i].setValues(e1, (int)indicators.NOTHING, (int)indicators.NOTHING, e2, e3, e4, (int)indicators.NOTHING);
						break;
				}
			}
		}

		public void correctConditions(int n, condition[] list, int[] indices)
		{
			for (int i = 0; i < n; i++)
			{
				indices[i] = list[i].getNode1();
			}

			for (int i = 0; i < n - 1; i++)
			{
				int pivot = list[i].getNode1();
				for (int j = i; j < n; j++)
				{

					if (list[j].getNode1() > pivot)
					{
						list[j].setNode1(list[j].getNode1() - 1);
					}
				}
			}
		}

		public void addExtension(ref string newfilename, ref string filename, ref string extension)
		{
			int ori_length = filename.Length;
			int ext_length = extension.Length;
			int i;
			for (i = 0; i < ori_length; i++)
			{
				newfilename[i] = filename[i];
			}
			for (i = 0; i < ext_length; i++)
			{
				newfilename[ori_length + i] = extension[i];
			}
			newfilename[ori_length + i] = '\0';
		}

		public void leerMallayCondiciones(mesh m, ref string filename)
		{
			string inputfilename = new string(new char[150]);
			ifstream file = new ifstream();
			float k;
			float Q;
			int nnodes;
			int neltos;
			int ndirich;
			int nneu;

			addExtension(ref inputfilename, ref filename, ".dat");
			file.open(inputfilename);

			file >> k >> Q;

			file >> nnodes >> neltos >> ndirich >> nneu;


			m.setParameters(k, Q);
			m.setSizes(nnodes, neltos, ndirich, nneu);
			m.createData();

			obtenerDatos(file, (int)lines.SINGLELINE, nnodes, (int)modes.INT_FLOAT_FLOAT, m.getNodes());
			obtenerDatos(file, (int)lines.DOUBLELINE, neltos, (int)modes.INT_INT_INT_INT, m.getElements());
			obtenerDatos(file, (int)lines.DOUBLELINE, ndirich, (int)modes.INT_FLOAT, m.getDirichlet());
			obtenerDatos(file, (int)lines.DOUBLELINE, nneu, (int)modes.INT_FLOAT, m.getNeumann());

			file.close();


			correctConditions(ndirich, m.getDirichlet(), m.getDirichletIndices());
		}

		private bool findIndex(int v, int s, int[] arr)
		{
			for (int i = 0; i < s; i++)
			{
				if (arr[i] == v)
				{
					return true;
				}
			}
			return false;
		}

		private void writeResults(mesh m, List<float> T, ref string filename)
		{
			string outputfilename = new string(new char[150]);
			int dirich_indices = m.getDirichletIndices();
			condition dirich = m.getDirichlet();
			ofstream file = new ofstream();

			addExtension(ref outputfilename, ref filename, ".post.res");
			file.open(outputfilename);

			file << "GiD Post Results File 1.0\n";
			file << "Result \"Temperature\" \"Load Case 1\" 1 Scalar OnNodes\nComponentNames \"T\"\nValues\n";

			int Tpos = 0;
			int Dpos = 0;
			int n = m.getSize((int)sizes.NODES);
			int nd = m.getSize((int)sizes.DIRICHLET);
			for (int i = 0; i < n; i++)
			{
				if (findIndex(i + 1, nd, dirich_indices))
				{
					file << i + 1 << " " << dirich[Dpos].getValue() << "\n";
					Dpos++;
				}
				else
				{
					file << i + 1 << " " << T.at(Tpos) << "\n";
					Tpos++;
				}
			}

			file << "End values\n";

			file.close();

		}
	}
}
