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
 *********************************************************************
 * - Animating disco floor lighs by rotating the floor
 * - Animating Disco ball
 * - Strobo handling here?
 *
 *******************************************************************/
using UnityEngine;
using System.Collections;

/*******************************************************************
 *
 * CLASS DiskoLight
 *
 *******************************************************************/

public class DiskoLights : MonoBehaviour, AnimQualityListener {

	/*******************************************************************
 	 *
 	 * PUBLIC VARIABLES, these need to be set in Unity GUI
	 *
	 *******************************************************************/

	public GameObject rotateThis;
	public float rotationSpeed;
	
	public Material discoBallMaterial;
	public float discoBallAnimSpeed;
	
	public Texture2D []discoBallTextures;
	
	/*******************************************************************
 	 *
 	 * PRIVATE VARIABLES
	 *
	 *******************************************************************/
	
	private bool animationEnabled;
	private int discoBallTexture;
	
	/*******************************************************************
	 *
	 * OVERLOADED METHODS, from MonoBehaviour
	 *
	 *******************************************************************/

	// Use this for initialization
	void Start () {
		LocationAnimQuality.RegisterListener(this);
		animationEnabled = LocationAnimQuality.Enabled();
		enabled = animationEnabled;
		discoBallTexture=0;
		if(animationEnabled) {
			InvokeRepeating("AnimateDiscoBall", discoBallAnimSpeed, discoBallAnimSpeed);
		}
		Debug.Log("DiskoLights.Start(): enabled: " + enabled);
	}
	
	// Update is called once per frame
	void Update () {
		rotateThis.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}
	
	void OnBecameVisible ()  {
		// We only react this if QualitySettings allows enabling Texture animation
		if(animationEnabled) {
			//Debug.Log("DiskoLights.OnBecameVisible(): Enabled");
			enabled = true;
		}
	}
	
	void OnBecameInvisible () {
		// We only react this if QualitySettings allows enabling Texture animation
		if(animationEnabled) {
			//Debug.Log("DiskoLights.OnBecameInvisible(): Disabled");
			enabled = false;
		}
	}

	
	/*******************************************************************************
	 *
	 * IMPLEMETED INTERFACES
	 *
	 *******************************************************************************/
	
	public void AnimationEnabled(bool animEnabled) {
		Debug.Log("DiskoLights.AnimationEnabled(" + animEnabled + ")");
		animationEnabled = animEnabled;	
		if(animEnabled == true) {
			enabled = true;
			discoBallTexture=0;
			InvokeRepeating("AnimateDiscoBall", discoBallAnimSpeed, discoBallAnimSpeed);
		}
		else {
			enabled = false;
			CancelInvoke ("AnimateDiscoBall");	
		}
	} 
	
	/*******************************************************************************
	 *
	 * PRIVATE METHODS
	 *
	 *******************************************************************************/

	private void AnimateDiscoBall() {
		discoBallTexture++;
		if(discoBallTexture >= discoBallTextures.Length) {
			discoBallTexture=0;
		}
		discoBallMaterial.SetTexture("_MainTex", discoBallTextures[discoBallTexture]);
	}
	
}
