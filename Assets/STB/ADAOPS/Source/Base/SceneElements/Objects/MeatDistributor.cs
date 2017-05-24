using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: BloodyObject
	/// # Manager to handle a meat distributor (it expends meat pieces everywhere)
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class MeatDistributor : MonoBehaviour
	{
		// public
		public List<GameObject> meatPrefabs = new List<GameObject> ();

		// private
		float timerBetweenDistributions = 1;

		// private -- list
		List<GameObject> actualCloneList = new List<GameObject> ();


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start
		/// # Start the class
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			this.GetComponent<Renderer> ().enabled = false;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Update
		/// # Update the class
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Update ()
		{
			if (timerBetweenDistributions <= 0)
			{
				GameObject baseContainer = BasicFunctions.CreateContainerIfNotExists ("_MEAT_PARTS");

				timerBetweenDistributions = Random.Range (1, 2);

				int index = Random.Range (0, meatPrefabs.Count);

				GameObject clone = Instantiate (meatPrefabs [index]);

				clone.transform.position = this.transform.position;
				//clone.GetComponent<Rigidbody> ().velocity = (-4 + 4 * Random.Range (0, 1)) * Vector3.one;


				Vector3 objectScale = clone.transform.localScale;

				objectScale.x *= Random.Range (0.5f, 2);

				if (index == 0)
				{
					objectScale.y *= Random.Range (0.5f, 2);
					objectScale.z *= Random.Range (0.5f, 2);
				}
				else
				{
					objectScale.y = objectScale.x;
					objectScale.z = objectScale.x;
				}

				clone.transform.localScale = objectScale;

				clone.transform.Rotate (Random.Range (0, 360) * Vector3.one);

				clone.transform.parent = baseContainer.transform;

				if (!clone.GetComponent<NotStainableObject> ())
				{
					clone.AddComponent<NotStainableObject> ();
				}

				actualCloneList.Add (clone);

				if (actualCloneList.Count > 24)
				{
					GameObject removable = actualCloneList [0];

					actualCloneList.Remove (removable);
					Destroy (removable);
				}
			}

			timerBetweenDistributions -= Time.deltaTime;
		}
	}
}