using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace STB.ADAOPS
{
	public class RainManager : MonoBehaviour
	{
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Class: RainManager
		/// # To handle rain and rain splash
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		 
		// public
		public Transform splashPrefab = null;

		// private
		float energyLimit = 100.0f;


		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// LateUpdate
		/// # Update the rain
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		void LateUpdate ()
		{
			List<Particle> particleList = GetComponent<ParticleEmitter> ().particles.ToList ();

			int[] livingParticleList = new int[particleList.Count];

			int particlesToKeep = 0;

			for (var i = 0; i < GetComponent<ParticleEmitter>().particleCount; i++)
			{
				Particle actualParticle = particleList [i];

				if (actualParticle.energy > energyLimit)
				{
					actualParticle.color = Color.yellow;

					particleList [i] = actualParticle;

					if (splashPrefab)
					{
						GameObject rainContainer = BasicFunctions.CreateContainerIfNotExists ("_RAIN");

						Transform actualSplash = Instantiate (splashPrefab, particleList [i].position, Quaternion.identity) as Transform;

						if (actualSplash)
						{
							actualSplash.parent = rainContainer.transform;
						}
					}
				}
				else
				{
					livingParticleList [particlesToKeep++] = i;
				}
			}

			Particle[] keepParticles = new Particle[particlesToKeep];

			for (var j = 0; j < particlesToKeep; j++)
				keepParticles [j] = particleList [livingParticleList [j]];

			GetComponent<ParticleEmitter> ().particles = keepParticles;
		}	
	}
}