Shader "Custom/Character Shader"
{
	Properties
	{
		_Border ("BorderColor", Color) = (0.7,0.7,0.7,1)
		_Fill ("FillColor", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector" = "True" }
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
					float2 lPos : TEXCOORD0;
				};

				fixed4 _Border;
				fixed4 _Fill;

				v2f vert (a2v i)
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, i.vertex);
					o.lPos = (2 * i.uv) - float2(1, 1);
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
					float f = 0;
					if (i.lPos.x > 0.75 || i.lPos.x < -0.75)
					{
						f += (abs(i.lPos.x) - 0.75) * 4;
					}
					if (i.lPos.y > 0.75 || i.lPos.y < -0.75)
					{
						f = max(f, (abs(i.lPos.y) - 0.75) * 4);
					}
					fixed4 col = lerp(_Fill, _Border, f);
					return col;
				}
			ENDCG
		}
	}
}
