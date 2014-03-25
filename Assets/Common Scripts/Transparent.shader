Shader "Transparent" {
	Properties {
		_MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
	}
	SubShader {
		Pass {
			Lighting Off
			AlphaTest Greater 0.0
			SetTexture [_MainTex] {
				combine texture
			}
		}
	} 
}
