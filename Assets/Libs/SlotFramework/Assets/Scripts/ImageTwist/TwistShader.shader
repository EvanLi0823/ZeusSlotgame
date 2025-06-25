Shader "Custom/TwistShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_BackgoundTex("Texture", 2D) = "white" {}
		_AFloat("a float", Range(-1,1)) = 0
		_ParamA("Parameter A", float) = 0
		_ParamB("Parameter B", float) = 0
		_TwistAreaLeft("Twist Area Left Border", Range(0,1)) = 0
		_TwistAreaRight("Twist Area Right Border", Range(0,1)) = 1
		_TwistAreaDown("Twist Area Down Border", Range(0,1)) = 0
		_TwistAreaUp("Twist Area Up Border", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert alpha
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex, _BackgoundTex;
			float _AFloat;
			float _ParamA;
			float _ParamB;
			float _TwistAreaUp;
			float _TwistAreaDown;
			float _TwistAreaLeft;
			float _TwistAreaRight;
			fixed4 _BackgroundColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			// 圆周缩放
			float OriginX(float pX, float pY) {
				float y = (pY - _TwistAreaDown) / (_TwistAreaUp - _TwistAreaDown);
				float delta = _ParamA * (sqrt(1 - (2 * y - 1) * (2 * y - 1)) - _ParamB);
				float changedX = ((pX - _TwistAreaLeft) / (_TwistAreaRight - _TwistAreaLeft) + delta)/(2 * delta + 1);
				changedX = _TwistAreaLeft + (_TwistAreaRight - _TwistAreaLeft) * changedX;

				return changedX;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col;

				// 计算缩放后的uv坐标
				float changedX = OriginX(i.uv.x, i.uv.y);
				// 计算当前像素所处区域
				float inUpDown = step(i.uv.y, _TwistAreaUp) * step(_TwistAreaDown, i.uv.y);
				float inLeftRight = step(changedX, _TwistAreaRight) * step(_TwistAreaLeft, changedX);
				float inBackground = step(i.uv.x, _TwistAreaRight) * step(_TwistAreaLeft, i.uv.x);

				//用最终的坐标来采样当前的纹理
				return (1 - inUpDown + inUpDown * (1 - inLeftRight) * (1 - inBackground)) * tex2D(_MainTex, i.uv)
				+ inUpDown * inLeftRight * tex2D(_MainTex, float2(changedX, i.uv.y))
				+ inUpDown * (1 - inLeftRight) * inBackground * tex2D(_BackgoundTex, i.uv);

//				if (_TwistAreaUp >= i.uv.y && i.uv.y >= _TwistAreaDown)
//				{
//					// 三角函数缩放
//					// float delta = _ParamA * cos(_ParamB * 3.14159 * (i.uv.y - 0.5)) * (0.5 - i.uv.x);
//					// float2 uv = float2(i.uv.x + delta, i.uv.y);
//
//					// 圆周缩放
//					float changedX = OriginX(i.uv.x, i.uv.y);
//
//					if (_TwistAreaRight >= changedX && changedX >=_TwistAreaLeft)
//					{
//						return tex2D(_MainTex, float2(changedX, i.uv.y));
//					}
//					else if (_TwistAreaRight >= i.uv.x && i.uv.x >=_TwistAreaLeft)
//					{
//						return tex2D(_BackgoundTex, i.uv);
//					}
//				}
//
//				//用最终的坐标来采样当前的纹理，就ok了
//				return tex2D(_MainTex, i.uv);
//				return tex2D(_MainTex, TRANSFORM_TEX(uv, _MainTex));
			}
			ENDCG
		}
	}
	FALLBACK "Unlit/Texture"
}
