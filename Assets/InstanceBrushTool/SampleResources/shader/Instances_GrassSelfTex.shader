Shader "Custom/Instances/Instances_GrassSelfTex"
{
    Properties
    {
        [MainTex][NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        [range(0,1)]_CutOff ("CutOff",range(0,1)) = 0.5
        _WindIntensity("WindIntensity" , float) = 1
        _WindPhase("WindPhase" , float) = 2
    }
    SubShader
    {
        Tags {
			"RenderType" = "Transparent"
			"RenderPipline" = "UniversalPipeline"
            
		}
        LOD 100
		Cull Off

        //UsePass "URP/urp_shadowcutoff/ShadowCaster"

        Pass
        {
            Name "ForwardLit"
            tags{
                "LightMode" = "UniversalForward"
            }
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            //#include "gamecore.inc.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Lib/Lighting.hlsl"
            #include "Lib/Quaternion.hlsl"
            //#include "Lib/Instancing.hlsl"

            //SRP Batch
            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _CutOff;
            float _WindIntensity;
            float _WindPhase;
            CBUFFER_END
            
            struct InstanceInfo
            {
                float3 position;
                float4 rotation;
                float3 localscale;
                float4 color;
            };

            StructuredBuffer<InstanceInfo> _Infosbuffer;
            
            struct appdata
            {
            
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal   : NORMAL;
                uint instanceID :SV_INSTANCEID;
                //UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                UNITY_VERTEX_INPUT_INSTANCE_ID

                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal_w : TEXCOORD1;
                float4 pos_w    : TEXCOORD3;
                float4 col : TEXCOORD4;
            };

            ///---
            /// Simulate Grass Wind Motion 
            ///---
            float4 WindMotion(float3 objectpos,float3 worldPosition , float3 worldnormal , float mask){
                //large Motion
                float mo_x = (2 * mask * sin( 1 * (_Time.z + objectpos.x + objectpos.y + objectpos.z ) * _WindPhase)) + 1;
                float mo_z = (1 * mask * sin( 2 * (_Time.z + objectpos.x + objectpos.y + objectpos.z ) * _WindPhase)) + 0.5;   
                //Displacement Part
                float3 disp =  (0.065 * sin( 2.650 * (worldPosition.x + worldPosition.y + worldPosition.z + _Time.z) * _WindPhase )) * worldnormal * float3(1 , 0.35 , 1) * mask * _WindIntensity * 2;
                
                return float4( mo_x / 10 *  _WindIntensity , 0, mo_z / 10 * _WindIntensity ,0) + float4(disp , 0);
            }

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v,o);

                InstanceInfo data = _Infosbuffer[v.instanceID];
            
                o.normal_w = normalize(mul(v.normal, (float3x3)UNITY_MATRIX_I_M));

                float3 objectpos = v.vertex.xyz;
                objectpos = objectpos * data.localscale;
                objectpos = quat_apply(objectpos.xyz,data.rotation);
                objectpos = objectpos + data.position;

                v.vertex.xyz = objectpos.xyz;

                float3 wpos = mul((float3x3)UNITY_MATRIX_M, objectpos).xyz;

                float mask = v.uv.y;
                //motion of Grass
                v.vertex += WindMotion(objectpos , wpos , o.normal_w, mask);
				wpos = mul(UNITY_MATRIX_M, v.vertex).xyz;
                o.uv = v.uv;
                o.vertex = mul(UNITY_MATRIX_VP, float4(wpos, 1.0));
				o.pos_w = float4(wpos.xyz,v.instanceID);
                o.col = data.color;
                
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i)

                half4 col = tex2D(_MainTex, i.uv);
                clip(col.a - _CutOff);
                half4 environment = _GlossyEnvironmentColor ;
                half4 light =  CalculateLambert(float3(0,1,0), i.pos_w.xyz, _WorldSpaceCameraPos) *2;
    
                half4 gammaColor = pow(saturate(i.col),2.2) * col;
                //half4 topcolor =  clamp(1,1.5,gammaColor + gammaColor  ;
                //half4 mixcol = lerp(gammaColor, topcolor, (sin(i.pos_w.w)+1)/2);

                return gammaColor.rgba * light.rgba * clamp((sin(i.pos_w.w)+1),1,1.2);
            }
            ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            tags{
                "LightMode" = "ShadowCaster"
            }
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Lib/Quaternion.hlsl"
            //#include "Lib/Instancing.hlsl"

            CBUFFER_START(UnityPerMaterial)
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform half _CutOff;
            CBUFFER_END

            //CBUFFER_START(UnityPerDraw)
            uniform float3 _LightDirection;
            uniform float4 _ShadowBias;
            uniform half4 _MainLightShadowParams;
            //CBUFFER_END
            struct InstanceInfo
            {
                float3 position;
                float4 rotation;
                float3 localscale;
                float4 color;
            };

            StructuredBuffer<InstanceInfo> _Infosbuffer;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal   : NORMAL;
                uint instanceID :SV_INSTANCEID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float3 ApplyShadowBias(float3 positionWS,float3 normalWS,float3 lightDirection)
            {
                float invNdotL = 1.0 - saturate(dot(lightDirection, normalWS));
                float scale = invNdotL * _ShadowBias.y;

                positionWS = lightDirection * _ShadowBias.xxx + positionWS;
                positionWS = normalWS * scale.xxx + positionWS;

                return positionWS;
            }

            v2f vert(appdata v)
            {
                v2f o = (v2f)0;
                UNITY_SETUP_INSTANCE_ID(v);     
                UNITY_TRANSFER_INSTANCE_ID(v,o);

                InstanceInfo data = _Infosbuffer[v.instanceID];

                float3 objectpos = v.vertex.xyz;
                objectpos = objectpos * data.localscale;
                objectpos = quat_apply(objectpos.xyz,data.rotation);
                objectpos = objectpos + data.position;

                float3 worldPos = TransformObjectToWorld(objectpos);
                half3 normalWS = TransformObjectToWorldNormal(v.normal);

                worldPos = ApplyShadowBias(worldPos,normalWS,_LightDirection);

                o.vertex = TransformWorldToHClip(worldPos);
#if UNITY_REVERSED_Z
                o.vertex.z = min(o.vertex.z , o.vertex.w * UNITY_NEAR_CLIP_VALUE);
#else
                o.vertex.z = max(o.vertex.z , o.vertex.w * UNITY_NEAR_CLIP_VALUE);
#endif
                o.uv = TRANSFORM_TEX(v.uv,_MainTex);
                return o;
            }

            real4 frag(v2f i):SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i)
                half4 col = tex2D(_MainTex,i.uv);
                clip(col.a - _CutOff);

                return 0;
            }
            ENDHLSL
        }
    }
}
