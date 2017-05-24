using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: GenericDestroyable
	/// # To handle all destroyable objects from the editor
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class GenericDestroyable : MonoBehaviour
	{		
		// static
		static int CLASS_INDEX = 0;

		// protected
		protected string baseName = "BASENAME_NOT_DEFINED";
		protected string seed = "SEED_NOT_DEFINED";

		// private
		int index = -1;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Generate
		/// # Generate a new object's name using given seed, basename and index
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public string Generate (string name, string seed, bool returnSimpleName, string simpleNamePrefix)
		{
			this.baseName = name;
			this.seed = seed;
			this.index = CLASS_INDEX;

			// increase class index for the next instance
			CLASS_INDEX++;

			
			if (returnSimpleName)
			{
				return (simpleNamePrefix + "_" + this.index.ToString ());
			}
			
			// return complete name
			return (this.baseName + "_" + this.seed + "_" + this.index.ToString ());
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetComplexName
		/// # Return the object complex name
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public string GetComplexName ()
		{
			return (this.baseName + "_" + this.seed + "_" + this.index.ToString ());
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetIndex
		/// # Return the object index
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public int GetIndex ()
		{
			return index;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetBaseName
		/// # Return the object basename
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public string GetBaseName ()
		{
			return baseName;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// GetSeed
		/// # Return the object seed
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public string GetSeed ()
		{
			return seed;
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DestroyAll
		/// # Destroy all destroyable objects given baseName (optional)
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void DestroyAll (bool byBaseName, string baseName)
		{
			foreach (Object destroyable in GameObject.FindObjectsOfType (typeof(GenericDestroyable)))
			{
				GenericDestroyable actualDestroyable = destroyable as GenericDestroyable;
				
				bool validDestroyable = true;							
								
				if (byBaseName && (actualDestroyable.GetBaseName () != baseName))
				{
					//Debug.Log ("Invalid base name: " + actualDestroyable.GetBaseName () + ", need to be " + baseName);
					validDestroyable = false;
				}
				
				if (validDestroyable)
				{
					Object.DestroyImmediate (actualDestroyable.gameObject, true);
				}
			}
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// DestroyLast
		/// # Destroy last object in scene given seed and basename (optional)
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void DestroyLast (string seed, bool byBaseName, string baseName)
		{
			int maxIndex = -1;

			GenericDestroyable destroyableToDelete = null;

			foreach (Object destroyable in GameObject.FindObjectsOfType (typeof(GenericDestroyable)))
			{
				GenericDestroyable actualDestroyable = destroyable as GenericDestroyable;

				bool validDestroyable = true;							
				
				if (actualDestroyable.GetSeed () != seed)
				{
					validDestroyable = false;
				}

				if (byBaseName && (actualDestroyable.GetBaseName () != baseName))
				{
					validDestroyable = false;
				}

				if (validDestroyable)
				{
					if (actualDestroyable.GetIndex () > maxIndex)
					{
						maxIndex = actualDestroyable.GetIndex ();
						destroyableToDelete = actualDestroyable;
					}
				}
			}

			if (destroyableToDelete)
			{
				Object.DestroyImmediate (destroyableToDelete.gameObject, true);
			}
		}
	}

}