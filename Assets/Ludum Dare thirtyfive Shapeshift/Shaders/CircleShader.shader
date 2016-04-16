Shader "Custom/Circle Shader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag


				struct a2v
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				fixed4 _Color;

				v2f vert (a2v i)
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
					o.uv = (2 * i.uv) - float2(1, 1);
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
					float4 col = _Color;
					if(length(i.uv) > 1)
					{
						discard;
					}
					return col;
				}
			ENDCG
		}
	}
}
