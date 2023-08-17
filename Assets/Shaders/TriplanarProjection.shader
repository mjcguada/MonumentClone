Shader "Custom/TriplanarProjection"
{
    Properties {
        _ColorX ("Color X", Color) = (1, 0, 0, 1)
        _ColorY ("Color Y", Color) = (0, 1, 0, 1)
        _ColorZ ("Color Z", Color) = (0, 0, 1, 1)
        _Scale ("Texture Scale", Vector) = (1, 1, 1)
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float3 worldNormal : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _ColorX;
            float4 _ColorY;
            float4 _ColorZ;
            float3 _Scale;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 worldNormal = normalize(i.worldNormal);
                float3 triplanarCoords = abs(worldNormal);
                triplanarCoords *= _Scale;

                fixed4 finalColor = _ColorX * triplanarCoords.x + _ColorY * triplanarCoords.y + _ColorZ * triplanarCoords.z;

                return finalColor;
            }
            ENDCG
        }
    }
}
