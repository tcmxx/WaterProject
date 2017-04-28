// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "TCShaders/SineShake" {
	//adapted from https://forum.unity3d.com/threads/shader-moving-trees-grass-in-wind-outside-of-terrain.230911/

	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
		_Illum("Illumin (A)", 2D) = "black" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_FrequencyAmplitudeXZ("Frequency And Amplitude for XZ", Float) = (0,0,0,0)
		_PhaseRateXZ("Phase and Y based Amp increase rate for XZ", Vector) = (0,0,0,0)
		_YPhaseRateX("phase based on Y for X", Float) = 0
		_YPhaseRateZ("phase based on Y for Z", Float) = 0
		_YOffset("Y offset of root", Float) = 0
		_IntialTimeFactor("initial time factor", Float) = 0.1
	}

		SubShader{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
#pragma target 3.0
#pragma surface surf Lambert alphatest:_Cutoff vertex:vert addshadow

		sampler2D _MainTex;
	sampler2D _Illum;
	fixed4 _Color;
	float4 _FrequencyAmplitudeXZ;
	float4 _PhaseRateXZ;
	float _YOffset;
	float _IntialTimeFactor;
	float _YPhaseRateX;
	float _YPhaseRateZ;

	struct Input {
		float2 uv_MainTex;
		float2 uv_Illum;
	};

	void vert(inout appdata_full v) {

		
		float y = v.vertex.y - _YOffset;

		float2 amp = clamp(y*_PhaseRateXZ.zw, -1, 1)*_FrequencyAmplitudeXZ.zw;
		float3 offset = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
		float2 waves = amp*sin(_PhaseRateXZ.xy + float2(y* _YPhaseRateX, y* _YPhaseRateZ) + _FrequencyAmplitudeXZ.xy*(_Time.x + (offset.y + offset.z + offset.x)*_IntialTimeFactor));

		//v.vertex.xz -= waves.xy;//use local wind
		v.vertex.x += 5* cos(v.vertex.z*_YPhaseRateX);
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).a;
		o.Alpha = c.a;
	}
	ENDCG
	}

		Fallback "Transparent/Cutout/VertexLit"
}