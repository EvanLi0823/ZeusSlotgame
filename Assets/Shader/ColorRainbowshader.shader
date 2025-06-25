
Shader "UI/ColorRainbow"
{
    Properties
    {
		// Variables from default UI shader

		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _SliceNum("平分多少份",Int)=2
		_Speed("Speed", Range(0.1, 3)) = 1
		
		_MinColor("MinColor", Range(0, 1)) = 0
		_MaxColor("MaxColor", Range(0, 1)) = 1
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

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

			static const float recip2Pi = 0.159154943;
			static const float twoPi = 6.2831853;

			float3 hsv2rgb(float3 c)
			{
				float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
				float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
				return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
			}
			
			float3 rgb2HSV(float3 c)  
            {  
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);  
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));  
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));  
                
                float d = q.x - min(q.w, q.y);  
                float e = 1.0e-10;  
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);  
            }  
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

			sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

			float _Speed;
			int _SliceNum;

//			float _SVSquareSize;

			float _MinColor,_MaxColor;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = v.texcoord;

                OUT.color = v.color * _Color;
                return OUT;
            }

//			half4 hueRing(float2 uv)
//			{
//				float2 coords = uv - .5;
//
//				float r = length(coords);
//
//				float fw = fwidth(r);
//
////				float a = smoothstep(.5, .5 - fw, r) * smoothstep(_HueCircleInner - fw, _HueCircleInner, r);
//
//				float angle = atan2(coords.y, coords.x) * recip2Pi ;
//				angle = (angle + _Time.y * _Speed) % 1;
//				
//				float tmp = _MaxColor - _MinColor;
////				angle =_MinColor+ angle / tmp ;
//
//				return half4(hsv2rgb(float3(angle, 1, 1)), 1);
//			}
//
//			half4 whiteRing(float2 uv, float2 pos, float inner, float outer)
//			{
//				float2 coords = uv - pos;
//
//				float r = length(coords);
//
//				float fw = fwidth(r);
//
//				float a = smoothstep(outer, outer - fw, r) * smoothstep(inner - fw, inner, r);
//
//				return half4(1, 1, 1, a);
//			}
//
//			half4 svSquare(float2 uv)
//			{
//				float2 sv = (uv - .5) / (_SVSquareSize * 2) + .5;
//
//				float dx = abs(ddx(sv.x));
//				float dy = abs(ddy(sv.y));
//
//				float a =
//					smoothstep(0, dx, sv.x) * smoothstep(1, 1 - dx, sv.x) *
//					smoothstep(0, dy, sv.y) * smoothstep(1, 1 - dy, sv.y);
//
//				return float4(hsv2rgb(float3(_HSV.x, sv)), a);
//			}

//			fixed4 mix(fixed4 bot, fixed4 top)
//			{
//				return fixed4(lerp(bot.rgb, top.rgb, top.a), max(bot.a, top.a));
//			}

	        half4 lineColor(float2 uv)
			{
			    float wholeSlice = 1.0/_SliceNum;
				float2 coords = uv - .5;

				float r = length(coords);

				float fw = fwidth(r);

//				float a = smoothstep(.5, .5 - fw, r) * smoothstep(_HueCircleInner - fw, _HueCircleInner, r);

				float angle = atan2(coords.y, coords.x) * recip2Pi *2; // 值为-0.5到0.5之间
				angle = ((angle+1) /2);//转到0到1之间
				
				angle = (angle + _Time.y * _Speed) % 1;
				if(angle==0)angle=1;
				
				int num = (angle / wholeSlice);
				float angleZone = angle -  wholeSlice * num;    //0到1之间的部分区域  如[0,0.25]
				
//				angleZone = _MinColor + (_MaxColor - _MinColor)  * ; //限定到 minColor和maxColor 中间
                float lerpValue = smoothstep(0,wholeSlice, angleZone);
				angleZone =_MaxColor - (_MaxColor-_MinColor) * abs(lerpValue - 0.5) * 2; //限定到 minColor和maxColor 中间，且取中间平均数 ，公式为 Max + 2x|(step-0.5)|x(max-min)
//				angle = (angle + _Time.y * _Speed) % 1;
				
//				float tmp = _MaxColor - _MinColor;
//				angle =_MinColor+ angle / tmp ;

				return half4(hsv2rgb(float3(angleZone, 1, 1)), 1);
			}

            fixed4 frag(v2f IN) : SV_Target
            {
				// Aspect ratio correction

//				float2 uv = _AspectRatio > 1 ?
//					float2(.5 + (IN.texcoord.x - .5) * _AspectRatio, IN.texcoord.y) :
//					float2(IN.texcoord.x, .5 + (IN.texcoord.y - .5) / _AspectRatio);

				// Hue ring

                float2 uv = IN.texcoord.xy;
                
                fixed4 orgin = tex2D(_MainTex, uv);
                float3 hsv = rgb2HSV(float3(orgin.xyz));
				half4 color = lineColor(uv);
				  fixed x = orgin.r*orgin.a;
				
                color = half4(color.r,color.g,color.b,x);
                
              
//                color = fixed4(x,x,x,x);
				// Hue ring selector

//				float hSelectorR = (.5 - _HueCircleInner) * .5;
//
//				half4 hSelector = whiteRing(
//					uv,
//					float2(cos(_HSV.x * twoPi), sin(_HSV.x * twoPi)) * (.5 - hSelectorR) + .5,
//					hSelectorR * _HueSelectorInner, hSelectorR);
//
//				color = mix(color, hSelector);
//
//				// Saturation value Square
//
//				half4 sv = svSquare(uv);
//
//				color = sv.a > 0 ? sv : color;
//
//				// Saturation value selector
//
//				half4 svSelector = whiteRing(
//					uv,
//					.5 + 2 * _SVSquareSize * (_HSV.yz - .5),
//					hSelectorR * _HueSelectorInner, hSelectorR);
//
//				color = mix(color, svSelector);

				// Unity stuff

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

				#ifdef UNITY_COLORSPACE_GAMMA
                return color;
				#endif

				return fixed4(GammaToLinearSpace(color.rgb), color.a);
            }
        ENDCG
        }
    }
}
