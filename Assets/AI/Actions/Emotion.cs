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

public class Emotion {

	public enum BodyParts {NONE, EYES, FACE, ALL}
	
	public static Emotion LOVE = new Emotion("emotion_love");
	public static Emotion LIKE = new Emotion("emotion_like");
	public static Emotion NEUTRAL = new Emotion("emotion_neutral");
	public static Emotion SAD = new Emotion("emotion_sad");
	public static Emotion DISLIKE = new Emotion("emotion_dislike");
	
	private string animationName;
	
	private Emotion(string animationName){
		this.animationName = animationName;
	}

	public string GetAnimationName(){
		return animationName;
	}
	
	public static Emotion GetEmotion(string xmlName){
		if (xmlName.Equals("love")){
			return LOVE;
		} else if (xmlName.Equals("like")){
			return LIKE;
		} else if (xmlName.Equals("neutral")){
			return NEUTRAL;
		} else if (xmlName.Equals("sad")){
			return SAD;
		} else if (xmlName.Equals("dislike")){
			return DISLIKE;
		} else {
			Debug.LogError("No emotion with the xml name: " + xmlName);
			return null;
		}
	}
}
