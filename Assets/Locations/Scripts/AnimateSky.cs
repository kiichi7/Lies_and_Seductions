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
 ***********************************************************************
 * CLASS AnimateSky
 * - Creates offset based animation for sky (in order to create paralax effect)
 * - Uses three layers for the sky, two first are semitransparent containing cloads and 
 *   third contains sky background and cloads
 * - Sky material needs to have "FlatSkyWithLayers" shader
 * - Changes sky for a DAY, NIGHT, and SUNSET. Sunset is a texture animation
 *  
 *
 **********************************************************************************/
using UnityEngine;
using System.Collections;

/*********************************************************************************
 *
 * CLASS AnimateSky
 *
 *********************************************************************************/

public class AnimateSky : MonoBehaviour, AnimQualityListener, TimeOfDayListener {

	/*********************************************************************************
	 *
	 * PUBLIC VARIABLES, these are set in Inspector
	 *
	 *********************************************************************************/

	// Sky to be animated
	public Material sky;
	
	// Speed for main animaton (different layers of sky can be animated with different speed)
	public float animationSpeedFar;
	public float animationSpeedMedium;
	public float animationSpeedNear;

	public SkyTextureSet dayTexureSet;
	public SkyTextureSet nightTextureSet;
	public SkyTextureSet []sunsetAnimationTextureSet;
	
	public float sunsetSpeed;
	
	public Camera mainCamera;
	
	/*********************************************************************************
	 *
	 * PRIVATE VARIABLES
	 *
	 *********************************************************************************/

	private float offsetFar;
	private float offsetMedium;
	private float offsetNear;
	
	private bool animationEnabled;
	private bool imageEffectSupported;
	// Do we need this
	private GameTime.TIME_OF_DAY timeOfDay;
	private int sunsetScreen;
	private int rampScreen;
	private int screenindex;
	
	private ColorCorrectionEffect cameraEffectScript;
	private CharacterState pcState;
	
	private static AnimateSky instance=null;
		
	/*********************************************************************************
	 *
	 * OVERLOADED METHDOS, from MonoBehaviour
	 *
	 *********************************************************************************/
	
	void Awake() {
			instance = this;
	}
	
