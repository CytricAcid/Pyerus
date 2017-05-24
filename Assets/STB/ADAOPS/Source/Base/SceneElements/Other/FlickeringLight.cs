using UnityEngine;
using System.Collections;

namespace STB.ADAOPS
{
	public class FlickeringLight : MonoBehaviour
	{
		float initialIntensity = 0;
		float counter = 0;
		float timeOff = 0;
	
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Start the class
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		void Start ()
		{
			initialIntensity = this.GetComponent<Light> ().intensity;

			if (Random.Range (0, 100) < 15)
			{
				this.GetComponent<Light> ().enabled = false;
				this.GetComponent<Light> ().intensity = 0;
			}
		}
		//////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Update the class
		/// </summary>
		//////////////////////////////////////////////////////////////////////////
		void Update ()
		{
			if (this.GetComponent<Light> ().enabled)
			{						
				//print ("timeOff: " + timeOff + "   counter: " + counter);
						

				if (counter <= timeOff)
				{
					this.GetComponent<Light> ().intensity -= 90 * Time.deltaTime;
				}
				else
				{
					this.GetComponent<Light> ().intensity += 90 * Time.deltaTime;
				}

				if (this.GetComponent<Light> ().intensity <= 0)
				{
					this.GetComponent<Light> ().intensity = 0;
				}

				if (this.GetComponent<Light> ().intensity >= initialIntensity)
				{
					this.GetComponent<Light> ().intensity = initialIntensity;
				}


				if (counter <= 0)
				{
					timeOff = Random.Range (0, 500) / 1000.0f;
					counter = Random.Range (0, 2000) / 1000.0f;
				}
				counter -= Time.deltaTime;
			}		
				
			//print (this.transform.renderer.material.name + " -> Color: " + this.transform.renderer.material.color);
			float c = 0.1f + this.GetComponent<Light> ().intensity / initialIntensity;

			if (c > 1)
			{
				c = 1;
			}

			Renderer actualRenderer = this.transform.parent.transform.GetComponent<Renderer> ();

			if (actualRenderer)
			{
				actualRenderer.material.color = new Color (c, c, c);				
			}
		}	
	}
}
