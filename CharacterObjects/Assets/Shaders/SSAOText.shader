﻿Shader ".ShaderExample/SSAOTest" 
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DepthTextureSampler("Depth Texture", 2D) = "black" {}
        _RandomTextureSampler("Random Texture", 2D) = "grey" {}
    }
   
 
   
   
    SubShader {
        Cull Off ZWrite Off
        pass {
//      blend One SrcColor
            CGPROGRAM
           
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment ps_ssao
            #pragma target 3.0
           
            sampler2D _MainTex;
            sampler2D _DepthTextureSampler;
            sampler2D _RandomTextureSampler;
           
            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
           
            v2f vert (appdata_img v)
            {
                v2f o;
               
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
 
                o.uv = v.texcoord.xy;
               
                return o;
            }
 
            float3 normal_from_depth(float depth, float2 texcoords) {
                const float2 offset1 = float2(0.0,0.001);
                const float2 offset2 = float2(0.001,0.0);
 
                float depth1 = tex2D(_DepthTextureSampler, texcoords + offset1).r;
                float depth2 = tex2D(_DepthTextureSampler, texcoords + offset2).r;
 
                float3 p1 = float3(offset1, depth1 - depth);
                float3 p2 = float3(offset2, depth2 - depth);
 
                float3 normal = cross(p1, p2);
                normal.z = -normal.z;
 
                return normalize(normal);
            }
           
           
 
           
            float4 ps_ssao(v2f In) : COLOR
            {
                float4 Output;
 
                const float total_strength = 1.0;
                const float base = 0.2;
 
                const float area = 0.0075;
                //const float area = 2.75;
 
                const float falloff = 0.000001;
 
                const float radius = 0.0002;
 
                const int samples = 16;
                const float3 sample_sphere[16] = {
                    float3( 0.5381, 0.1856,-0.4319), float3( 0.1379, 0.2486, 0.4430),
                    float3( 0.3371, 0.5679,-0.0057), float3(-0.6999,-0.0451,-0.0019),
                    float3( 0.0689,-0.1598,-0.8547), float3( 0.0560, 0.0069,-0.1843),
                    float3(-0.0146, 0.1402, 0.0762), float3( 0.0100,-0.1924,-0.0344),
                    float3(-0.3577,-0.5301,-0.4358), float3(-0.3169, 0.1063, 0.0158),
                    float3( 0.0103,-0.5869, 0.0046), float3(-0.0897,-0.4940, 0.3287),
                    float3( 0.7119,-0.0154,-0.0918), float3(-0.0533, 0.0596,-0.5411),
                    float3( 0.0352,-0.0631, 0.5460), float3(-0.4776, 0.2847,-0.0271)
                    };
 
                float3 random = normalize( tex2D(_RandomTextureSampler, In.uv/*In.Tex0 * 4.0)*/).rgb );
 
                float depth = tex2D(_DepthTextureSampler, In.uv).r;
 
                float3 position = float3(In.uv, depth);
                float3 normal = normal_from_depth(depth, In.uv);
 
                float radius_depth = radius/depth;
                float occlusion = 0.0;
               
                for(int i=0; i < samples; i++) {
                    float3 ray = radius_depth * reflect(sample_sphere[i], random);
                    float3 hemi_ray = position + sign(dot(ray,normal)) * ray;
 
                    float occ_depth = tex2D(_DepthTextureSampler, saturate(hemi_ray.xy)).r;
                    float difference = depth - occ_depth;
 
                    occlusion += step(falloff, difference) * (1.0-smoothstep(falloff, area, difference));
                }
 
                float ao = 1.0 - total_strength * occlusion * (1.0 / samples);
                Output.rgb = saturate(ao + base);
 
                return Output;
            }
           
            ENDCG
        }
    }
}