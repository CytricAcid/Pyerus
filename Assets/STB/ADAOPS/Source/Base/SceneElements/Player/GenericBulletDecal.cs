using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: GenericBulletDecal
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class GenericBulletDecal
	{
		// static
		static Material bulletMaterial = null;
		static List<GenericMeshDecal> bulletList = new List<GenericMeshDecal> ();
		static int bulletCounter = 0;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// LoadBulletList
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static void LoadBulletList ()
		{
			if ((bulletList.Count <= 0) && (bulletMaterial))
			{
				for (int i = 0; i < 20; i++)
				{
					if (DecalInGameManager.DECAL_INGAME_MANAGER)
					{
						bulletList.Add (DecalInGameManager.DECAL_INGAME_MANAGER.CreateNewMeshDecal (bulletMaterial, null, BasicDefines.TOO_FAR_POSITION, Vector3.one, 4, Vector2.zero, false));
					}
				}
			}
		} 
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// ShootNewBullet
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void ShootNewBullet (Material material, Vector3 hitPoint, Vector3 hitNormal)
		{				
			//Debug.Log ("ShootNewBullet A");

			bulletMaterial = material;

			LoadBulletList ();
						
			if (bulletList.Count > 0)
			{
				//Debug.Log ("ShootNewBullet B");

				bulletList [bulletCounter].material = bulletMaterial;
				bulletList [bulletCounter].transform.position = hitPoint + 0.001f * hitNormal;
				bulletList [bulletCounter].transform.rotation = Quaternion.FromToRotation (Vector3.up, hitNormal);
				bulletList [bulletCounter].transform.localScale = 0.001f * Random.Range (300, 400) * Vector3.one;
				bulletList [bulletCounter].UpdateDecallShape (true, true);
				bulletList [bulletCounter].rotationRange = new Vector2 (0, 360);

				bulletCounter++;

				if (bulletCounter >= bulletList.Count)
				{
					bulletCounter = 0;
				}
			}
		}
	}
}