Shader "PK2/Player/Specular-BRDF3" {
	Properties {
		[Header(Albedo)] _MainTex ("Albedo(RGB)", 2D) = "white" {}
		_AlbedoIntensity ("Albedo Intensity", Range(0, 10)) = 1
		_Color ("Color", Vector) = (1,1,1,1)
		[Space] _EyesTex ("EyesTexture", 2D) = "white" {}
		_MouthTex ("MouthTexture", 2D) = "white" {}
		_TattooTex ("TattooTexture", 2D) = "white" {}
		[Space] [Header(Specular)] [NoScaleOffset] _SpecularMap ("Specualr Map(R:Metallic G:Glossiness B:SSS)", 2D) = "white" {}
		_Metallic ("Metallic", Range(0, 1)) = 0.5
		_Smoothness ("_Smoothness", Range(0, 1)) = 0.5
		_SpecualrColor ("Specualr Color", Vector) = (1,1,1,1)
		[Space] [Header(Normal)] [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "normal" {}
		_BumpScale ("Bump Scale", Range(0, 2)) = 1
		[Space] [Header(Rim)] _RimColor ("Rim Color", Vector) = (1,1,1,1)
		_RimColorPower ("Rim Power", Range(0, 10)) = 10
		_RimColorIntensity ("Rim Intensity", Range(0, 10)) = 0
		[Space] [Header(Reflection)] [NoScaleOffset] _ReflectionTex ("Reflection(Cube)", Cube) = "_Skybox" {}
		_EnvIntensity ("Env Intensity", Range(0, 20)) = 0
		[Space] [Header(Emission)] _Emission ("Emission", Range(1, 10)) = 1
		_EmissionColor ("Emission Color", Vector) = (1,1,1,1)
		[Space] [Header(SSS)] _sssWrap ("SSS Wrap", Range(0, 1)) = 0
		_sssColor ("SSS Color", Vector) = (0.5,0.5,0.5,1)
		_sssIntensity ("SSS Intensity", Range(0, 10)) = 0
		[Space] [Header(Light)] _AmbientColor ("Ambient Light Color", Vector) = (0.5,0.5,0.5,0.5)
		_Minimum ("Minimum", Range(0, 1)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "PK2/CastShadow/Cast-Shadow-Standard-s-PlayerHead"
}