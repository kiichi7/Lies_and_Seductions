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
 * CLASS LocationAnimQuality 
 *
 * - public static bool Enabled()
 *          use this in Awake() or Start() to get see if texture animation effects 
 *          should be enabled
 *
 * - public static void RegisterListener(AnimQualityListener listener)
 *          Register as listener to get changes made in Options mene
 *          Listener need to implement interface defined in AnimQuolityListener
 *
 * - public static void UnRegisterListener(AnimQualityListener listener)
 *
 * - public static void Changed(QualityLevel level)
 *          Options menu calls this when QualityLevel is changed
 *
 **********************************************************************************/
using UnityEngine;
using System.Collections;

public class LocationAnimQuality {

	/*************************************************************************
	 *
	 * PRIVATE MEMBERS
	 *
	 ************************************************************************/

	private ArrayList listeners;
	private static LocationAnimQuality s_instance = null;
	
	/*************************************************************************
	 *
	 * PUBLIC STATIC METHODS
	 *
	 ************************************************************************/

	public static bool Enabled() {
		return LocationAnimQuality.IsEnabled(QualitySettings.currentLevel);	
	} 

	public static void Changed(QualityLevel level) {
		LocationAnimQuality.instance().Broadcast(LocationAnimQuality.IsEnabled(level));
	}

	public static void RegisterListener(AnimQualityListener listener) {
		LocationAnimQuality.instance().listeners.Add(listener);
	}
	
	public static void UnRegisterListener(AnimQualityListener listener) {
		LocationAnimQuality.instance().listeners.Remove(listener);
	}

	public static LocationAnimQuality instance() {
		if(s_instance == null) {
			s_instance = new LocationAnimQuality();
			s_instance.Init();
		}
		return s_instance;
	}

    /*************************************************************************
	 *
	 * PRIVATE METHODS
	 *
	 ************************************************************************/
	
	private void Broadcast(bool enabled) {
		foreach(AnimQualityListener listener in listeners) {
			listener.AnimationEnabled(enabled);	
		}
	}

	private void Init() {
		listeners = new ArrayList();	
	}
	
	private static bool IsEnabled(QualityLevel level) {
		bool enabled = true;
		switch(level) {
			case QualityLevel.Simple:
				enabled = false;
				break;
			case QualityLevel.Fast:
				enabled = false;
				break;
			case QualityLevel.Fastest:
				enabled = false;
				break;
		}
		return enabled;
	}

}
