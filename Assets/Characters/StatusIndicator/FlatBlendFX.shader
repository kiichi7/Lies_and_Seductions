// IS THIS USED?
Shader "FlatBlendFX" {
   	}
	SubShader {
		Tags {"Queue" = "Transparent" }
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
    		SetTexture [_MainTex] {constantColor (1,1,1,[_Blend]) combine texture * constant }
    		
		}	
	}
	 