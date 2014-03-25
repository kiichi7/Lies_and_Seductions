/**********************************************************************
 *
 * SHADER GUI/SpeechBubblr
 *
 * Petri Lankoski: Unity GUI/TextShader, replaced original ZTest with ZTest 
 * LEqual to make not to drawn in the top of everything else as we want to 
 * make speech bubble texts to be occluded by characters and objects.
 *
 ***********************************************************************/
 Shader "GUI/SpeechBubble" {	Properties {   		_MainTex ("Font Texture", 2D) = "white" {}   		_Color ("Text Color", Color) = (1,1,1,1)
   	}	SubShader {   		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }   		Lighting Off Cull Off ZTest Always ZWrite Off Fog { Mode Off }   		Blend SrcAlpha OneMinusSrcAlpha
   		Pass {      		ZTest LEqual      		Color [_Color]      		SetTexture [_MainTex] {         		combine primary, texture * primary      		}   		}
   	}} 