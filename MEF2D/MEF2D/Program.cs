using System;
using System.Collections.Generic;


namespace MEF2D
{
    class Program
    {
        static void Main(string[] args)
        {
			string filename = new string(new char[150]);
			filename = args[1];

			List<List<List<float>>> localKs = new List<List<List<float>>>();
			List<List<float>> localbs = new List<List<float>>();
			List<List<float>> K = new List<List<float>>();
			List<float> b = new List<float>();
			List<float> T = new List<float>();

			Console.Write("IMPLEMENTACION DEL METODO DE LOS ELEMENTOS FINITOS\n");
			Console.Write("\t- TRANSFERENCIA DE CALOR\n");
			Console.Write("\t- 2 DIMENSIONES\n");
			Console.Write("\t- FUNCIONES DE FORMA LINEALES\n");
			Console.Write("\t- PESOS DE GALERKIN\n");
			Console.Write("\t- MALLA TRIANGULAR IRREGULAR\n");
			Console.Write("*********************************************************************************\n\n");

			Sel nMethod = new Sel();
			Math_tools aMethod = new Math_tools();

			Classes.mesh m = new Classes.mesh();
			Tools.leerMallayCondiciones(m, filename);
			Console.Write("Datos obtenidos correctamente\n********************\n");


			nMethod.crearSistemasLocales(m, localKs, localbs);
			//showKs(localKs); showbs(localbs);
			Console.Write("******************************\n");

			aMethod.zeroes(K, m.getSize((int)Classes.sizes.NODES));
			aMethod.zeroes(b, m.getSize((int)Classes.sizes.NODES));
			nMethod.ensamblaje(m, localKs, localbs, K, b);
			//showMatrix(K); showVector(b);
			Console.Write("******************************\n");
			//cout << K.size() << " - "<<K.at(0).size()<<"\n";
			//cout << b.size() <<"\n";

			nMethod.applyNeumann(m, b);
			//showMatrix(K); showVector(b);
			Console.Write("******************************\n");
			//cout << K.size() << " - "<<K.at(0).size()<<"\n";
			//cout << b.size() <<"\n";


			nMethod.applyDirichlet(m, K, b);
			//showMatrix(K); showVector(b);
			Console.Write("******************************\n");
			//cout << K.size() << " - "<<K.at(0).size()<<"\n";
			//cout << b.size() <<"\n";

			aMethod.zeroes(T, b.Count);
			nMethod.calculate(K, b, T);

			//cout << "La respuesta es: \n";
			//showVector(T);

			Tools.writeResults(m, T, filename);
		}
	}
}
