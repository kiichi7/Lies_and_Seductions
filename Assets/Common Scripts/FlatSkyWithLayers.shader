/**********************************************************************
 *
 * SHADER Flat Sky with Layers
 *
 * Copyright 2008 Tommi Horttana, Petri Lankoski, Jari Suominen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License 
 * at http://www.apache.org/licenses/LICENSE-2.0 Unless required 
 * by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 *
 ***********************************************************************/
Shader "Flat Sky with Layers" {
	Properties {
		_Far ("Base (RGB)", 2D) = "white" {}
		_Medium ("Base (RGB) Transparency (A)", 2D) = "white" {}
		_Near ("Base (RGB) Transparency (A)", 2D) = "white" {}
	}
	SubShader {
		Pass 
      	{
           	Blend Off
           	Cull off
           	SetTexture [_Far] { combine texture }
      	}
         
      	Pass
      	{
           	Blend SrcAlpha OneMinusSrcAlpha
           	Cull off
           	SetTexture [_Medium] { combine texture }
      	}   

      	Pass
      	{
           	Blend SrcAlpha OneMinusSrcAlpha
           	Cull off
           	SetTexture [_Near] { combine texture }
		}
	} 
	FallBack "Flat Texture", 1
}
