Shader "Custom/CheckeredShader"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (0, 0, 1, 1)  // White color
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)  // Black color
        _Tile ("Tile Size", Float) = 10.0          // Number of tiles
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 _Color1;
            fixed4 _Color2;
            float _Tile;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 grid = floor(i.uv * _Tile);
                float checker = fmod(grid.x + grid.y, 2);
                return lerp(_Color1, _Color2, checker);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
