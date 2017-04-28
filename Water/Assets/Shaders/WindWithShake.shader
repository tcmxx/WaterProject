// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "TCShaders/WindWithShake" {
	//adapted from https://forum.unity3d.com/threads/shader-moving-trees-grass-in-wind-outside-of-terrain.230911/

	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_Illum("Illumin (A)", 2D) = "black" {}
	_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_ShakeWindspeed("Shake Windspeed", Float) = 1.0
		_ShakingWindX("Shake strength X ", Float) = 1.0
		_ShakingWindZ("Shake strength Z", Float) = 1.0
		_PlantYFactors("Plant Y Factors", Vector) = (0,0,0,0)
		_MainWindX("Main Wind X strength", Float) = 0
		_MainWindZ("Main Wind Z strength", Float) = 0
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
	float _ShakeWindspeed;
	float4 _PlantYFactors;
	float _MainWindX;
	float _MainWindZ;
	float _ShakingWindX;
	float _ShakingWindZ;
	float _IntialTimeFactor;
	struct Input {
		float2 uv_MainTex;
		float2 uv_Illum;
	};

	void vert(inout appdata_full v) {

		const float _WindSpeed = (_ShakeWindspeed);

		const float4 waveSpeed = float4 (1, 1.4, 2, 2.8);
		float4 _waveXmove = float4(0.03, 0.06, 0.04, 0.12);
		float4 _waveZmove = float4 (0.02, 0.05, 0.06, 0.1);
		float3 offset = mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
		float4 waves = (_Time.x + (offset.y + offset.z + offset.x)*_IntialTimeFactor)  * waveSpeed *_WindSpeed;
		float4 s = sin(waves);

		//float waveAmount = v.texcoord.y * (v.color.a + _ShakeBending);
		float yDist = abs(v.vertex.y - _PlantYFactors.w);
		float signY = sign(v.vertex.y - _PlantYFactors.w);
		float mainAmount = (yDist*_PlantYFactors.x + yDist* yDist*_PlantYFactors.y
			+ yDist* yDist*yDist*_PlantYFactors.z);
		s *= mainAmount*signY;

		float3 waveMove = float3 (0,0,0);
		waveMove.x = dot(s, _waveXmove)*_ShakingWindX;
		waveMove.z = dot(s, _waveZmove)*_ShakingWindZ;

		waveMove += float4(_MainWindX, 0,_MainWindZ,0)*mainAmount*signY;
		v.vertex.xz -= waveMove.xz;//use local wind
		float sqrxz = sqrt(waveMove.x*waveMove.x + waveMove.z*waveMove.z);
		v.vertex.y = cos(abs(asin(clamp(sqrxz,0, yDist*0.8) / yDist)))*yDist*signY + _PlantYFactors.w - clamp(sqrxz - 0.8*yDist,0, yDist)*signY;
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