	// Use this for initialization
	void Start () {
		
		Init();
		
		// registering to receice changes in QualitySetting in animation menu
		LocationAnimQuality.RegisterListener(this);
		
		// get the initial value and enable or disable Behaviour based on it
		enabled = LocationAnimQuality.Enabled();
		animationEnabled = enabled;
		
		GameTime.RegisterTimeOfDayListener(this);
		cameraEffectScript = mainCamera.GetComponent(typeof(ColorCorrectionEffect)) as ColorCorrectionEffect;
		
		// We want to switch image effects on and off. So we need to check if effect is supported
		// Effect does this, but we do not know if it disabled itself because effect or shader is not supported
		imageEffectSupported = SystemInfo.supportsImageEffects;
		if(imageEffectSupported) {
			imageEffectSupported = cameraEffectScript.shader.isSupported;
		}
		Debug.Log("AnimateSky.Start():  image effects and effect shader supported: " + imageEffectSupported);
		
		GameObject pc = CharacterManager.GetPC();
		pcState = pc.GetComponent(typeof(CharacterState)) as CharacterState;
		
		//SetSkyTextureSet(dayTexureSet);
		//timeOfDay=GameTime.TIME_OF_DAY.DAY;
		timeOfDay = GameTime.GetTimeOfDay();
		
		switch(timeOfDay) {
			case GameTime.TIME_OF_DAY.DAY:
				SetSkyTextureSet(dayTexureSet);
				break;
			case GameTime.TIME_OF_DAY.NIGHT:
				SetSkyTextureSet(nightTextureSet);
				break;	
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		 	
		// Actual animation, we do it by manipulating texture offset
		offsetFar+=animationSpeedFar * Time.deltaTime;
		offsetMedium+=animationSpeedMedium * Time.deltaTime;
		offsetNear+=animationSpeedNear * Time.deltaTime;
		
		sky.SetTextureOffset ("_Far", new Vector2(offsetFar, 0));
		sky.SetTextureOffset ("_Medium", new Vector2(offsetMedium, 0));
		sky.SetTextureOffset ("_Near", new Vector2(offsetNear, 0));
	}

	void OnBecameVisible ()  {
		// We only react this if QualitySettings allows enabling Texture animation
		if(animationEnabled) {
			Debug.Log("AnimateSky:Enabled");
			enabled = true;
		}
	}
	
	void OnBecameInvisible () {
		// We only react this if QualitySettings allows enabling Texture animation
		if(animationEnabled) {
			Debug.Log("AnimateSky:Disabled");
			Init();
			enabled = false;
		}
	}
		
	/*********************************************************************************
	 *
	 * PUBLIC STATIC
	 *
	 *********************************************************************************/

	public static void AreaChanged() {
		// Called by Area class. We cannot poll 
		Debug.Log("AnimateSky.AreaChanged(): current area=" + instance.pcState.GetCurrentArea().name);
		
	
		// Check if QualityFactors setting allows ImageEffects
		if(!instance.animationEnabled) {
			//Debug.Log("-- Color Correction Effect OFF");
			return;
		}
		
		// No Effects in daytime
		if(instance.timeOfDay == GameTime.TIME_OF_DAY.DAY) { return; } 
		
		// Effects are used only in Deck, not inside
		if(instance.pcState.GetCurrentArea().name.Equals("Deck") && instance.imageEffectSupported && instance.cameraEffectScript.textureRamp) {
				//Debug.Log("-- Color Correction Effect Effet ON"); 
				instance.cameraEffectScript.enabled = true;
		}
		else {
			instance.cameraEffectScript.enabled = false;
			//Debug.Log("-- Color Correction Effet OFF");
		}
	} 
	
	/*********************************************************************************
	 *
	 * INTERFACE METHDOS, from AnimQualityListener
	 *
	 *********************************************************************************/
	
	public void AnimationEnabled(bool animEnabled) {
		//Debug.Log("AnimateSky.AnimationEnabled enambled: " + animEnabled);
		if(animEnabled == false) {
			Init();	
			cameraEffectScript.enabled = false;	
		}
		else {
			if(imageEffectSupported && cameraEffectScript.textureRamp) {
				cameraEffectScript.enabled = true;	
			} 	
		}
		enabled = animEnabled;
		animationEnabled = enabled;
		
	} 
	
	/*********************************************************************************
	 *
	 * INTERFACE METHDOS, from TimeOfDayListener
	 *
	 *********************************************************************************/

	public void SetSunset() {
		Debug.Log("AnimateSky.SetSunset()");
		if(animationEnabled) {
			StopSunsetAnimationIfNeeded();
			timeOfDay = GameTime.TIME_OF_DAY.SUNSET;
			sunsetScreen=0;
			Invoke("NextSunsetStep", sunsetSpeed);
		}
		else {
			//Debug.Log("-- QualityFactor  below Good, skipping sunset animation");
			SetNight();	
		}
	}
	
	public void SetDay() {
		Debug.Log("AnimateSky.SetDay()");
		StopSunsetAnimationIfNeeded();
		timeOfDay = GameTime.TIME_OF_DAY.DAY;
		SetSkyTextureSet(dayTexureSet);
	}
	
	// This is called if it is a day and dunrise time was skipped
	public void SetNight() {
		Debug.Log("AnimateSky.SetNight()");
		StopSunsetAnimationIfNeeded(); 
		timeOfDay = GameTime.TIME_OF_DAY.NIGHT;
		SetSkyTextureSet(nightTextureSet);		
	}
	
	public void TimeSkipped() {
		if(timeOfDay == GameTime.TIME_OF_DAY.SUNSET) {
			SetNight();
		}	
	}
	
	/*********************************************************************************
	 *
	 * PRIVATE METHDOS
	 *
	 *********************************************************************************/
	
	private void NextSunsetStep() {
		//Debug.Log("AnimateSky.NextSunsetStep(): sunsetScreen=" + sunsetScreen);
		if(sunsetAnimationTextureSet.Length > sunsetScreen) {
			SetSkyTextureSet(sunsetAnimationTextureSet[sunsetScreen++]);
			Invoke("NextSunsetStep", sunsetSpeed);
		}		
		else {
			//Debug.Log("AnimateSky.NextSunsetStep() ready. It is night");
			timeOfDay = GameTime.TIME_OF_DAY.NIGHT;
			SetSkyTextureSet(nightTextureSet);
		}
	}
	
	private void StopSunsetAnimationIfNeeded() {
		if(timeOfDay == GameTime.TIME_OF_DAY.SUNSET) {
			CancelInvoke("NextSunsetStep");
		}	
	}
	
	private void SetSkyTextureSet(SkyTextureSet set) {
		// Day, Night and Sunset are created by changing textures of
		// Sky
		sky.SetTexture("_Far", set.far);
		sky.SetTexture("_Medium", set.medium);
		sky.SetTexture("_Near", set.near);
		// Set postprocessing effect for nights and sunset
		SetRamp(set.ramp);
		
		if(animationEnabled) {
			enabled = set.animate;
		}
	}
	
	private void SetRamp(Texture2D ramp) {
		// Here we manipulate Color Correction Postprosessing Effect Ramp
		// in order change colors  in nights and sunset times
		
		if(!imageEffectSupported) { return; }
		
		cameraEffectScript.textureRamp = ramp;
		
		// We do not use image effect if QualityFactor is low
		if(!animationEnabled) {return;}
		
		// If ramp is null the effect should be disabled
		// effect is only used in Deck area, so we enble it only there, 
		// otherwise effect is dispabled
		if(ramp && pcState.GetCurrentArea().name.Equals("Deck")) {
			cameraEffectScript.enabled = true;		
		}
		else {	
			cameraEffectScript.enabled = false;		
		}
	
		
	} 
	
	private void Init() {
		offsetFar=0.0f;
		offsetMedium=0.0f;
		offsetNear=0.0f;
		sky.SetTextureOffset ("_Far", new Vector2(0, 0));
		sky.SetTextureOffset ("_Medium", new Vector2(0, 0));
		sky.SetTextureOffset ("_Near", new Vector2(0, 0));

	}
}
