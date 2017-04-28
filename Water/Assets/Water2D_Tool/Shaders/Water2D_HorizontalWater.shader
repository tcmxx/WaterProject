// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Water2D/HorizontalWater" {
	Properties {
	
		_MainTex ("Fallback texture", 2D) = "black" {}
		_BumpMap ("Normals ", 2D) = "bump" {}
		_BumpWaves("Bump waves", Float) = 1.3
		_Distortion("Distortion", Float) = 0.17
		_BumpTiling ("Bump Tiling", Vector) = (0.04 ,0.04, 0.04, 0.08)
		_BumpDirection ("Bump Direction & Speed", Vector) = (1.0 ,30.0, 20.0, -20.0)
		_BaseColor ("Base color", COLOR)  = ( .17, .25, .2, 0.5)
	}

	CGINCLUDE

		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float4 normalInterpolator : TEXCOORD0;
			float3 viewInterpolator : TEXCOORD1;
			float4 bumpCoords : TEXCOORD2;
			float4 screenPos : TEXCOORD3;
			float4 grabPassPos : TEXCOORD4;
		};

		sampler2D _BumpMap;
		sampler2D _RefractionTex;
		sampler2D _ShoreTex;
		sampler2D_float _CameraDepthTexture;

		uniform float4 _RefrColorDepth;
		uniform float4 _BaseColor;

		uniform float _BumpWaves;
		uniform float _Distortion;
		uniform float4 _BumpTiling;
		uniform float4 _BumpDirection;
	
		#define VERTEX_WORLD_NORMAL i.normalInterpolator.xyz
	
		v2f vert(appdata_full v)
		{
			v2f o;
		
			half3 worldSpaceVertex = mul(unity_ObjectToWorld,(v.vertex)).xyz;
			half3 vtxForAni = (worldSpaceVertex).xzz;

			half3 nrml = half3(0,1,0);
			half3 offsets = half3(0,0,0);
		
			v.vertex.xyz += offsets;
		
			half2 tileableUv = worldSpaceVertex.xz;
		
			o.bumpCoords.xyzw = (tileableUv.xyxy + _Time.xxxx * _BumpDirection.xyzw) * _BumpTiling.xyzw;

			o.viewInterpolator.xyz =  (-90 - _WorldSpaceCameraPos);

			o.pos = UnityObjectToClipPos(v.vertex);

			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0f;
			#endif
	
			o.screenPos = ComputeScreenPos(o.pos); 
			o.grabPassPos.xy = (float2( o.pos.x, o.pos.y*scale ) + o.pos.w ) * 0.5;
			o.grabPassPos.zw = o.pos.zw;
		
			o.normalInterpolator.xyz = nrml;
			o.normalInterpolator.w = 1;
		
			return o;
		}

		half4 frag( v2f i ) : SV_Target
		{
			half3 bump = (UnpackNormal(tex2D(_BumpMap, i.bumpCoords.xy)) + UnpackNormal(tex2D(_BumpMap, i.bumpCoords.zw))) * 0.5;
			half3 worldN = VERTEX_WORLD_NORMAL + bump.xxy * _BumpWaves * half3(1,0,1);
			half3 worldNormal = normalize(worldN);
			half3 viewVector = normalize(i.viewInterpolator.xyz);

			half4 distortOffset = half4(worldNormal.xz * _Distortion * 10.0, 0, 0);
			half4 screenWithOffset = i.screenPos + distortOffset;
			half4 grabWithOffset = i.grabPassPos + distortOffset;
		
			half4 rtRefractionsNoDistort = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(i.grabPassPos));
			half refrFix = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(grabWithOffset));
			half4 rtRefractions = tex2Dproj(_RefractionTex, UNITY_PROJ_COORD(grabWithOffset));
		
			half4 edgeBlendFactors = half4(1.0, 0.0, 0.0, 0.0);
			half4 baseColor = _BaseColor;

			baseColor = lerp (lerp (rtRefractions, baseColor, baseColor.a), half4(1.0,1.0,1.0,1.0), half(0.0));
			baseColor.a = edgeBlendFactors.x;

			return baseColor;
		}
	
	ENDCG

	Subshader
	{
		Tags {"RenderType"="Transparent" "Queue"="Transparent"}
	
		Lod 500
		ColorMask RGB
	
		GrabPass { "_RefractionTex" }
	
		Pass {
				Blend SrcAlpha OneMinusSrcAlpha
				ZTest LEqual
				ZWrite Off
				Cull Off
		
				CGPROGRAM
		
				#pragma target 3.0
		
				#pragma vertex vert
				#pragma fragment frag
		
				ENDCG
		}
	}
	Fallback "Transparent/Diffuse"
}
