Shader "Flat Texture With Alpha" {
	Properties {
		_MainTex ("Base (RGB) Transparency (A)", 2D) = "" {}
		_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
	}
	SubShader {
		Pass {
			Material {
			}
			Lighting Off
			AlphaTest Greater [_Cutoff]
			SetTexture [_MainTex] {
				combine texture
			}
		}
	} 
}
