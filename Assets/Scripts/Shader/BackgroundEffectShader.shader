Shader "Custom/BackgroundEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _WaveVector ("Wave Vector", Vector) = (0, 0, 30, 8)
        _WaveRadius ("Wave Radius", Float) = 0
        _Color ("Back Color", Float) = 1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #pragma shader_feature WAVE_EFFECT
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

            float4 _WaveVector;
            float _WaveRadius;

			v2f vert (appdata v)
			{
				v2f o;
                #ifdef WAVE_EFFECT
                float2 vec = v.vertex.xy - _WaveVector.xy;
                float dis = max(length(vec), 0.01);
                float off = sin(clamp(1 - (dis - _WaveRadius) / _WaveVector.z, 0, 1) * 3.14) * _WaveVector.w;
                v.vertex.xy = vec * (1 + off / dis) + _WaveVector.xy;
                #endif
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            float _Color;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col * _Color;
			}
			ENDCG
		}
	}
}
