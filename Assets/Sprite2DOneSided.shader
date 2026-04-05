Shader "Sprites/2DOneSided"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Back
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                UNITY_TRANSFER_FOG(OUT,OUT.vertex);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;
                UNITY_APPLY_FOG(IN.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
