// Toony Colors Pro+Mobile 2
// (c) 2014-2017 Jean Moreno

Shader "Toony Colors Pro 2/User/TransparentOneSided"
{
	Properties
	{
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	[TCP2Separator]
		
		//TOONY COLORS RAMP
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
		_RampThresholdOtherLights ("Threshold (Other Lights)", Range(0,1)) = 0.5
		_RampSmoothOtherLights ("Smoothing (Other Lights)", Range(0.001,1)) = 0.5
	[TCP2Separator]
	
	[TCP2HeaderHelp(SUBSURFACE SCATTERING, Subsurface Scattering)]
		_SSDistortion ("Distortion", Range(0,2)) = 0.2
		_SSPower ("Power", Range(0.1,16)) = 3.0
		_SSScale ("Scale", Float) = 1.0
	[TCP2Separator]
	
	[TCP2HeaderHelp(TRANSPARENCY)]
		//Blending
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendTCP2 ("Blending Source", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlendTCP2 ("Blending Dest", Float) = 10
		//Alpha Testing
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
		
		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend [_SrcBlendTCP2] [_DstBlendTCP2]
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom addshadow keepalpha
		#pragma target 3.0
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		half _SSDistortion;
		half _SSPower;
		half _SSScale;
		fixed _Cutoff;
		
		struct Input
		{
			half2 uv_MainTex;
			float vFace : VFACE;
		};
		
		//================================================================
		// CUSTOM LIGHTING
		
		//Lighting-related variables
		fixed4 _HColor;
		fixed4 _SColor;
		float _RampThreshold;
		float _RampSmooth;
		float _RampThresholdOtherLights;
		float _RampSmoothOtherLights;
		
		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Gloss;
			fixed Alpha;
			float vFace;
		};
		
		inline half4 LightingToonyColorsCustom (inout SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten)
		{
			s.Normal = normalize(s.Normal);
			s.Normal.z *= s.vFace;
			fixed ndl = max(0, dot(s.Normal, lightDir));
		#if defined(UNITY_PASS_FORWARDBASE)
			fixed3 ramp = smoothstep(_RampThreshold-_RampSmooth*0.5, _RampThreshold+_RampSmooth*0.5, ndl);
		#else
			fixed3 ramp = smoothstep(_RampThresholdOtherLights-_RampSmoothOtherLights*0.5, _RampThresholdOtherLights+_RampSmoothOtherLights*0.5, ndl);
		#endif
		#if !(POINT) && !(SPOT)
			ramp *= atten;
		#endif
			_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
			ramp = lerp(_SColor.rgb, _HColor.rgb, ramp);
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp;
			c.a = s.Alpha;
		#if (POINT || SPOT)
			c.rgb *= atten;
		#endif
			//Subsurface Scattering
			half3 ssLight = lightDir + s.Normal * _SSDistortion;
			half ssDot = pow(saturate(dot(viewDir, -ssLight)), _SSPower) * _SSScale;
		  #if (POINT || SPOT)
			half ssAtten = atten * 2;
		  #else
			half ssAtten = 1;
		  #endif
			half3 ssColor = ssAtten * ssDot;
			c.rgb += s.Albedo * ssColor;
			return c;
		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;
			
			//Cutout (Alpha Testing)
			clip (o.Alpha - _Cutoff);
			
			//VFace Register (backface lighting)
			o.vFace = IN.vFace;
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
