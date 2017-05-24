using UnityEngine;
using System.Collections;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: ObjectLauncher
	/// # To handle object's launching from player
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class ObjectLauncher : MonoBehaviour
	{
		// public
		public DirtyObject dirtyObjectPrefab = null;
		public DirtyObject bloodyObjectPrefab = null;
		public DirtyObject sparksObjectPrefab = null;
		public DirtyObject watterObjectPrefab = null;
		public DirtyObject bulletObject = null;
		public DirtyObject craterCreator = null;

		// private
		GameObject objectsContainer = null;

		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start
		/// # Initialise all
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			objectsContainer = BasicFunctions.CreateContainerIfNotExists ("_OBJECTS");
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Update
		/// # Update the class and get mouse inputs
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Update ()
		{
			DirtyObject dirtyObject = null;


			// put a bullet decal
			if (Input.GetMouseButtonDown (0))
			{
				RaycastHit hit = new RaycastHit ();
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				
				if (Physics.Raycast (ray, out hit))
				{
					if (DecalInGameManager.DECAL_INGAME_MANAGER)
					{
						GenericBulletDecal.ShootNewBullet (bulletObject.decalMaterial, hit.point, hit.normal);
					}
				}				
			}

			// throw a simple ray to put a watter object
			if (Input.GetMouseButtonDown (1))
			{
				RaycastHit hit = new RaycastHit ();
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				
				if (Physics.Raycast (ray, out hit))
				{
					if (DecalInGameManager.DECAL_INGAME_MANAGER)
					{
						DecalInGameManager.DECAL_INGAME_MANAGER.CreateNewMeshDecal (watterObjectPrefab.decalMaterial, hit.transform, hit.point, hit.normal, 4, Vector2.zero, false);
					}
				}				
			}

			// throw a crater creator
			if (Input.GetKeyDown (KeyCode.C))
			{
				//Debug.Log("Create new crater creator");
				dirtyObject = Instantiate (craterCreator);
			}

			// throw a random dirty object
			if (Input.GetKeyDown (KeyCode.Space))
			{
				int randomNumber = Random.Range (0, 2);


				// throw a simple bloody object
				if (randomNumber == 0)
				{
					dirtyObject = Instantiate (bloodyObjectPrefab);
				}
			
				// throw a simple burning object
				if (randomNumber == 1)
				{
					dirtyObject = Instantiate (sparksObjectPrefab);
				}
			
				// throw a simple watter object
				if (randomNumber == 2)
				{
					dirtyObject = Instantiate (watterObjectPrefab);
				}
				
				// throw a simple dirty object
				if (randomNumber == 3)
				{
					dirtyObject = Instantiate (dirtyObjectPrefab);
				}
			}


			// if we have created a new dirty object
			if (dirtyObject)
			{
				dirtyObject.transform.position = this.transform.position + 2 * this.transform.forward;
				dirtyObject.GetComponent<Rigidbody> ().velocity = 10 * this.transform.forward;
				dirtyObject.name = bloodyObjectPrefab.name;
				dirtyObject.transform.parent = objectsContainer.transform;
				
				Destroy (dirtyObject.gameObject, 9);
			}
		}
	}
}