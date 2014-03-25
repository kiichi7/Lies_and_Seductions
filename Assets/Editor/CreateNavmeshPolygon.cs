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
using UnityEditor;
using System.Collections;


public class CreateNavmeshPolygon : ScriptableObject {
	
	private static ArrayList vertexSelection;
	private static ArrayList edgeSelection;
	
	[MenuItem ("Custom/Begin Navmesh Selection ^q")]
	public static void MenuBeginNavmeshSelection(){
		if (edgeSelection != null){
			foreach (NavmeshEdge edge in edgeSelection){
				edge.SetActivated(false);
			}
		}
		vertexSelection = new ArrayList();
		GameObject vertexGO = Selection.activeGameObject;
		NavmeshVertex vertex = vertexGO.GetComponent(typeof(NavmeshVertex)) as NavmeshVertex;
		vertexSelection.Add(vertex);
		edgeSelection = new ArrayList();
	}
	
	[MenuItem ("Custom/Select Navmesh Vertex ^w")]
	public static void MenuSelectNavmeshVertex(){
		if (vertexSelection == null || vertexSelection.Count == 0){
			Debug.LogError("CreateNavmeshPolygon.MenuSelectNavmeshVertex(): Begin Navmesh Vertex Selection first!");
			return;
		}
		GameObject vertexGO = Selection.activeGameObject;
		NavmeshVertex vertex = vertexGO.GetComponent(typeof(NavmeshVertex)) as NavmeshVertex;
		if (vertex != null && vertex != (NavmeshVertex)vertexSelection[vertexSelection.Count - 1]){
			NavmeshEdge edge = vertex.AddActiveEdge((NavmeshVertex)vertexSelection[vertexSelection.Count - 1]);
			edgeSelection.Add(edge);
			if (vertex == vertexSelection[0]){
				CreatePolygon();
			} else {
				vertexSelection.Add(vertex);
			}
		}
	}
	
	[MenuItem ("Custom/Create Navmesh Node at selection position ^d")]
	public static void MenuCreateNavmeshNodeAtSelectionPosition(){
		Transform currentSelection = Selection.activeTransform;
		Vector3 vertexPosition;
		if (currentSelection != null){
			vertexPosition = currentSelection.position;
		} else {
			vertexPosition = Vector3.zero;
		}
		GameObject vertexGO = new GameObject("Navmesh Vertex");
		vertexGO.tag = "NavmeshVertex";
		vertexGO.AddComponent("NavmeshVertex");
		vertexGO.transform.position = vertexPosition;
		vertexGO.transform.parent = currentSelection.transform.parent;
		Selection.activeGameObject = vertexGO;
	}
	
	private static void CreatePolygon(){
		if (edgeSelection.Count < 3){
			Debug.Log("Select at least 3 Navmesh Vertices (by using the Select Navmesh Vertex command) before creating the Navmesh Polygon!");
			return;
		} else {
			NavmeshEdge[] edges = new NavmeshEdge[edgeSelection.Count];
			for (int i = 0; i < edgeSelection.Count; i++){
				edges[i] = (NavmeshEdge)edgeSelection[i];
				if (edges[i] == null){
					Debug.LogError("Invalid selection for nav mesh creation!");
				}
			}
			NavmeshPolygon.Create(edges);
			foreach (NavmeshEdge edge in edgeSelection){
				edge.SetActivated(false);
			}
		}
	}
	
	[MenuItem ("Custom/Fix Navmesh ^e")]
	static void MenuFixNavmesh(){
		GameObject[] navmeshVertexGOs = GameObject.FindGameObjectsWithTag("NavmeshVertex");
		for (int i = 0; i < navmeshVertexGOs.Length; i++){
			NavmeshVertex vertex = navmeshVertexGOs[i].GetComponent(typeof(NavmeshVertex)) as NavmeshVertex;
			vertex.ResetPosition();
		}
		GameObject[] navmeshEdgeGOs = GameObject.FindGameObjectsWithTag("NavmeshEdge");
		for (int i = 0; i < navmeshEdgeGOs.Length; i++){
			NavmeshEdge edge = navmeshEdgeGOs[i].GetComponent(typeof(NavmeshEdge)) as NavmeshEdge;
			edge.ResetPosition();
			edge.ResetCollider();
		}
		GameObject[] navmeshPolygonGOs = GameObject.FindGameObjectsWithTag("NavmeshPolygon");
		for (int i = 0; i < navmeshPolygonGOs.Length; i++){
			NavmeshPolygon polygon = navmeshPolygonGOs[i].GetComponent(typeof(NavmeshPolygon)) as NavmeshPolygon;
			polygon.ResetPosition();
			for (Transform treeNode = polygon.transform; treeNode != null; treeNode = treeNode.parent){
				Area area = treeNode.GetComponent(typeof(Area)) as Area;
				if (area != null){
					polygon.area = area;
					break;
				}
			}
		}
	}
	
	[MenuItem ("Custom/Remove Navmesh Item ^r")]
	static void MenuRemoveNavmeshItem(){
		GameObject toRemove = Selection.activeGameObject;
		if (toRemove.tag.Equals("NavmeshPolygon")){
			NavmeshPolygon polygon = toRemove.GetComponent(typeof(NavmeshPolygon)) as NavmeshPolygon;
			polygon.Remove();
		} else if (toRemove.tag.Equals("NavmeshEdge")){
			NavmeshEdge edge = toRemove.GetComponent(typeof(NavmeshEdge)) as NavmeshEdge;
			edge.Remove();
		} else if (toRemove.tag.Equals("NavmeshVertex")){
			NavmeshVertex vertex = toRemove.GetComponent(typeof(NavmeshVertex)) as NavmeshVertex;
			vertex.Remove();
		}
	}
	
	[MenuItem ("Custom/Number Navmesh Items ^n")]
	static void MenuNumberNavmeshItems(){
		GameObject[] navmeshVertexGOs = GameObject.FindGameObjectsWithTag("NavmeshVertex");
		for (int i = 0; i < navmeshVertexGOs.Length; i++){
			navmeshVertexGOs[i].name = "NavmeshVertex (" + i + ")";
		}
		GameObject[] navmeshEdgeGOs = GameObject.FindGameObjectsWithTag("NavmeshEdge");
		for (int i = 0; i < navmeshEdgeGOs.Length; i++){
			navmeshEdgeGOs[i].name = "NavmeshEdge (" + i + ")";
		}
		GameObject[] navmeshPolygonGOs = GameObject.FindGameObjectsWithTag("NavmeshPolygon");
		for (int i = 0; i < navmeshPolygonGOs.Length; i++){
			navmeshPolygonGOs[i].name = "NavmeshPolygon (" + i + ")";
		}
			
	}

}