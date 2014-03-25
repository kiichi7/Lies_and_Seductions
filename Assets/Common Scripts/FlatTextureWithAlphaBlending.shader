Shader "Flat Texture With Alpha Blending" {
	Properties {
		_MainTex("_MainText", 2D) = "white" {}
		//_Color("_MainColor", Color) = (1,1,1,1)
	}
	
	SubShader {
		Tags{"Queue" = "Transparent"}
		Lighting Off ZTest LEqual ZWrite On Fog {Mode Off}
		Blend SrcAlpha OneMinusSrcAlpha
		Pass {
			//Color [_Color]
			SetTexture [_MainTex] {
				combine texture, texture * primary
				//combine primary, texture * primary
			}
		}
	}
	
}