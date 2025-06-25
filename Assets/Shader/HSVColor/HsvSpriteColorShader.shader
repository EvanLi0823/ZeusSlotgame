Shader "Custom/HsvSpriteColorShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _TargetColor("TargetColor", Color) = (0.5, 0, 0.5, 0)

        	_Stencil ("Stencil Ref", Float) = 0
		_StencilComp ("Stencil Comparison", Float) = 8
	}
	SubShader
	{
	Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		// No culling or depth

		Pass
		{
			Stencil
				{
					Ref [_Stencil]
					Comp [_StencilComp]
					Pass Keep
				}
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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            fixed4 _TargetColor;

            fixed3 rgb2hsv(fixed3 c)
            {
               fixed4 K = fixed4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
               fixed4 p = lerp(fixed4(c.bg, K.wz), fixed4(c.gb, K.xy), step(c.b, c.g));
               fixed4 q = lerp(fixed4(p.xyw, c.r), fixed4(c.r, p.yzx), step(p.x, c.r));

               float d = q.x - min(q.w, q.y);
               float e = 1.0e-10;
               return fixed3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            fixed3 hsv2rgb(fixed3 c)
            {
               fixed4 K = fixed4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
               fixed3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
               return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				//col = 1 - col; return col;

                fixed3 hsv = rgb2hsv(col);
                hsv.x = rgb2hsv(_TargetColor).x;
                hsv.y = rgb2hsv(_TargetColor).y;
                hsv.z *= rgb2hsv(_TargetColor).z;
                col = fixed4(hsv2rgb(hsv).r, hsv2rgb(hsv).g, hsv2rgb(hsv).b, 1);

				return col;
			}
			ENDCG
		}
	}
}
