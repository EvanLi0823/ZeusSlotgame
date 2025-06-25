Shader "Effect/RealYou/Mask_Shine"
{
	Properties
	{
		
		
		
		[Enum(Addtive), 1, AlphaBlend, 10)] _DestBlend ("混合模式", Float) = 1
		[Enum(Off, 0, Front, 1, Back, 2)]_Cull ("双面", Float) = 2

		
		_MainTex ("MainTex", 2D) = "white" { }
		_Color ("Color", Color) = (1, 1, 1, 1)
		_MaskTex ("MaskTex", 2D) = "white" { }
		_Xspeed ("X轴速度", float) = 0
		_Yspeed ("Y轴速度", float) = 0
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
		Cull [_Cull]
		ZWrite Off
		Blend SrcAlpha [_DestBlend]
		
		Pass
		{
			
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			sampler2D _MainTex, _MaskTex;
			float4 _MaskTex_ST, _MainTex_ST;
			float _Xspeed, _Yspeed;
			fixed4 _Color;
			
			struct a2v
			{
				float4 vertex: POSITION;
				float2 texcoord: TEXCOORD0;
				fixed4 vertexColor: COLOR;
			};
			
			struct v2f
			{
				float4 uv: TEXCOORD0;
				float4 pos: SV_POSITION;
				fixed4 vertexColor: COLOR;
			};
			
			
			
			v2f vert(a2v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _MaskTex);
				o.uv.x = o.uv.x + _Time.y * _Xspeed;
				o.uv.y = o.uv.y + _Time.y * _Yspeed;
				o.uv.xy = float2(o.uv.x, o.uv.y);
				o.vertexColor = v.vertexColor;
				return o;
			}
			
			fixed4 frag(v2f i): SV_Target
			{
				fixed3 c = tex2D(_MainTex, i.uv.xy) * _Color;
				fixed3 m = tex2D(_MaskTex, i.uv.zw);
				fixed3 finalcolor = c * m;
				return fixed4(finalcolor * i.vertexColor, m.r * _Color.a * i.vertexColor.a);
			}
			ENDCG
			
		}
	}
}