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
 *
 * Animations for the sea 
 * - Waves by changing the offset of the sea texture
 * - Sunset and sunrise handling for the sea
 *
 **********************************************************************************/
using UnityEngine;
using System.Collections;

public class AnimateWater : MonoBehaviour, AnimQualityListener, TimeOfDayListener  {

	/*********************************************************************************
	 *
	 * PUBLIC VARIABLES
	 *
	 *********************************************************************************/

	// Texture of this material will be animated
	public Material mat;
	
	// Speed for main animaton (water moving like ship is moving)
	public float animationSpeed;
	
	// Defines the speed added noise animation 
	public float noiseSpeed;

	// Defines how much noise we will add to main animation
	public float noise;
	
	public Texture2D dayTexture;
	public Texture2D []waterSunsetTextures;
	public Texture2D nightTexture;
	public float sunsetSpeed;

	/*********************************************************************************
	 *
	 * PRIVATE VARIABLES
	 *
	 *********************************************************************************/

	private float yOffset;
	
	private bool animationEnabled;

	private GameTime.TIME_OF_DAY timeOfDay;
        private int sunsetScreen;

	
	/*********************************************************************************
	 *
	 * OVERLOADED METHDOS, from MonoBehaviour
	 *
	 *********************************************************************************/

	
	// Use this for initialization
	void Start () {
		Init();
		// registering to receice changes in QualitySetting in animation menu
		LocationAnimQuality.RegisterListener(this);
		
		// get the initial value and enable or disable Behaviour based on it
		enabled = LocationAnimQuality.Enabled();
		animationEnabled = 	enabled;

		timeOfDay = GameTime.GetTimeOfDay();
                
                switch(timeOfDay) {
                        case GameTime.TIME_OF_DAY.DAY:
                                SetTexture(dayTexture);
                                break;
                        case GameTime.TIME_OF_DAY.NIGHT:
                                SetTexture(nightTexture);
                                break;  
                }
                GameTime.RegisterTimeOfDayListener(this);

	}
	
	// Update is called once per frame
	void Update () {
		// Actual animation, we do it by manipulating texture offset
		yOffset-=animationSpeed*Time.deltaTime;
		float xOffset = noise*Mathf.Cos(Time.time*noiseSpeed);
		mat.SetTextureOffset ("_MainTex", new Vector2(xOffset, yOffset));
	}
	
	void OnBecameVisible ()  {
		// We only react this if QualitySettings allows enabling Texture animation
		if(animationEnabled) {
			Debug.Log("AnimateWater:Enabled");
			enabled = true;
		}
	}
	
	void OnBecameInvisible () {
		// We only react this if QualitySettings allows enabling Texture animation
		if(animationEnabled) {
			Debug.Log("AnimateWater:Disabled");
			Init();
			enabled = false;
		}
	}

	
	/*********************************************************************************
	 *
	 * OVERLOADED METHDOS, from AnimQualityListener
	 *
	 *********************************************************************************/

	
	public void AnimationEnabled(bool animEnabled) {
		Debug.Log("AnimateWater.AnimationEnabled(" + animEnabled + ")");
		if(animEnabled == false) {
			Init();	
		}
		enabled = animEnabled;	
		animationEnabled = 	enabled;
	} 
	
	private void Init() {
		yOffset = 0.0f;
		mat.SetTextureOffset ("_MainTex", new Vector2(0, 0));
	}

   /*********************************************************************************
    *
    * INTERFACE METHDOS, from TimeOfDayListener
    *
    *********************************************************************************/

    public void SetSunset() {
                Debug.Log("AnimateWater.SetSunset()");
               	if(animationEnabled) {
	                StopSunsetAnimationIfNeeded();
    	            timeOfDay = GameTime.TIME_OF_DAY.SUNSET;
        	        sunsetScreen=0;
                	Invoke("NextSunsetStep", sunsetSpeed);
               	}
               	else {
               		SetNight();
               	}
        }
        
        public void SetDay() {
                Debug.Log("AnimateWater.SetDay()");
                StopSunsetAnimationIfNeeded();
                timeOfDay = GameTime.TIME_OF_DAY.DAY;
                SetTexture(dayTexture);
        }
        
        // This is called if it is a day and dunrise time was skipped
        public void SetNight() {
                Debug.Log("AnimateWater.SetNight()");
                StopSunsetAnimationIfNeeded(); 
                timeOfDay = GameTime.TIME_OF_DAY.NIGHT;
                SetTexture(nightTexture);              
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
     	//Debug.Log("AnimateWater.NextSunsetStep(): sunsetScreen=" + sunsetScreen);
        if(waterSunsetTextures.Length > sunsetScreen) {
                        SetTexture(waterSunsetTextures[sunsetScreen++]);
                        Invoke("NextSunsetStep", sunsetSpeed);
        }
        else {
                        //Debug.Log("AnimateSky.NextSunsetStep() ready. It is night");
                        timeOfDay = GameTime.TIME_OF_DAY.NIGHT;
                        SetTexture(nightTexture);
        }
     }
        
     private void StopSunsetAnimationIfNeeded() {
                if(timeOfDay == GameTime.TIME_OF_DAY.SUNSET) {
                        CancelInvoke("NextSunsetStep");
                }       
     }
        
     private void SetTexture(Texture2D texture) {
                mat.SetTexture("_MainTex", texture);
     }


}
