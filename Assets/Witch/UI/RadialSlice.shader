Shader "Jade/Unlit/RadialSlice"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _InnerRadius ("InnerRadius", Float) = 0.5
        _ArcStart ("ArcStart", Float) = 0.0
        _ArcEnd ("ArcEnd", Float) = 1.0

        _TintColor ("TintColor", Color) = (1.0,1.0,1.0,1.0)
        _BorderColor ("BorderColor", Color) = (1.0,1.0,1.0,1.0)
        _BorderWidth ("BorderWidth", Float) = 0.05
    }
    SubShader
    {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
        LOD 100

        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;

            float _InnerRadius;
            float _ArcStart;
            float _ArcEnd;

            float4 _TintColor;
            float4 _BorderColor;
            float _BorderWidth;

            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;
                fixed x = i.uv.x * 2 - 1;
                fixed y = i.uv.y * 2 - 1;
                fixed rSquared = x*x + y*y;
                fixed r = sqrt(rSquared);
                
                fixed start = min(_ArcStart, _ArcEnd);
                fixed end = max(_ArcStart, _ArcEnd);
                
                fixed theta = atan2(y,x) - start;
                if(theta >= UNITY_PI*2) {theta -= UNITY_PI*2;} 
                if(theta < 0) {theta += UNITY_PI*2;} 
                fixed arcLength = end - start;
                
                fixed borderDist = min( 1 - r, r - _InnerRadius );
                borderDist = min(borderDist, theta);
                borderDist = min(borderDist, - theta + arcLength);

                fixed4 borderFactor = clamp(1 - borderDist/_BorderWidth,0,1);
                col.rgb = col.rgb * (1-borderFactor) + _BorderColor.rgb * borderFactor;

                clip(borderDist);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
