// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Toony Colors Pro+Mobile 2
// (c) 2014-2017 Jean Moreno

Shader "Shaders/Water"
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
		_MainTex2 ("Second Texture (RGB)", 2D) = "white" {}
	[TCP2Separator]
		
		//TOONY COLORS RAMP
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("Ramp Smoothing", Range(0.001,1)) = 0.1
	[TCP2Separator]
	[TCP2HeaderHelp(WATER)]
		_Color ("Water Color", Color) = (0.5,0.5,0.5,1.0)
		
		[Header(Depth Color)]
		_DepthColor ("Depth Color", Color) = (0.5,0.5,0.5,1.0)
		[PowerSlider(5.0)] _DepthDistance ("Depth Distance", Range(0.01,3)) = 0.5
		
		[Header(Foam)]
		_FoamSpread ("Foam Spread", Range(0.01,5)) = 2
		_FoamStrength ("Foam Strength", Range(0.01,1)) = 0.8
		_FoamColor ("Foam Color (RGB) Opacity (A)", Color) = (0.9,0.9,0.9,1.0)
		[NoScaleOffset]
		_FoamTex ("Foam (RGB)", 2D) = "white" {}
		_FoamSmooth ("Foam Smoothness", Range(0,0.5)) = 0.02
		_FoamSpeed ("Foam Speed", Vector) = (2,2,2,2)
		
		[Header(Vertex Waves Animation)]
		_WaveSpeed ("Speed", Float) = 2
		_WaveHeight ("Height", Float) = 0.1
		_WaveFrequency ("Frequency", Range(0,10)) = 1
		
		[Header(UV Waves Animation)]
		_UVWaveSpeed ("Speed", Float) = 1
		_UVWaveAmplitude ("Amplitude", Range(0.001,0.5)) = 0.05
		_UVWaveFrequency ("Frequency", Range(0,10)) = 1
		[Header(UV Waves Second Tex)]
		_UVWaveSpeed2 ("Speed", Float) = 0.5
		_UVWaveAmplitude2 ("Amplitude", Range(0.001,0.5)) = 0.07
		_UVWaveFrequency2 ("Frequency", Range(0,10)) = 1.5
	[TCP2Separator]
		//Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}
	
	SubShader
	{
		Tags {"Queue"="Geometry" "RenderType"="Opaque"}
		
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsWater keepalpha vertex:vert nolightmap fullforwardshadows
		#pragma target 3.0
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		float4 _MainTex_ST;
		sampler2D _MainTex2;
		float4 _MainTex2_ST;
		sampler2D_float _CameraDepthTexture;
		fixed4 _DepthColor;
		half _DepthDistance;
		half4 _FoamSpeed;
		half _FoamSpread;
		half _FoamStrength;
		sampler2D _FoamTex;
		fixed4 _FoamColor;
		half _FoamSmooth;
		half _WaveHeight;
		half _WaveFrequency;
		half _WaveSpeed;
		half _UVWaveAmplitude;
		half _UVWaveFrequency;
		half _UVWaveSpeed;
		half _UVWaveAmplitude2;
		half _UVWaveFrequency2;
		half _UVWaveSpeed2;
		
		

		struct Input
		{
			half4 texcoord;
			half3 viewDir;
			half4 sinAnim;
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
			half4 mainTexcoords = worldPos.xzxz * 0.1;
			o.texcoord.xy = TRANSFORM_TEX(mainTexcoords.xy, _MainTex);
			o.texcoord.zw = TRANSFORM_TEX(mainTexcoords.zw, _MainTex2);
			half4 x;
			x.xy = ((v.vertex.xy+v.vertex.yz) * _UVWaveFrequency) + (TIME.xx * _UVWaveSpeed);
			x.zw = ((v.vertex.xy+v.vertex.yz) * _UVWaveFrequency2) + (TIME.xx * _UVWaveSpeed2);
			o.sinAnim = x;
			//vertex waves
			float3 _pos = v.vertex.xyz * _WaveFrequency;
			float _phase = TIME * _WaveSpeed;
			half4 vsw_offsets = half4(1.0, 2.2, 0.6, 1.3);
			half4 vsw_ph_offsets = half4(1.0, 1.3, 2.2, 0.4);
			half4 waveXZ = sin((_pos.xxzz * vsw_offsets) + (_phase.xxxx * vsw_ph_offsets));
			// float waveFactorX = (waveXZ.x + waveXZ.y) * _WaveHeight / 2;
			// float waveFactorZ = (waveXZ.z + waveXZ.w) * _WaveHeight / 2;
			float waveFactorX = dot(waveXZ.xy, 1) * _WaveHeight / 2;
			float waveFactorZ = dot(waveXZ.zw, 1) * _WaveHeight / 2;
		#define VSW_STRENGTH 1
			v.vertex.y += (waveFactorX + waveFactorZ) * VSW_STRENGTH;
			float4 pos = UnityObjectToClipPos(v.vertex);
			o.sPos = ComputeScreenPos(pos);
			COMPUTE_EYEDEPTH(o.sPos.z);
		}

		//================================================================
		// SURFACE FUNCTION

		void surf(Input IN, inout SurfaceOutputWater o)
		{

			half4 uvDistort;
			uvDistort.xy = ((sin(0.9*IN.sinAnim.xy) + sin(1.33*IN.sinAnim.xy+3.14) + sin(2.4*IN.sinAnim.xy+5.3))/3) * _UVWaveAmplitude; 
			uvDistort.zw = ((sin(2.7*IN.sinAnim.zw) + sin(0.8*IN.sinAnim.zw+2.21) + sin(1.2*IN.sinAnim.zw+1.8))/3) * _UVWaveAmplitude2; 
			IN.texcoord.zw += uvDistort.zw;
			IN.texcoord.xy += uvDistort.xy;
			half ndv = saturate( dot(IN.viewDir, o.Normal) );
			fixed4 mainTex = tex2D(_MainTex, IN.texcoord.xy);
			mainTex = (mainTex + tex2D(_MainTex2, IN.texcoord.zw)) / 2;
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
			//Depth-based foam
			half2 foamUV = IN.texcoord.xy;
			foamUV.xy += TIME.xx*_FoamSpeed.xy*0.05;
			fixed4 foam = tex2D(_FoamTex, foamUV);
			foamUV.xy += TIME.xx*_FoamSpeed.zw*0.05;
			fixed4 foam2 = tex2D(_FoamTex, foamUV);
			foam = (foam + foam2) / 2;
			float foamDepth = saturate(_FoamSpread * depthDiff);
			half foamTerm = (smoothstep(foam.r - _FoamSmooth, foam.r + _FoamSmooth, saturate(_FoamStrength - foamDepth)) * saturate(1 - foamDepth)) * _FoamColor.a;
			//Alter color based on depth buffer (soft particles technique)
			mainTex.rgb = lerp(_DepthColor.rgb, mainTex.rgb, saturate(_DepthDistance * depthDiff));	//N.V corrects the result based on view direction (depthDiff tends to not look consistent depending on view angle)));
			_Color = lerp(fixed4(1,1,1,1), _Color, mainTex.a);
			o.Albedo = lerp(mainTex.rgb * _Color.rgb, _FoamColor.rgb, foamTerm);
			o.Alpha = mainTex.a * _Color.a;
			o.Alpha = lerp(o.Alpha, _FoamColor.a, foamTerm);
		}
		
		ENDCG

	}
	
	//Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector_SG"
}
