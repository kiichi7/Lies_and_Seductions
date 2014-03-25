using UnityEngine;
using UnityEditor;
using System.Collections;

public class ConnectWaypoints : ScriptableObject {
	/*[MenuItem ("Custom/Connect Waypoints")]
	static void MenuConnectWaypoints(){        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
         
        foreach(Transform transform1 in transforms){
        	foreach (Transform transform2 in transforms){
        		Waypoint waypoint1 = (Waypoint)transform1.GetComponent("Waypoint");
        		if (waypoint1 == null){
        			waypoint1 = (Waypoint)transform1.GetComponent("TransitWaypoint");
        		}
        		Waypoint waypoint2 = (Waypoint)transform2.GetComponent("Waypoint");
        		if (waypoint2 == null){
        			waypoint2 = (Waypoint)transform2.GetComponent("TransitWaypoint");
        		}
        		if (waypoint1 != waypoint2){
	        		for (int i = 0; i < waypoint1.neighbours.Length; i++){
    	    			if (waypoint1.neighbours[i] == null){
        					waypoint1.neighbours[i] = waypoint2;
        					break;
    	    			} else if (waypoint1.neighbours[i] == waypoint2){
    	    				break;
    	    			}
        			}
        		}
        	}
        }
	}
	
	[MenuItem ("Custom/Disconnect Waypoints")]
	static void MenuDisconnectWaypoints(){        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
         
        foreach(Transform transform1 in transforms){
        	foreach (Transform transform2 in transforms){
        		Waypoint waypoint1 = (Waypoint)transform1.GetComponent("Waypoint");
        		if (waypoint1 == null){
        			waypoint1 = (Waypoint)transform1.GetComponent("TransitWaypoint");
        		}
        		Waypoint waypoint2 = (Waypoint)transform2.GetComponent("Waypoint");
        		if (waypoint2 == null){
        			waypoint2 = (Waypoint)transform2.GetComponent("TransitWaypoint");
        		}
        		if (waypoint1 != waypoint2){
	        		for (int i = 0; i < waypoint1.neighbours.Length; i++){
    	    			if (waypoint1.neighbours[i] == waypoint2){
    	    				waypoint1.neighbours[i] = null;
    	    			}
	        		}
        		}
        	}
        }
	}*/

}