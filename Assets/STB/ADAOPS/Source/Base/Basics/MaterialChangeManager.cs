#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace STB.ADAOPS
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Class: MaterialChangeManager
	/// # When a new standar material is created it needs to set some values
	/// </summary>
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class MaterialChangeManager : MonoBehaviour
	{
		// public
		public enum BlendMode
		{
			Opaque,
			Cutout,
			Fade,		// Old school alpha-blending mode, fresnel does not affect amount of transparency
			Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
		}

		
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// MaterialChanged
		/// # Set the values
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void MaterialChanged (Material material)
		{
			// Clamp EmissionScale to always positive
			if (material.GetFloat ("_EmissionScaleUI") < 0.0f)
			{
				material.SetFloat ("_EmissionScaleUI", 0.0f);
			}
			
			// Apply combined emission value
			Color emissionColorOut = EvalFinalEmissionColor (material);
			material.SetColor ("_EmissionColor", emissionColorOut);
			
			// Handle Blending modes
			SetupMaterialWithBlendMode (material, (BlendMode)material.GetFloat ("_Mode"));
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// EvalFinalEmissionColor
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static Color EvalFinalEmissionColor (Material material)
		{
			return material.GetColor ("_EmissionColorUI") * material.GetFloat ("_EmissionScaleUI");
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// SetupMaterialWithBlendMode
		/// # Setup the new material with blend mode
		/// </summary>
		///////////////////////////////////////////////////////////////////////////////////////////////////////
		static void SetupMaterialWithBlendMode (Material material, BlendMode blendMode)
		{
			switch (blendMode)
			{
			case BlendMode.Opaque:
				material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt ("_ZWrite", 1);
				material.DisableKeyword ("_ALPHATEST_ON");
				material.DisableKeyword ("_ALPHABLEND_ON");
				material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = -1;
				break;

			case BlendMode.Cutout:
				material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
				material.SetInt ("_ZWrite", 1);
				material.EnableKeyword ("_ALPHATEST_ON");
				material.DisableKeyword ("_ALPHABLEND_ON");
				material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 2450;
				break;

			case BlendMode.Fade:
				material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt ("_ZWrite", 0);
				material.DisableKeyword ("_ALPHATEST_ON");
				material.EnableKeyword ("_ALPHABLEND_ON");
				material.DisableKeyword ("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;

			case BlendMode.Transparent:
				material.SetInt ("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				material.SetInt ("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				material.SetInt ("_ZWrite", 0);
				material.DisableKeyword ("_ALPHATEST_ON");
				material.DisableKeyword ("_ALPHABLEND_ON");
				material.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
				material.renderQueue = 3000;
				break;
			}
		}
	}
}
#endif
