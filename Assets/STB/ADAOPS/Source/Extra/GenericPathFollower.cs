using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: GenericPathFollower
	/// # To handle objects which want to follow a path using a spline and waypoints
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class GenericPathFollower : MonoBehaviour
	{
		// private
		public float speed = 1000;
		public bool loop = true;
		public bool closed = true;
		public float framerate = 10;
		public List<Vector3> waypointPosition = new List<Vector3> ();
		public List<Vector3> waypointNormal = new List<Vector3> ();

		// private
		GenericSplineFunction positionSpline = null;
		GenericSplineFunction normalSpline = null;
		GenericMeshDecal actualDecal = null;
		float counterBetweenUpdates = 0.1f;

			
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			positionSpline = new GenericSplineFunction ();
			normalSpline = new GenericSplineFunction ();	

			for (int i = 0; i < waypointPosition.Count; i++)
			{
				positionSpline.AddPoint (waypointPosition [i], !closed && (i == waypointPosition.Count - 1));
				normalSpline.AddPoint (waypointNormal [i], !closed && (i == waypointPosition.Count - 1));
			}
			
			if (closed)
			{
				positionSpline.AddPoint (waypointPosition [0], true);
				normalSpline.AddPoint (waypointNormal [0], true);
			}

			actualDecal = this.gameObject.GetComponent<GenericMeshDecal> ();
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Create
		/// # Create the object given a waypoint list
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Create (List<WayPoint> waypointList, bool closed, bool loop)
		{			
			this.loop = loop;
			this.closed = closed;

			waypointPosition.Clear ();
			waypointNormal.Clear ();

			for (int i = 0; i < waypointList.Count; i++)
			{
				waypointPosition.Add (waypointList [i].transform.position);
				waypointNormal.Add (waypointList [i].hitNormal);
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Update
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void Update ()
		{			
			if (counterBetweenUpdates <= 0)
			{
				counterBetweenUpdates = 1.0f/framerate; 

				this.transform.localPosition = positionSpline.GetActualPoint () + 0.1f * normalSpline.GetActualPoint ();
				//this.transform.up = normalSpline.GetActualPoint ();
			
			
				if (actualDecal)
				{				
					//this.transform.rotation = Quaternion.FromToRotation (Vector3.up, normalSpline.GetActualPoint ());
					//this.transform.up = normalSpline.GetActualPoint ();

					actualDecal.BuildMesh (actualDecal);
				}
			}

			counterBetweenUpdates -= Time.deltaTime;

			if (loop)
			{
				positionSpline.Update (speed * Time.deltaTime);	
				normalSpline.Update (speed * Time.deltaTime);	
			}
			else
			{
				positionSpline.UpdateUntilEnd (speed * Time.deltaTime);	
				normalSpline.UpdateUntilEnd (speed * Time.deltaTime);	
			}
		}
	}
}