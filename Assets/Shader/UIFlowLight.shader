Shader "Custom/UIFlowLight"
{
	Properties
	{
		[PerRendererData]_MainTex ("Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap("Pixel Snap",float)=0

		// FlowLight
		_FLowLightTex("Move Texture",2D)="white"{}//扫光贴图
		_FLowLightColor("FlowLight Color",Color)=(0,0,0,1)//扫光颜色
		_RangeBeginX("RangeBeginX",Range(0,1))=0.5//扫光起始x
		_Power("Power",float)=1//扫光大小
		_SpeedX("SpeedX",float)=1//X轴移动速度
		_SpeedY("SpeedY",float)=0//Y轴移动速度
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

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

		Cull off
		Lighting off
		ZWrite off
		Blend One OneMinusSrcAlpha


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _PIXELSNAP_ON
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				half2 texcoord : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				// FlowLight
				float2 texFlowLight :TEXCOORD1;
			};

			// FLowLight
			fixed4 _FLowLightColor;
			float _Power;
			sampler2D _FLowLightTex;
			fixed4 _FLowLightTex_ST;
			fixed _SpeedX;
			fixed _SpeedY;
			float _RangeBeginX;

			sampler2D _MainTex;
			
			v2f vert (appdata IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord=IN.texcoord;

				// FlowLight
				OUT.texFlowLight=TRANSFORM_TEX(IN.texcoord,_FLowLightTex);
			
				float offsetX =_Time.x *_SpeedX;
				offsetX = frac(offsetX);

				float offsetY =_Time.x *_SpeedY;
				offsetY = frac(offsetY);
				
				OUT.texFlowLight.x+= offsetX + _RangeBeginX;
				OUT.texFlowLight.y+= offsetY;

				OUT.color = IN.color;
				#ifdef PixelSnap_ON
				OUT.vertex=UnityPixelSnap(OUT.vertex);
				#endif

				return OUT;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, IN.texcoord);
			
				// FlowLight
				fixed4 cadd=tex2D(_FLowLightTex,IN.texFlowLight)*_Power*_FLowLightColor;
				cadd.rgb*=col.rgb;
				col.rgb +=cadd.rgb;
				 col.rgb *=col.a;

				return col;
			}
			ENDCG
		}
	}
}
