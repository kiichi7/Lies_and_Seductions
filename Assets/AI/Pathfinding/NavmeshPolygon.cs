/**********************************************************************
 *
 * CLASS NavmeshPolygon
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

public class NavmeshPolygon : MonoBehaviour {

	public NavmeshEdge[] edges;
	public Area area;
	private ArrayList portals;
	
	public static NavmeshPolygon Create(NavmeshEdge[] edges){
		GameObject navmeshPolygonGO = new GameObject("Navmesh Polygon");
		navmeshPolygonGO.tag = "NavmeshPolygon";
		navmeshPolygonGO.AddComponent("NavmeshPolygon");
		NavmeshPolygon navmeshPolygon = (NavmeshPolygon)navmeshPolygonGO.GetComponent("NavmeshPolygon");

		navmeshPolygon.edges = edges;
				
		foreach (NavmeshEdge edge in edges){
			edge.AddPolygon(navmeshPolygon);
		}
		
		navmeshPolygon.ResetPosition();
		
		return navmeshPolygon;
	}
	
	public Area GetArea(){
		return area;
	}
	
	public void ResetPosition(){
		float x = 0.0f;
		float y = 0.0f;
		float z = 0.0f;
		foreach (NavmeshEdge edge in edges){
			x += edge.transform.position.x;
			y += edge.transform.position.y;
			z += edge.transform.position.z;
		}
		x = x / edges.Length;
		y = y / edges.Length;
		z = z / edges.Length;
		transform.position = new Vector3(x, y, z);		
		transform.parent = edges[0].transform.parent;
	}
	
	public void Start(){
		portals = new ArrayList();
		foreach (NavmeshEdge edge in edges){
			if (edge.IsPortal()){
				portals.Add(edge);
			}
		}
	}
	
	public bool IsPointWithin(Vector3 point){
		bool oddNumberOfCrosses = false;
		foreach (NavmeshEdge edge in edges){
			if (edge.IsDirectlyToTheRightOf(point)){
				oddNumberOfCrosses = !oddNumberOfCrosses;
			}
		}
		return oddNumberOfCrosses;
	}
	
	public NavmeshEdge GetPortal(NavmeshPolygon otherPolygon){
		foreach (NavmeshEdge portal in portals){
			if (portal.GetOtherPolygon(this) == otherPolygon){
				return portal;
			}
		}
		Debug.LogError("The polygons don't have any common edges!");
		return null;
	}
	
	public ArrayList GetPortals(){
		return portals;
	}
	
	public void Remove(){
		GameObject.DestroyImmediate(gameObject);
	}
	
	public void OnDrawGizmos(){
		Gizmos.DrawIcon(transform.position, "Navmesh Polygon.bmp");
	}
}
