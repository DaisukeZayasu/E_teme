// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/SignFontShader2" {

	Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Font Texture", 2D) = "white" {}
    }
    SubShader {
        Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}
        Pass {
            Tags {"LightMode" = "ForwardBase"}     
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            struct vertex_input {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord  : TEXCOORD0;
            };

            struct vertex_output {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float3 lightDir : TEXCOORD1;
                float3 normal   : TEXCOORD2;
                LIGHTING_COORDS(3, 4)
                float3 vertexLighting : TEXCOORD5;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _LightColor0;


            vertex_output vert(vertex_input v) {
                vertex_output o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                //o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.uv = v.texcoord.xy;
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

                o.lightDir = ObjSpaceLightDir(v.vertex);
                o.normal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);

                o.vertexLighting = float3(0.0, 0.0, 0.0);

                #ifdef VERTEXLIGHT_ON

                float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

                for (int index = 0; index < 4; index++) {    
                   float4 lightPosition = float4(unity_4LightPosX0[index], unity_4LightPosY0[index], unity_4LightPosZ0[index], 1.0);
                   float3 vertexToLightSource = float3(lightPosition - worldPos);        
                   float3 lightDirection = normalize(vertexToLightSource);
                   float squaredDistance = dot(vertexToLightSource, vertexToLightSource);
                   float attenuation = 1.0 / (1.0  + unity_4LightAtten0[index] * squaredDistance);
                   float3 diffuseReflection = attenuation * float3(unity_LightColor[index]) * float3(_Color) * max(0.0, dot(worldN, lightDirection));         
                   o.vertexLighting = o.vertexLighting + diffuseReflection * 2;
                }

                #endif

                return o;
            }

            half4 frag(vertex_output i) : COLOR {
                i.lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i);

                fixed4 tex = tex2D(_MainTex, i.uv);
                tex *= _Color + fixed4(i.vertexLighting, 1.0);

                fixed diff = saturate(dot(i.normal, i.lightDir));

                fixed4 c;
                c.rgb = UNITY_LIGHTMODEL_AMBIENT.rgb * 2 * tex.rgb;
                c.rgb += (tex.rgb * _LightColor0.rgb * diff) * (atten * 2);
                c.a = tex.a + _LightColor0.a * atten;

                return c;
            }

            ENDCG
        }

        Pass {
            Tags {"LightMode" = "ForwardAdd"}    
            Blend One One  
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdadd

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float3 lightDir : TEXCOORD2;
                float3 normal   : TEXCOORD1;
                LIGHTING_COORDS(3, 4)
            };

            v2f vert(appdata_tan v) {
                v2f o;

                //o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                //o.uv = v.texcoord.xy;
                o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

                o.lightDir = ObjSpaceLightDir(v.vertex);

                o.normal = v.normal;
                TRANSFER_VERTEX_TO_FRAGMENT(o);

                return o;
            }


            fixed4 _Color;
            fixed4 _LightColor0;

            fixed4 frag(v2f i) : COLOR {
                i.lightDir = normalize(i.lightDir);
                fixed atten = LIGHT_ATTENUATION(i);
                fixed4 tex = tex2D(_MainTex, i.uv);
                tex *= _Color;
                fixed3 normal = i.normal;
                fixed diff = saturate(dot(normal, i.lightDir));

                fixed4 c;
                c.rgb = (tex.rgb * _LightColor0.rgb * diff) * (atten * 2);
                c.a = tex.a;

                return c;
            }

            ENDCG
        }
    }
    Fallback "VertexLit"
}