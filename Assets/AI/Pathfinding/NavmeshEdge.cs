/**********************************************************************
 *
 * CLASS NavmeshEdge
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
using System;

public class NavmeshEdge : MonoBehaviour {
	
	public const int NAVMESH_LAYER_NUMBER = 10;
	
	private const float COLLIDER_WIDTH = 0.001f;
	private const float COLLIDER_HEIGHT = 5.0f;

	public NavmeshVertex[] vertices = new NavmeshVertex[2];
	public NavmeshPolygon[] polygons = new NavmeshPolygon[2];
	private bool activated = false;
	
	/*public void Awake() {
		enabled = false;	
	}*/
	
	public static NavmeshEdge Create(NavmeshVertex vertex1, NavmeshVertex vertex2){
		GameObject edgeGO = new GameObject("Navmesh Edge");
		edgeGO.AddComponent("NavmeshEdge");
		edgeGO.tag = "NavmeshEdge";
		NavmeshEdge edge = (NavmeshEdge)edgeGO.GetComponent("NavmeshEdge");
		
		edge.vertices[0] = vertex1;
		edge.vertices[1] = vertex2;
		
		edge.ResetPosition();
		edge.ResetCollider();
		
		return edge;
	}
	
	public void ResetPosition(){
		Vector3 vertex1Position = vertices[0].transform.position;
		Vector3 vertex2Position = vertices[1].transform.position;
		transform.position = new Vector3((vertex1Position.x + vertex2Position.x) / 2, (vertex1Position.y + vertex2Position.y) / 2, (vertex1Position.z + vertex2Position.z) / 2);
		transform.parent = vertices[0].transform.parent;
		transform.LookAt(vertex1Position, Vector3.up);
	}
	
	public void ResetCollider(){
		if (collider != null){
			Component.DestroyImmediate(collider);
		}
		if (!IsPortal()){
			BoxCollider boxCollider = (BoxCollider)gameObject.AddComponent("BoxCollider");
			boxCollider.size = new Vector3(COLLIDER_WIDTH, COLLIDER_HEIGHT, Vector3.Distance(vertices[0].transform.position, vertices[1].transform.position));
			gameObject.layer = NAVMESH_LAYER_NUMBER;
		}
	}
	
	public bool IsPortal(){
		return polygons[0] != null && polygons[1] != null;
	}
	
	public bool IsDirectlyToTheRightOf(Vector3 point){
		float crossingZ = point.z;
		Vector3 endPoint1 = vertices[0].transform.position;
		Vector3 endPoint2 = vertices[1].transform.position;
		Vector3 leftEndPoint;
		Vector3 rightEndPoint;
		if (endPoint1.x < endPoint2.x){
			leftEndPoint = endPoint1;
			rightEndPoint = endPoint2;
		} else {
			leftEndPoint = endPoint2;
			rightEndPoint = endPoint1;
		}
		float slope = (rightEndPoint.z - leftEndPoint.z) / (rightEndPoint.x - leftEndPoint.x);
		float crossingX = leftEndPoint.x + (crossingZ - leftEndPoint.z) / slope;
		
		Vector3 endPointWithLowerZ;
		Vector3 endPointWithHigherZ;
		if (endPoint1.z > endPoint2.z){
			endPointWithLowerZ = endPoint2;
			endPointWithHigherZ = endPoint1;
		} else {
			endPointWithLowerZ = endPoint1;
			endPointWithHigherZ = endPoint2;
		}
		bool result = crossingZ > endPointWithLowerZ.z && crossingZ < endPointWithHigherZ.z && crossingX > point.x;
		return result;
	}
	
	public NavmeshVertex GetOtherVertex(NavmeshVertex vertex){
		foreach (NavmeshVertex otherVertex in vertices){
			if (vertex != otherVertex){
				return otherVertex;
			}
		}
		Debug.LogError("Other NavmeshVertex not found in NavmeshEdge!");
		return null;
	}
	
	public NavmeshPolygon GetOtherPolygon(NavmeshPolygon polygon){
		foreach (NavmeshPolygon otherPolygon in polygons){
			if (polygon != otherPolygon){
				return otherPolygon;
			}
		}
		Debug.LogError("Other NavmeshPolygon not found in NavmeshEdge!");
		return null;
	}		
	
	public void AddPolygon(NavmeshPolygon polygon){
		for (int i = 0; i < polygons.Length; i++){
			if (polygons[i] == null){
				polygons[i] = polygon;
				break;
			} else if (i == polygons.Length - 1){
				Debug.LogError("More than 2 NavmeshPolygons connected to one NavmeshEdge!");
			}
		}
	}
	
	public void SetActivated(bool activated){
		this.activated = activated;
	}
	
	public void Remove(){
		for (int i = 0; i < polygons.Length; i++){
			if (polygons[i] != null){
				polygons[i].Remove();
				polygons[i] = null;
			}
		}
		GameObject.DestroyImmediate(gameObject);
	}
	
	public void OnDrawGizmos(){
		/*GameObject selectedGO = Selection.activeGameObject;
		foreach (NavmeshVertex vertex in vertices){
			if (vertex.gameObject == selectedGO){
				Gizmos.DrawIcon(transform.position, "Navmesh Edge.bmp");
			}
		}
		foreach (NavmeshPolygon polygon in polygons){
			if (polygon.gameObject == selectedGO){
				Gizmos.DrawIcon(transform.position, "Navmesh Edge.bmp");
			}
		}*/
		Gizmos.DrawIcon(transform.position, "Navmesh Edge.bmp");
		if (activated){
			Gizmos.color = Color.cyan;
		} else {
			Gizmos.color = Color.yellow;
		}
		try {
			Gizmos.DrawLine(vertices[0].transform.position, vertices[1].transform.position);
		} catch (NullReferenceException ex){
			Debug.LogError("Error drawing gizmo: " + name);
		}
	}
}
