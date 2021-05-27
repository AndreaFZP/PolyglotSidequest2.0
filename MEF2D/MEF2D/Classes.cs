using System;
using System.Collections.Generic;
using System.Text;

namespace MEF2D
{
    public class Classes
    {
		public enum indicators
		{
			NOTHING
		}
		public enum lines
		{
			NOLINE,
			SINGLELINE,
			DOUBLELINE
		}
		public enum modes
		{
			NOMODE,
			INT_FLOAT,
			INT_FLOAT_FLOAT,
			INT_INT_INT_INT
		}
		public enum parameters
		{
			THERMAL_CONDUCTIVITY,
			HEAT_SOURCE
		}
		public enum sizes
		{
			NODES,
			ELEMENTS,
			DIRICHLET,
			NEUMANN
		}

		public abstract class item
		{
			protected int id;
			protected float x;
			protected float y;
			protected int node1;
			protected int node2;
			protected int node3;
			protected float value;
			public void setId(int identifier)
			{
				id = identifier;
			}

			public void setX(float x_coord)
			{
				x = x_coord;
			}

			public void setY(float y_coord)
			{
				y = y_coord;
			}

			public void setNode1(int node_1)
			{
				node1 = node_1;
			}

			public void setNode2(int node_2)
			{
				node2 = node_2;
			}

			public void setNode3(int node_3)
			{
				node3 = node_3;
			}

			public void setValue(float value_to_assign)
			{
				value = value_to_assign;
			}

			public int getId()
			{
				return id;
			}

			public float getX()
			{
				return x;
			}

			public float getY()
			{
				return y;
			}

			public int getNode1()
			{
				return node1;
			}

			public int getNode2()
			{
				return node2;
			}

			public int getNode3()
			{
				return node3;
			}

			public float getValue()
			{
				return value;
			}

			public abstract void setValues(int a, float b, float c, int d, int e, int f, float g);

		}

		public class node : item
		{

			public override void setValues(int a, float b, float c, int d, int e, int f, float g)
			{
				id = a;
				x = b;
				y = c;
			}

		}

		public class element : item
		{

			public override void setValues(int a, float b, float c, int d, int e, int f, float g)
			{
				id = a;
				node1 = d;
				node2 = e;
				node3 = f;
			}

		}

		public class condition : item
		{


			public override void setValues(int a, float b, float c, int d, int e, int f, float g)
			{
				node1 = d;
				value = g;
			}

		}

		public class mesh
		{
			private float[] parameters1 = new float[2];
			private int[] sizes1 = new int[4];
			private node[] node_list;
			private element[] element_list;
			private int[] indices_dirich;
			private condition[] dirichlet_list;
			private condition[] neumann_list;
			public void setParameters(float k, float Q)
			{
				parameters1[(int)parameters.THERMAL_CONDUCTIVITY] = k;
				parameters1[(int)parameters.HEAT_SOURCE] = Q;
			}
			public void setSizes(int nnodes, int neltos, int ndirich, int nneu)
			{
				sizes1[(int)sizes.NODES] = nnodes;
				sizes1[(int)sizes.ELEMENTS] = neltos;
				sizes1[(int)sizes.DIRICHLET] = ndirich;
				sizes1[(int)sizes.NEUMANN] = nneu;
			}
			public int getSize(int s)
			{
				return sizes1[s];
			}
			public float getParameter(int p)
			{
				return parameters1[p];
			}
			public void createData()
			{
				node_list = Arrays.InitializeWithDefaultInstances<node>(sizes1[(int)sizes.NODES]);
				element_list = Arrays.InitializeWithDefaultInstances<element>(sizes1[(int)sizes.ELEMENTS]);
				indices_dirich = new int[(int)sizes.DIRICHLET];
				dirichlet_list = Arrays.InitializeWithDefaultInstances<condition>(sizes1[(int)sizes.DIRICHLET]);
				neumann_list = Arrays.InitializeWithDefaultInstances<condition>(sizes1[(int)sizes.NEUMANN]);
			}
			public node[] getNodes()
			{
				return node_list;
			}
			public element[] getElements()
			{
				return element_list;
			}

			public int[] getDirichletIndices()
			{
				return indices_dirich;
			}
			public condition[] getDirichlet()
			{
				return dirichlet_list;
			}
			public condition[] getNeumann()
			{
				return neumann_list;
			}
			public node[] getNode(int i)
			{
				return new node(node_list[i]);
			}
			public element[] getElement(int i)
			{
				return new element(element_list[i]);
			}
			public condition[] getCondition(int i, int type)
			{
				if (type == (int)sizes.DIRICHLET)
				{

					return new condition(dirichlet_list[i]);
				}
				else
				{

					return new condition(neumann_list[i]);
				}
			}
		}

		internal static class Arrays
		{
			public static T[] InitializeWithDefaultInstances<T>(int length) where T : new()
			{
				T[] array = new T[length];
				for (int i = 0; i < length; i++)
				{
					array[i] = new T();
				}
				return array;
			}

			public static void DeleteArray<T>(T[] array) where T : System.IDisposable
			{
				foreach (T element in array)
				{
					if (element != null)
						element.Dispose();
				}
			}
		}


	}
}
