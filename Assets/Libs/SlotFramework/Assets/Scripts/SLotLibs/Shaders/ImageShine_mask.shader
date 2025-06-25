// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ImageShine_mask" {
    Properties {
        _MainTex ("image", 2D) = "white" {}
        _percent ("_percent", Range(-5, 5)) = 1
        _angle("_angle", Range(0, 360)) = 30
        _width("_width",Float) = 2
        _shineColor("_shineColor",Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }
    
    CGINCLUDE
        #include "UnityCG.cginc" 
        #include "UnityUI.cginc"

        #pragma multi_compile __ UNITY_UI_ALPHACLIP

        sampler2D _MainTex;
        float _percent;
        float _angle;
        float _width;
        float4 _shineColor;

        float4 _ClipRect;
        struct v2f {    
            float4 pos:SV_POSITION;    
            float2 uv : TEXCOORD0;   //用TEXCOORD1来使坐标范围固定为0到1,原来默认的是TEXCOORD0
            float2 uv1: TEXCOORD1;
            float4 worldPosition : TEXCOORD2;
        };  
  
        v2f vert(appdata_full v) {  
            v2f o;  
            o.pos = UnityObjectToClipPos (v.vertex);  
            o.uv = v.texcoord.xy;
            o.uv1 = v.texcoord1.xy;  
            o.worldPosition = v.vertex;
            return o;  
        }  
  
        fixed4 frag(v2f i) : COLOR0 {
            float2 uv = i.uv.xy - float2(0.5,0.5);    
                      
            float temp_angle =  radians(_angle);    
            fixed2x2 rotMat = fixed2x2(cos(temp_angle),sin(temp_angle),-sin(temp_angle),cos(temp_angle));  // 旋转矩阵å

         fixed4 k = tex2D(_MainTex, i.uv);
         fixed4 k1 = tex2D(_MainTex, i.uv);
             
            uv = (i.uv1 + fixed2(_percent,_percent)) * _width; // 缩放并位移
            uv = mul(rotMat, uv); //旋转

            fixed temp1 = saturate(lerp(fixed(1),fixed(0),abs(uv.y)));
            fixed4 temp_line = fixed4(temp1,temp1,temp1,temp1);
            temp_line *= _shineColor; 
 

            k +=  temp_line; // 加上光线
            k *= fixed4(fixed3(1,1,1),k1.a); 

            k.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                
            #ifdef UNITY_UI_ALPHACLIP
            clip (k.a - 0.001);
            #endif
            
            return k;
        }  
    ENDCG    
  
    SubShader {   
        Tags {"Queue" = "Transparent"
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"}    

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp] 
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
         
        ZWrite Off     
        Blend SrcAlpha OneMinusSrcAlpha     
        ColorMask [_ColorMask]
        Pass {    
            CGPROGRAM    
            #pragma vertex vert    
            #pragma fragment frag    
            #pragma fragmentoption ARB_precision_hint_fastest     
  
            ENDCG    
        }
    }
    FallBack Off  
}
