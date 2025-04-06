Shader "MK/Toon/Free" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Color (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_LightThreshold ("LightThreshold", Range(0.01, 1)) = 0.3
		_LightSmoothness ("Light Smoothness", Range(0, 1)) = 0
		_RimSmoothness ("Rim Smoothness", Range(0, 1)) = 0.5
		_ShadowColor ("Shadow Color", Vector) = (0,0,0,1)
		_HighlightColor ("Highlight Color", Vector) = (1,1,1,1)
		_ShadowIntensity ("Shadow Intensity", Range(0, 2)) = 1
		_OutlineColor ("Outline Color", Vector) = (0,0,0,1)
		_OutlineSize ("Outline Size", Float) = 0.02
		_RimColor ("Rim Color", Vector) = (1,1,1,1)
		_RimSize ("Rim Size", Range(0, 3)) = 1.5
		_RimIntensity ("Intensity", Range(0, 1)) = 0.5
		_Shininess ("Shininess", Range(0.01, 1)) = 0.275
		_SpecColor ("Specular Color", Vector) = (1,1,1,0.5)
		_SpecularIntensity ("Intensity", Range(0, 1)) = 0.5
		_EmissionColor ("Emission Color", Vector) = (0,0,0,1)
		[HideInInspector] _MKEditorShowMainBehavior ("Main Behavior", Float) = 1
		[HideInInspector] _MKEditorShowDetailBehavior ("Detail Behavior", Float) = 0
		[HideInInspector] _MKEditorShowLightBehavior ("Light Behavior", Float) = 0
		[HideInInspector] _MKEditorShowShadowBehavior ("Shadow Behavior", Float) = 0
		[HideInInspector] _MKEditorShowRenderBehavior ("Render Behavior", Float) = 0
		[HideInInspector] _MKEditorShowSpecularBehavior ("Specular Behavior", Float) = 0
		[HideInInspector] _MKEditorShowTranslucentBehavior ("Translucent Behavior", Float) = 0
		[HideInInspector] _MKEditorShowRimBehavior ("Rim Behavior", Float) = 0
		[HideInInspector] _MKEditorShowReflectionBehavior ("Reflection Behavior", Float) = 0
		[HideInInspector] _MKEditorShowDissolveBehavior ("Dissolve Behavior", Float) = 0
		[HideInInspector] _MKEditorShowOutlineBehavior ("Outline Behavior", Float) = 0
		[HideInInspector] _MKEditorShowSketchBehavior ("Sketch Behavior", Float) = 0
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
	Fallback "Hidden/MK/Toon/FreeMobile"
	//CustomEditor "MK.Toon.MKToonFreeEditor"
}