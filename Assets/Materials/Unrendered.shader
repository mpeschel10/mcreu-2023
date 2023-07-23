Shader "Unlit/Unrendered"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        // Do the tags even do anything?

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha 
			ZWrite Off
            Cull Front
			Cull Back
			LOD 200
            
            CGPROGRAM
                #include "UnityCG.cginc"
                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };
                
                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                #pragma vertex vert
                #pragma fragment frag

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = float4(0, 0, 0, 0);//UnityObjectToClipPos(v.vertex);
                    o.uv = float2(0, 0);//TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }
                
                fixed4 frag (v2f i) : SV_Target
                {
                    return float4(0, 0, 0, 0);
                }
            ENDCG
        }
    }
}
