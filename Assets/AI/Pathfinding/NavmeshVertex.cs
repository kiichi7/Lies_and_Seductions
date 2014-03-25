/**********************************************************************
 *
 * CLASS NavmeshVertex
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

public class NavmeshVertex : MonoBehaviour {

	//public NavmeshPolygon[] polygons = new NavmeshPolygon[10];
	public NavmeshEdge[] edges = new NavmeshEdge[10];

	public NavmeshEdge AddActiveEdge(NavmeshVertex neighbour){
		NavmeshEdge edgeToActivate = null;
		foreach (NavmeshEdge edge in edges){
			if (edge != null && edge.GetOtherVertex(this) == neighbour){
				edgeToActivate = edge;
			}
		}
		if (edgeToActivate == null){
			edgeToActivate = NavmeshEdge.Create(this, neighbour);
			AddEdge(edgeToActivate);
			neighbour.AddEdge(edgeToActivate);
		}
		edgeToActivate.SetActivated(true);
		return edgeToActivate;
	}
	
	public void ResetPosition(){
		transform.position = new Vector3(transform.position.x, transform.parent.position.y, transform.position.z);
	}
	
	public void AddEdge(NavmeshEdge edge){
		for (int i = 0; i < edges.Length; i++){
			if (edges[i] == null){
				edges[i] = edge;
				return;
			} else if (i == edges.Length - 1){
				Debug.LogError("More than 10 NavmeshEdges connected to one NavmeshVertex!");
			}
		}
	}
		
	public void Remove(){
		for (int i = 0; i < edges.Length; i++){
			if (edges[i] != null){
				edges[i].Remove();
				edges[i] = null;
			}
		}
		GameObject.DestroyImmediate(gameObject);
	}

	public void OnDrawGizmos(){
		Gizmos.DrawIcon(transform.position, "Navmesh Vertex.bmp");
	}	
}
