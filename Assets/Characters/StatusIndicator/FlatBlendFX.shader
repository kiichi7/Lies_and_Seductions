// IS THIS USED?
Shader "FlatBlendFX" {	Properties {    	_Blend ("Blend", Range(0.0,1.0)) = 0.5    	_MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
   	}
	SubShader {
		Tags {"Queue" = "Transparent" }
		ZWrite On    	ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
    		SetTexture [_MainTex] {constantColor (1,1,1,[_Blend]) combine texture * constant }
    		
		}	
	}
	 	Fallback "Flat Texture With Alpha", 1}