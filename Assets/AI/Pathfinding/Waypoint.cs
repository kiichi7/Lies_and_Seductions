/**********************************************************************
 *
 * CLASS Waypoint
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

public class Waypoint : MonoBehaviour{
	private const float SAFETY_RADIUS = 0.35f;
	
	public bool IsBlocked(bool pcOnly){
		Collider[] blockers = Physics.OverlapSphere(transform.position, SAFETY_RADIUS);
		foreach (Collider blocker in blockers){
			if (!blocker.isTrigger && (!pcOnly || blocker.gameObject == CharacterManager.GetPC())){
				return true;
			}
		}
		return false;
	}
	
	public virtual void OnDrawGizmos(){
		Gizmos.DrawIcon(transform.position, "Key Location.bmp");
	}
}