Shader "Custom/GlitchURP"
{
    Properties {
        _Amount ("Amount", Range(0,1)) = 0
        _Speed ("Speed", Range(0,10)) = 3
        _RGBSplit ("RGB Split", Range(0,5)) = 1
        _Blockiness ("Blockiness", Range(1,200)) = 40
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Cull Off

        Pass
        {
            Name "GlitchURP"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture); SAMPLER(sampler_BlitTexture);
            float4 _BlitTexture_TexelSize;

            float _Amount, _Speed, _RGBSplit, _NoiseIntensity, _Blockiness;

            struct Varyings { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };

            Varyings Vert(uint id:SV_VertexID)
            {
                Varyings o;
                float2 pos = (id==0)? float2(-1,-1) : (id==1)? float2( 3,-1) : float2(-1, 3);
                o.positionCS = float4(pos, 0, 1);

                float2 uv = pos * 0.5 + 0.5;

                #if UNITY_UV_STARTS_AT_TOP
                    uv.y = 1.0 - uv.y;
                #endif

                o.uv = uv;
                return o;
            }

            float hash21(float2 p){
                p = frac(p*float2(123.34,345.45));
                p += dot(p,p+34.345);
                return frac(p.x*p.y);
            }

            float2 glitchUV(float2 uv, float t){
                if (_Amount <= 0.0001) return uv;
                float rows = max(1.0, _Blockiness);
                float yBlock = floor(uv.y * rows) / rows;
                float j = (hash21(float2(yBlock, floor(t*_Speed))) - 0.5) * _Amount * 0.2;
                float tear = step(0.96, hash21(float2(yBlock, floor(t*_Speed*0.25))));
                j += tear * (hash21(float2(yBlock*13.7, 7.1)) - 0.5) * _Amount * 0.6;
                uv.x = saturate(uv.x + j);
                return uv;
            }

            float3 addNoise(float2 uv, float t, float3 col){
                float n = hash21(uv*float2(1024,768) + t*13.37);
                return lerp(col, col + (n-0.5), _NoiseIntensity*_Amount);
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float t = _Time.y;

                float2 baseUV = glitchUV(i.uv, t);

                float off = _RGBSplit * _Amount * 0.002;
                float3 col;
                col.r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, baseUV + float2( off, 0)).r;
                col.g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, baseUV).g;
                col.b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, baseUV + float2(-off, 0)).b;

                col = addNoise(baseUV, t, col);
                return float4(col,1);
            }
            ENDHLSL
        }
    }
}