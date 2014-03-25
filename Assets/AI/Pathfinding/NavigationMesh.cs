/***********************************************************************
 *
 * Copyright 2008 Pedro Teixeira 
 *
 * Licensed under the Apache License, Version   2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * http://www.apache.org/licenses/LICENSE-2.0 Unless required by 
 * applicable law or agreed to in writing, software distributed under 
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES 
 * OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and 
 * limitations under the License. 
 *
 * Source: http://code.google.com/p/ninjaxnagame/
 *********************************************************************
 * 
 * Adapted for "Lies and Seductions" by
 * Tommi Horttana (2008) 
 *
 *********************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class NavigationMesh : MonoBehaviour {
	/// <summary>	
	/// Keep references to nodes.
	/// </summary>
	private List<NavmeshPolygon> polygons;

	private static NavigationMesh instance;
	
	public void Awake(){
		instance = this;
		polygons = new List<NavmeshPolygon>();
		GameObject[] polygonGOs = GameObject.FindGameObjectsWithTag("NavmeshPolygon");
		foreach (GameObject polygonGO in polygonGOs){
			polygons.Add((NavmeshPolygon)polygonGO.GetComponent("NavmeshPolygon"));
		}
		enabled=false;
	}
	
	/// <summary>
	/// Determine in which mesh node the corresponding point is located.
	/// </summary>
	/// <param name="point"></param>
	/// <returns></returns>
	public static NavmeshPolygon FindEnclosingPolygon(Vector3 point) {
		foreach(NavmeshPolygon polygon in instance.polygons) {
			if(polygon.IsPointWithin(point)) {
				return polygon;
			}
		}   
		throw new ApplicationException("Point (" + point + ") must be inside a NavmeshPolygon.");
	}
}