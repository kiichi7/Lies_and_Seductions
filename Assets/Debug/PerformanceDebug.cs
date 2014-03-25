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
 * Usage:
 * - add to scene
 * - static void PerformanceDebug.Reset() resets counters
 * - static string PerformanceDebug.GetAsString() returns stringwith framerate info (avrage, stddev, min, max, N)
 *
 * - static float PerformanceDebug.GetAverageFramerate() returns average frame rate
 * - static float PerformanceDebug.GetStdDev() returns standard deviation
 * - static float PerformanceDebug.GetMaxFramerate() returns maximum framerate perceived
 * - static float PerformanceDebug.GetMinFramerate() returns minimun framerate perceived
 * - static float PerformanceDebug.GetSamplesCount() return the number of samples collected
 *
 ******************************************************************************************/
using UnityEngine;
using System.Collections;

public class PerformanceDebug : MonoBehaviour {

	/*************************************************************************************
	 *
	 * PUBLIC MEMBERS, set these in inspector
	 *
	 *************************************************************************************/

	// how many samples we will collect at max
	public int sampleWindowSize = 1000;

	/*************************************************************************************
	 *
	 * PRIVATE MEMBERS
	 *
	 *************************************************************************************/

	private float maxFrameRate;
	private float minFrameRate;
	private float frameRateSum;
	
	// the number of samples
	private int frame;
	
	// Sort() uses this to keep track if samples have been sorted or not
	bool sorted;
	
	private ArrayList samples;

	static private PerformanceDebug s_instance = null;

	/*************************************************************************************
	 *
	 * OVERLOADED METHODS from MonoBehaviour
	 *
	 *************************************************************************************/

	// Use this for initialization
	void Start () {
		s_instance = GetInstance();
		Init();
	}
	
	// Update is called once per frame
	void Update () {
		//float framerate = 1.0f/Time.deltaTime; // Frames per seconds
		//samples[frame] = framerate;
		samples.Add(1.0f/Time.deltaTime);
		frame++;
		if(frame >= sampleWindowSize) {
			enabled = false;
		}		
	}
	
	/*************************************************************************************
	 *
	 * PUBLIC STATIC METHODS
	 *
	 *************************************************************************************/

	public static void Reset() {
		PerformanceDebug.s_instance.Init();
	}
	
	public static string GetAsString() {
		
		if (Debug.isDebugBuild == false) { return "Not a Debug Build"; }
		
		if(PerformanceDebug.s_instance==null) {return "";}
		
		string retval = "Framerate:\n- avrg=" + PerformanceDebug.GetAverageFramerate();
		retval += " stddev=" + PerformanceDebug.GetStdDev(); 
		retval += " \n- min=" + PerformanceDebug.GetMinFramerate();
		retval += " max=" + PerformanceDebug.GetMaxFramerate();
		float low25 = PerformanceDebug.GetLow25Average();
		if (low25 < 0) {
			retval += "\n- low 25% avrg=too few samples";
		}
		else {
			retval += "\n- low 25% avrg=" + low25;
		}
		retval += "\n- N=" + PerformanceDebug.GetSamplesCount() +"\n";
		
		return retval;
	}
	
	public static float GetAverageFramerate() {
		return PerformanceDebug.s_instance.CalcAverage();
	}
			
	public static float GetMaxFramerate() {
		return PerformanceDebug.s_instance.FindMax();
	}
	
	public static float GetStdDev() {
		if (Debug.isDebugBuild == false) { return -1.0f; }
		return 	PerformanceDebug.s_instance.StdDev();
	}
	
	public static float GetMinFramerate() {
		if (Debug.isDebugBuild == false) { return -1.0f; }
		return PerformanceDebug.s_instance.FindMin();
	}
	
	public static int GetSamplesCount() {
		if (Debug.isDebugBuild == false) { return -1; }
		return PerformanceDebug.s_instance.frame;
	}
	
	public static float GetLow25Average() {
		if (Debug.isDebugBuild == false) { return -1.0f; }
		return PerformanceDebug.s_instance.CalcLow25Average();
	}

	
	/*************************************************************************************
	 *
	 * PRIVATE METHODS
	 *
	 *************************************************************************************/

	
	private float CalcAverage() {
		float sum = 0.0f;
		foreach(float n in samples) {
			sum += n;	
		}
		return sum/(float)frame;
	}
	
	
	private float FindMax() {
		Sort();
		return (float) samples[frame-1];
	}	
	
	private float StdDev() {
		float SumOfSqrts = 0;
		float avgr = GetAverageFramerate();
		foreach(float sam in samples) {
			SumOfSqrts += Mathf.Pow((sam - avgr), 2);
		}
		float framecount = (float)frame;
		return Mathf.Sqrt(SumOfSqrts/(framecount-1));
	}
	
	private float FindMin() { 
		Sort();
		return (float)samples[0];
		
	}
	
	private void Sort() {
		// We sort samples only if it is not sorted yet
		if(!sorted) { 
			sorted=true;
			samples.Sort();
		}
	}
	
	private float CalcLow25Average() {
		Sort();
		int first25 = frame/4;
		float lowSum = 0.0f;
		int count = 0;
		foreach(float t in samples) {
			if(count<first25) {
				lowSum += t;
				count++;
			}
			else {
				break;	
			}	
		}
		return lowSum/(float)first25;	
	}
	
	static private PerformanceDebug GetInstance() {
		if (s_instance == null) {
          	s_instance =  (PerformanceDebug)FindObjectOfType(typeof (PerformanceDebug)) as PerformanceDebug;
            if (s_instance == null)
            	Debug.LogError("PerformanceDebug instance not found, you need to add it to the scene");
         }
         else {
         	Debug.LogError("Multiple PerformanceDebug instances, there can be only one.");	
         }
         return s_instance;		
	}
	
	private void Init() {
		if (Debug.isDebugBuild == false) { 
			enabled = false;
			return;
		}
		enabled = true;
		samples = new ArrayList();
		frame = 0;
		
		// Sort() uses this to keep track if samples have been sorted or not
		sorted = false;
	}


	
	
}
