// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ImageMask" {
	Properties {
		_MainTex ("image", 2D) = "white" {}
	//-------------------add----------------------
	  _MinX ("Min X", Float) = 0.5
      _MaxX ("Max X", Float) = 0.8
      _MinY ("Min Y", Float) = 0.5
      _MaxY ("Max Y", Float) = 0.8
      _MinX2 ("Min X", Float) = 0.5
      _MaxX2 ("Max X", Float) = 0.8
      _MinY2 ("Min Y", Float) = 0.5
      _MaxY2 ("Max Y", Float) = 0.8
       _MinX3 ("Min X", Float) = 0.5
      _MaxX3 ("Max X", Float) = 0.8
      _MinY3 ("Min Y", Float) = 0.5
      _MaxY3 ("Max Y", Float) = 0.8
      //-------------------add----------------------
	}
	
	CGINCLUDE
        #include "UnityCG.cginc"           
        
        sampler2D _MainTex;
			float _MinX;
            float _MaxX;
            float _MinY;
            float _MaxY;
            float _MinX2;
            float _MaxX2;
            float _MinY2;
            float _MaxY2;
            float _MinX3;
            float _MaxX3;
            float _MinY3;
            float _MaxY3;
		
        struct v2f {    
            float4 pos:SV_POSITION;    
            float2 uv : TEXCOORD0;   
        };  
  
        v2f vert(appdata_base v) {  
            v2f o;  
            o.pos = UnityObjectToClipPos (v.vertex);  
            o.uv = v.texcoord.xy;  
            return o;  
        }  
  
        fixed4 frag(v2f i) : COLOR0 {
       	 	fixed4 c = tex2D(_MainTex, i.uv);
	             c.a *= !(i.uv.x >= _MinX && i.uv.x <= _MaxX && i.uv.y >= _MinY && i.uv.y <= _MaxY);
	             c.a *= !(i.uv.x >= _MinX2 && i.uv.x <= _MaxX2 && i.uv.y >= _MinY2 && i.uv.y <= _MaxY2);
	        	 c.a *= !(i.uv.x >= _MinX3 && i.uv.x <= _MaxX3 && i.uv.y >= _MinY3 && i.uv.y <= _MaxY3);
                 c.rgb *= c.a;
                return c;
		
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
