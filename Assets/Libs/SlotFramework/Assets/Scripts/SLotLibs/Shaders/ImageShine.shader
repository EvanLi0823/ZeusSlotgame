// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ImageShine" {
	Properties {
		_MainTex ("image", 2D) = "white" {}
		_percent ("_percent", Range(-5, 5)) = 1
		_angle("_angle", Range(0, 360)) = 30
		_width("_width",Float) = 2
		_shineColor("_shineColor",Color) = (1,1,1,1)
	}
	
	CGINCLUDE
        #include "UnityCG.cginc"           
        
        sampler2D _MainTex;
		float _percent;
		float _angle;
		float _width;
		float4 _shineColor;
		
        struct v2f {    
            float4 pos:SV_POSITION;    
            float2 uv : TEXCOORD0;   //用TEXCOORD1来使坐标范围固定为0到1,原来默认的是TEXCOORD0
            float2 uv1: TEXCOORD1;
        };  
  
        v2f vert(appdata_full v) {  
            v2f o;  
            o.pos = UnityObjectToClipPos (v.vertex);  
            o.uv = v.texcoord.xy;
            o.uv1 = v.texcoord1.xy;  
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
            
            return k;
        }  
    ENDCG    
  
    SubShader {   
        Tags {"Queue" = "Transparent"}     
        ZWrite Off     
        Blend SrcAlpha OneMinusSrcAlpha     
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
