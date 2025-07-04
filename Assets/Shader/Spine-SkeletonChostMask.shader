﻿// - Unlit + no shadow
// - Premultiplied Alpha Blending (One OneMinusSrcAlpha)
// - Double-sided, no depth

Shader "Spine/SkeletonChostMask" {
    Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		[NoScaleOffset] _MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_TextureFade ("Texture Fade Out", Range(0,1)) = 0

		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
    }
    SubShader {
		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="False"
			"RenderType"="Transparent"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
		}

		Fog { Mode Off }
		Blend One OneMinusSrcAlpha
		ZWrite Off
		Cull Off
      
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;
			fixed4 _Color;
			fixed _TextureFade;
			    
			struct VertexInput {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct VertexOutput {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
				float4 color : COLOR;
			};

			VertexOutput vert (VertexInput v) {
			    VertexOutput o;
			    o.pos = UnityObjectToClipPos(v.vertex);
			    o.uv = v.uv;
			    o.color = v.color;
			    return o;
			}

			fixed4 frag (VertexOutput i) : COLOR {
			    fixed4 tc = tex2D(_MainTex, i.uv);
				tc = fixed4(max(_TextureFade, tc.r), max(_TextureFade, tc.g), max(_TextureFade, tc.b), tc.a);
				return tc * ((i.color * _Color) * tc.a);
			}
			ENDCG
		}
    }
}