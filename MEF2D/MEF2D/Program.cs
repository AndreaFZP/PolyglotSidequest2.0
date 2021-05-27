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
		}
	}
}
