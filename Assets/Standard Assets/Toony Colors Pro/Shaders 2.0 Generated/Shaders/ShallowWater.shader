// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Toony Colors Pro+Mobile 2
// (c) 2014-2017 Jean Moreno

Shader "Shaders/ShallowWater"
{
	Properties
	{
		[TCP2HelpBox(Warning,Make sure that the Camera renders the depth texture for this material to work properly.    You can use the script __TCP2_CameraDepth__ for this.)]
	[TCP2HeaderHelp(BASE, Base Properties)]
		//TOONY COLORS
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
	[TCP2Separator]
		
		//TOONY COLORS RAMP
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
	[TCP2Separator]
	[TCP2HeaderHelp(WATER)]
		_Color ("Water Color (RGB) Opacity (A)", Color) = (0.5,0.5,0.5,1.0)
		
		[Header(Depth Color)]
		_DepthColor ("Depth Color", Color) = (0.5,0.5,0.5,1.0)
		[PowerSlider(5.0)] _DepthDistance ("Depth Distance", Range(0.01,3)) = 0.5
		[Header(Depth based Transparency)]
		[PowerSlider(5.0)] _DepthAlpha ("Depth Alpha", Range(0.01,10)) = 0.5
		_DepthMinAlpha ("Depth Min Alpha", Range(0,1)) = 0.5
		
		[Header(UV Waves Animation)]
		_UVWaveSpeed ("Speed", Float) = 1
		_UVWaveAmplitude ("Amplitude", Range(0.001,0.5)) = 0.05
		_UVWaveFrequency ("Frequency", Range(0,10)) = 1
	[TCP2Separator]
	[TCP2HeaderHelp(TRANSPARENCY)]
		//Blending
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendTCP2 ("Blending Source", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlendTCP2 ("Blending Dest", Float) = 10
	[TCP2Separator]
		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Blend [_SrcBlendTCP2] [_DstBlendTCP2]
		
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsWater keepalpha vertex:vert nolightmap fullforwardshadows
		#pragma target 3.0
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D_float _CameraDepthTexture;
		fixed4 _DepthColor;
		half _DepthDistance;
		half _DepthAlpha;
		fixed _DepthMinAlpha;
		half _UVWaveAmplitude;
		half _UVWaveFrequency;
		half _UVWaveSpeed;
		
		

		struct Input
		{
			half2 texcoord;
			half3 viewDir;
			float4 sPos;
		};
		
		//================================================================
		// CUSTOM LIGHTING
		
		//Lighting-related variables
		half4 _HColor;
		half4 _SColor;
		float _RampThreshold;
		float _RampSmooth;
		
		//Custom SurfaceOutput
		struct SurfaceOutputWater
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			fixed Alpha;
		};
		
		inline half4 LightingToonyColorsWater (inout SurfaceOutputWater s, half3 lightDir, half3 viewDir, half atten)
		{
			s.Normal = normalize(s.Normal);
			fixed ndl = max(0, dot(s.Normal, lightDir));
			fixed3 ramp = smoothstep(_RampThreshold-_RampSmooth*0.5, _RampThreshold+_RampSmooth*0.5, ndl);
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
			return c;
		}

		//================================================================
		// VERTEX FUNCTION
		
		
		struct appdata_tcp2
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
		};
		
			#define TIME (_Time.y)
		
		void vert(inout appdata_tcp2 v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			//Main texture UVs
			float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
			half2 mainTexcoords = worldPos.xz * 0.1;
			o.texcoord.xy = TRANSFORM_TEX(mainTexcoords.xy, _MainTex);
			half2 x = ((v.vertex.xy+v.vertex.yz) * _UVWaveFrequency) + (TIME.xx * _UVWaveSpeed);
			half2 uvDistort = ((sin(0.9*x) + sin(1.33*x+3.14) + sin(2.4*x+5.3))/3) * _UVWaveAmplitude; 
			o.texcoord.xy += uvDistort.xy;
			float4 pos = UnityObjectToClipPos(v.vertex);
			o.sPos = ComputeScreenPos(pos);
			COMPUTE_EYEDEPTH(o.sPos.z);
		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input IN, inout SurfaceOutputWater o)
		{
			half ndv = saturate( dot(IN.viewDir, o.Normal) );
			fixed4 mainTex = tex2D(_MainTex, IN.texcoord.xy);
			float sceneZ = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.sPos));
			if(unity_OrthoParams.w > 0)
			{
				//orthographic camera
			#if defined(UNITY_REVERSED_Z)
				sceneZ = 1.0f - sceneZ;
			#endif
				sceneZ = (sceneZ * _ProjectionParams.z) + _ProjectionParams.y;
			}
			else
				//perspective camera
				sceneZ = LinearEyeDepth(sceneZ);
			float partZ = IN.sPos.z;
			float depthDiff = (sceneZ - partZ);
			depthDiff *= ndv * 2;
			//Alter color based on depth buffer (soft particles technique)
			mainTex.rgb = lerp(_DepthColor.rgb, mainTex.rgb, saturate(_DepthDistance * depthDiff));	//N.V corrects the result based on view direction (depthDiff tends to not look consistent depending on view angle)));
			o.Albedo = mainTex.rgb * _Color.rgb;
			_Color.a *= saturate((_DepthAlpha * depthDiff) + _DepthMinAlpha);
			o.Alpha = mainTex.a * _Color.a;
		}
		
		ENDCG

	}
	
	//Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
