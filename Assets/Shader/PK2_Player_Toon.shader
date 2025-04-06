Shader "PK2/Player/Toon" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Diffuse", 2D) = "white" {}
		_Ambientlight ("Ambient light", Range(0, 2)) = 0.75
		_MetallicGlossMap ("Metallic (Glosiness)", 2D) = "white" {}
		_Metallic ("Metallic", Range(0, 5)) = 4
		_Glosiness ("Glosiness", Range(0, 5)) = 1
		_ScanLineTex ("ScanLineTex", 2D) = "black" {}
		_ScanLineOffset ("ScanLineOffset", Float) = 0
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
}