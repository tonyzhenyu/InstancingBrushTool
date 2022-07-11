Shader "URP/Effect/urp_effect_project"
{
    Properties
    {
        _Tint("Tint",Color) = (1,1,1,1)
        [NoScaleOffset][MainTex]_MainTex("MainTex" , 2D) = "white"{}
        _ClipThreshold("ClipThreshold" , Range(0,1)) = 0
        _Scale("Scale" , float) = 1
        [KeywordEnum(Sphere,Cube)] _ProjectorType("ProjectorType", float) = 0
        [Toggle(Valid)]_Valid("Valid" , int) = 0
    }
    SubShader
    {
        Tags 
        { 
            "RenderPipline" = "UniversalPipeline" 
            "RenderType" = "Opaque"
            "Queue" = "Transparent"
            "PreviewType" = "Cube"
        }

        LOD 100
        Cull front
        Blend SrcAlpha DstColor
        ZWrite off
        ZTest off

        HLSLINCLUDE
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _PROJECTORTYPE_SPHERE
            #pragma shader_feature _PROJECTORTYPE_CUBE
            #pragma shader_feature Valid

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _Tint;
            sampler2D _MainTex;
            half _ClipThreshold;
            half _Scale;
            CBUFFER_END
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                //float3 viewDir : TEXCOORD2;
                float4 screenUV : TEXCOORD3;
                UNITY_VERTEX_OUTPUT_STEREO
            };
        ENDHLSL

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            
            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                //float3 wpos = mul(UNITY_MATRIX_M, v.vertex).xyz;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

                //o.viewDir = normalize( _WorldSpaceCameraPos.xyz - wpos.xyz);
                o.screenUV = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                half4 col = _Tint;

                half2 scrUV = i.screenUV.xy / i.screenUV.w;
                half depth = SampleSceneDepth(scrUV);

                half3 rebuildWpos = ComputeWorldSpacePosition(scrUV, depth, UNITY_MATRIX_I_VP);
                half3 objecPos = mul(UNITY_MATRIX_I_M,float4(rebuildWpos ,1.0)).xyz;

                half4 tex = tex2D(_MainTex,abs(objecPos.xz) * _Scale);

                #if _PROJECTORTYPE_SPHERE
                    clip((half3)_ClipThreshold - distance(objecPos, (float3)0));
                #elif _PROJECTORTYPE_CUBE
                    clip((half3)_ClipThreshold - abs(objecPos));
                #endif
                
                #if Valid
                    col = _Tint;
                #else
                    col = half4(1,0,0,1);
                #endif

                return (half4)tex.r * col;
            }
            ENDHLSL
        }
    }
}
