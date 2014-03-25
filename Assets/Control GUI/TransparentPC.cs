/**********************************************************************
 *
 * CLASS 
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
using UnityEngine;
using System.Collections;
public class TransparentPC : MonoBehaviour {
	private RenderTexture renderTexture;
	public GUITexture targetTexture;
	public float maximumDistanceWithTransparency = 0.5f;
	
	public void Start() {
		// 1024, 1024
		//camera.targetTexture.width = Screen.width;
		//camera.targetTexture.height = Screen.height;
		renderTexture.isPowerOfTwo = false;
		//targetTexture.texture = renderTexture;
		//float w = Screen.width * ((float)Screen.width / 1024.0f);
		targetTexture.pixelInset = new Rect(0, 0, Screen.width, Screen.height);
		targetTexture.enabled = true;
	}
	
	public void UpdateAlpha(){
		float distance = Vector3.Distance(transform.position, CharacterManager.GetPC().transform.position);
		if (distance < maximumDistanceWithTransparency){
			float pcAlpha = distance / (maximumDistanceWithTransparency + 4); 
			renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
    		camera.targetTexture = renderTexture;
			targetTexture.texture = renderTexture;
    		targetTexture.color = new Color(targetTexture.color.r, targetTexture.color.g, targetTexture.color.b, pcAlpha);
    		targetTexture.enabled = true;
		} else {
			camera.targetTexture = null;
			targetTexture.enabled = false;
		}
	}
}