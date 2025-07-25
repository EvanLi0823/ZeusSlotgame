﻿Shader "Effect/RealYou/Gray_WithStencil"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        [Enum(Less, 1, NotLess, 10)] _Comparison ("比较方式", Float) = 10
        _Xvalue("X 轴进度", Range(0,1)) = 0
        _Yvalue("Y 轴进度", Range(0,1)) = 0

        
        _Color ("Color", Color) = (1, 1, 1, 1)

        _Brightness("亮度",Range(0,1)) = 0.5

        // _ColorMask ("Color Mask", Float) = 255

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
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            CGPROGRAM
#pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float4 _MaskTex_ST, _MainTex_ST;
            float _Xvalue, _Yvalue, _Brightness, _Comparison;
            fixed4 _Color;


            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };


            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
			OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _AlphaTex;

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);

                #if ETC1_EXTERNAL_ALPHA
			// get the color from an external texture (usecase: Alpha support for ETC1 on android)
			    color.a = tex2D(_AlphaTex, uv).r;
                #endif //ETC1_EXTERNAL_ALPHA

                float tempX = _Xvalue;
                float tempY = _Yvalue;
               
                if (_Comparison == 1)
                {
                    if (uv.x <= tempX && uv.y <= tempY)
                    {
                        color.rgb = color.rgb * _Brightness;
                    }
                }
                else
                {
                    if (uv.x > tempX && uv.y > tempY)
                    {
                        color.rgb = color.rgb * _Brightness;
                    }
                    
                }
                return color;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord);
                return c;
            }
            ENDCG

        }
    }
}