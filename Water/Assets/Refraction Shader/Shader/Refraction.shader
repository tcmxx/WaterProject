// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

// Shader created with Shader Forge Beta 0.17 
// Shader Forge (c) Joachim 'Acegikmo' Holmer
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.17;sub:START;pass:START;ps:lgpr:1,nrmq:0,limd:1,blpr:5,bsrc:3,bdst:7,culm:2,dpts:2,wrdp:True,uamb:False,mssp:True,ufog:False,aust:False,igpj:False,qofs:0,lico:1,qpre:3,flbk:,rntp:2,lmpd:False,lprd:True,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300;n:type:ShaderForge.SFN_Final,id:0,x:32895,y:32419|diff-294-OUT,spec-325-OUT,gloss-327-OUT,normal-383-OUT,alpha-369-OUT,refract-14-OUT;n:type:ShaderForge.SFN_Slider,id:13,x:34168,y:32706,ptlb:Refraction Intensity,min:0,cur:0.3007519,max:1;n:type:ShaderForge.SFN_Multiply,id:14,x:33601,y:32724|A-16-OUT,B-220-OUT;n:type:ShaderForge.SFN_ComponentMask,id:16,x:33781,y:32647,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-313-OUT;n:type:ShaderForge.SFN_Tex2d,id:25,x:34339,y:32503,ptlb:Refraction,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:2,isnm:False|UVIN-222-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:26,x:34731,y:32429,uv:0;n:type:ShaderForge.SFN_Fresnel,id:217,x:33996,y:32077|NRM-343-OUT,EXP-345-OUT;n:type:ShaderForge.SFN_ConstantLerp,id:219,x:33784,y:32090,a:0.1,b:0.1|IN-217-OUT;n:type:ShaderForge.SFN_Multiply,id:220,x:33781,y:32791|A-13-OUT,B-221-OUT;n:type:ShaderForge.SFN_Vector1,id:221,x:34085,y:32831,v1:0.2;n:type:ShaderForge.SFN_Panner,id:222,x:34528,y:32441,spu:0,spv:1|UVIN-26-UVOUT,DIST-234-OUT;n:type:ShaderForge.SFN_Time,id:233,x:34809,y:32563;n:type:ShaderForge.SFN_Multiply,id:234,x:34625,y:32605|A-233-T,B-235-OUT;n:type:ShaderForge.SFN_ValueProperty,id:235,x:34790,y:32741,ptlb:Speed,v1:1;n:type:ShaderForge.SFN_VertexColor,id:240,x:33781,y:33098;n:type:ShaderForge.SFN_Multiply,id:241,x:33617,y:32990|A-278-OUT,B-240-A;n:type:ShaderForge.SFN_Tex2d,id:246,x:34339,y:32290,ptlb:Main Texture,tex:3a5a96df060a5cf4a9cc0c59e13486b7,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:247,x:34149,y:32334|A-246-RGB,B-25-RGB;n:type:ShaderForge.SFN_ComponentMask,id:278,x:33781,y:32942,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-313-OUT;n:type:ShaderForge.SFN_Multiply,id:294,x:33409,y:32197|A-369-OUT,B-219-OUT;n:type:ShaderForge.SFN_Multiply,id:313,x:33798,y:32461|A-247-OUT,B-13-OUT;n:type:ShaderForge.SFN_Vector1,id:325,x:33426,y:32413,v1:5;n:type:ShaderForge.SFN_Vector1,id:327,x:33426,y:32472,v1:0.6;n:type:ShaderForge.SFN_LightVector,id:343,x:34226,y:31957;n:type:ShaderForge.SFN_Vector1,id:345,x:34243,y:32111,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:368,x:33567,y:33191,ptlb:Transparence,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:369,x:33469,y:32880|A-241-OUT,B-368-OUT;n:type:ShaderForge.SFN_Multiply,id:383,x:33189,y:32523|A-313-OUT,B-369-OUT;proporder:246-13-25-235-368;pass:END;sub:END;*/

Shader "Langvv/Refraction" {
    Properties {
        _MainTexture ("Main Texture", 2D) = "white" {}
        _RefractionIntensity ("Refraction Intensity", Range(0, 1)) = 0
        _Refraction ("Refraction", 2D) = "black" {}
        _Speed ("Speed", Float ) = 1
        _Transparence ("Transparence", Float ) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _RefractionIntensity;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            uniform float _Speed;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float _Transparence;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 shLight : TEXCOORD0;
                float4 uv0 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                float3 tangentDir : TEXCOORD4;
                float3 binormalDir : TEXCOORD5;
                float4 screenPos : TEXCOORD6;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(7,8)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
                o.shLight = ShadeSH9(float4(v.normal * 1.0,1)) * 0.5;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_233 = _Time + _TimeEditor;
                float node_13 = _RefractionIntensity;
                float3 node_313 = ((tex2D(_MainTexture,TRANSFORM_TEX(i.uv0.rg, _MainTexture)).rgb*tex2D(_Refraction,TRANSFORM_TEX((i.uv0.rg+(node_233.g*_Speed)*float2(0,1)), _Refraction)).rgb)*node_13);
                float node_369 = ((node_313.r*i.vertexColor.a)*_Transparence);
                float3 normalLocal = (node_313*node_369);
                float3 normalDirection =  mul( normalLocal, tangentTransform );
                
                float nSign = sign( dot( viewDirection, normalDirection ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(0.6*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_325 = 5.0;
                float3 specularColor = float3(node_325,node_325,node_325);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float node_294 = (node_369*lerp(0.1,0.1,pow(1.0-max(0,dot(lightDirection, viewDirection)),1.0)));
                float3 finalColor = ( diffuse + i.shLight ) * float3(node_294,node_294,node_294) + specular;
/// Final Color:
                return fixed4(lerp(tex2D(_GrabTexture, float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (node_313.rg*(node_13*0.2))).rgb, finalColor,node_369),1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _GrabTexture;
            uniform float4 _TimeEditor;
            uniform float _RefractionIntensity;
            uniform sampler2D _Refraction; uniform float4 _Refraction_ST;
            uniform float _Speed;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform float _Transparence;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(6,7)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.pos;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_233 = _Time + _TimeEditor;
                float node_13 = _RefractionIntensity;
                float3 node_313 = ((tex2D(_MainTexture,TRANSFORM_TEX(i.uv0.rg, _MainTexture)).rgb*tex2D(_Refraction,TRANSFORM_TEX((i.uv0.rg+(node_233.g*_Speed)*float2(0,1)), _Refraction)).rgb)*node_13);
                float node_369 = ((node_313.r*i.vertexColor.a)*_Transparence);
                float3 normalLocal = (node_313*node_369);
                float3 normalDirection =  mul( normalLocal, tangentTransform );
                
                float nSign = sign( dot( viewDirection, normalDirection ) ); // Reverse normal if this is a backface
                i.normalDir *= nSign;
                normalDirection *= nSign;
                
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(0.6*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float node_325 = 5.0;
                float3 specularColor = float3(node_325,node_325,node_325);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float node_294 = (node_369*lerp(0.1,0.1,pow(1.0-max(0,dot(lightDirection, viewDirection)),1.0)));
                float3 finalColor = diffuse * float3(node_294,node_294,node_294) + specular;
/// Final Color:
                return fixed4(finalColor * node_369,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
