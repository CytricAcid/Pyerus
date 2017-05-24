using UnityEngine;
using System.Collections.Generic;

namespace STB.ADAOPS
{
///////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>
/// Class: GenericPoly
/// # To create a new generic poly
/// </summary>
///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class GenericPoly
	{	
		// private
		const int vertexNumber = 9;

		// public
		public List<Vector3> vertexList = new List<Vector3> (vertexNumber);

	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Constructor
		/// # Initialise the class given a vertex list
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public GenericPoly (params Vector3[] vts)
		{
			vertexList.AddRange (vts);
		}	
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ClipPoly
		/// # Return clip poly for actual poly
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static GenericPoly ClipPoly (GenericPoly actualPoly, Plane plane)
		{
			//Debug.Log ("ClipPoly");

			bool[] positive = new bool[vertexNumber];
			int positiveCount = 0;

			for (int i = 0; i < actualPoly.vertexList.Count; i++)
			{
				positive [i] = !plane.GetSide (actualPoly.vertexList [i]);

				if (positive [i])
				{
					positiveCount++;
				}
			}
		
			if (positiveCount == 0)
			{
				return null; 
			}

			if (positiveCount == actualPoly.vertexList.Count)
			{
				return actualPoly;
			}

			GenericPoly temporalPoly = new GenericPoly ();

			for (int i = 0; i < actualPoly.vertexList.Count; i++)
			{
				int next = i + 1;
				next %= actualPoly.vertexList.Count;

				if (positive [i])
				{
					temporalPoly.vertexList.Add (actualPoly.vertexList [i]);
				}

				if (positive [i] != positive [next])
				{
					Vector3 v1 = actualPoly.vertexList [next];
					Vector3 v2 = actualPoly.vertexList [i];
				
					Vector3 v = BasicFunctions.LineCast (plane, v1, v2);
					temporalPoly.vertexList.Add (v);
				}
			}
		
			return temporalPoly;
		}
	}
}