using UnityEngine;
using System.Collections;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: DecalInGameManager
	/// # Manager to handle in game decals
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class DecalInGameManager : MonoBehaviour
	{
		// public static
		public static DecalInGameManager DECAL_INGAME_MANAGER = null;

		// public -- prefabs
		public GenericMeshDecal decalPrefab = null;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start
		/// # Start the class
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			DECAL_INGAME_MANAGER = this;

			if (!decalPrefab)
			{
				print ("NOTE -> decalPrefab not defined in DecalInGameManager, you have to add it: " + gameObject.name);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// CreateNewDecal
		/// # Create a new mesh decal in the scene
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public GenericMeshDecal CreateNewMeshDecal (Material decalMaterial, Transform parent, Vector3 point, Vector3 normal, float scaleMultiplier, Vector2 rotationRange, bool attachDecalToCollisionObject)
		{			
			//print ("CreateNewDecal");

			GenericMeshDecal actualDecal = Instantiate (decalPrefab.gameObject).GetComponent<GenericMeshDecal> ();
			actualDecal.material = decalMaterial;
			actualDecal.transform.position = point + 0.001f * normal;
			actualDecal.transform.localScale = scaleMultiplier * actualDecal.transform.localScale;
			actualDecal.transform.rotation = Quaternion.FromToRotation (Vector3.up, normal);
			
			if (attachDecalToCollisionObject)
			{
				actualDecal.transform.parent = parent;
			}
			else
			{
				GameObject decalsContainer = BasicFunctions.CreateContainerIfNotExists (BasicDefines.MESH_DECAL_CONTAINER_NAME);
				actualDecal.transform.parent = decalsContainer.transform;
			}

			actualDecal.rotationRange = rotationRange;
			
			actualDecal.name = "RunTimeDecal";

			actualDecal.UpdateDecallShape (true, false);


			return actualDecal.GetComponent<GenericMeshDecal> ();
		}
	}
}
