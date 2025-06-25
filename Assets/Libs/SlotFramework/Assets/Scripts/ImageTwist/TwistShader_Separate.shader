Shader "Custom/TwistShader_Separate"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_MainMachineTexture("Texture", 2D) = "white" {}
		_AFloat("a float", Range(-1,1)) = 0
		_ParamA("Parameter A", float) = 0
		_ParamB("Parameter B", float) = 0
		_TwistAreaLeft("Twist Area Left Border", Range(0,1)) = 0
		_TwistAreaRight("Twist Area Right Border", Range(0,1)) = 1
		_TwistAreaDown("Twist Area Down Border", Range(0,1)) = 0
		_TwistAreaUp("Twist Area Up Border", Range(0,1)) = 1
		_BackgroundColor("Background color", color) = (0, 0, 0 ,0)
		_MainTexRatio("Main Tex Ratio", float) = 1
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

			sampler2D _MainTex; float4 _MainTex_ST;
			sampler2D _MainMachineTexture; float4 _MainMachineTexture_ST;
			float _AFloat;
			float _ParamA;
			float _ParamB;
			float _TwistAreaUp;
			float _TwistAreaDown;
			float _TwistAreaLeft;
			float _TwistAreaRight;
			fixed4 _BackgroundColor;
			float _MainTexRatio;

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
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col; 

				float2 uv;
				if (_TwistAreaUp >= i.uv.y && i.uv.y >= _TwistAreaDown)
				{
					// 三角函数缩放
					// float delta = _ParamA * cos(_ParamB * 3.14159 * (i.uv.y - 0.5)) * (0.5 - i.uv.x);
					// float2 uv = float2(i.uv.x + delta, i.uv.y);

					// 圆周缩放
					float y = (i.uv.y - _TwistAreaDown) / (_TwistAreaUp - _TwistAreaDown);
					float delta = _ParamA * (sqrt(1 - (2 * y - 1) * (2 * y - 1)) - _ParamB);
					float changedX = (i.uv.x + delta)/(2 * delta + 1);

					if (_TwistAreaRight >= changedX && changedX >=_TwistAreaLeft)
					{
						uv = float2(changedX, i.uv.y);
					}
					else
					{
						return tex2D(_MainMachineTexture, TRANSFORM_TEX(i.uv,_MainMachineTexture));
					}
				}
				else
				{
					return tex2D(_MainMachineTexture, TRANSFORM_TEX(i.uv,_MainMachineTexture));
				}

				//用最终的坐标来采样当前的纹理，就ok了
				col = tex2D(_MainTex, TRANSFORM_TEX(uv,_MainTex));
				if (col.x == _BackgroundColor.x && col.y == _BackgroundColor.y && col.z == _BackgroundColor.z)
				{
					return tex2D(_MainMachineTexture, TRANSFORM_TEX(i.uv,_MainMachineTexture));
				}
				else
				{
					return col;
				}
			}
			ENDCG
		}
	}
	FALLBACK "Unlit"
}
