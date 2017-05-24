using UnityEngine;
using System.Collections;

namespace STB.ADAOPS
{
	public class MeshDecalsRotator : MonoBehaviour
	{
		// public
		public Transform pivot = null;

		// private
		float rotation = 0;

		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			GenericMeshDecal d = this.gameObject.GetComponent<GenericMeshDecal> ();

			if (d)
			{
				d.lockedShapeAlways = true;
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Update
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Update ()
		{
			rotation += 4 * Time.deltaTime;

			if (rotation > 360)
			{
				rotation = rotation - 360;
			}

			this.transform.RotateAround (pivot.transform.position, Vector3.up, rotation * Mathf.Deg2Rad);
			
			GenericMeshDecal d = this.gameObject.GetComponent<GenericMeshDecal> ();

			if (d)
			{
				d.BuildMesh (d);
			}
		}
	}
}
