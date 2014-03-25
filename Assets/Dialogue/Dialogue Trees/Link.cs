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
using System.Xml;
using System;

public class Link : LineInLoading {

	private bool choiceOnly;
	private int linkedID;
	private ArrayList linkedLines;
	
	public Link(XmlElement element, LineCatalogue catalogue){
		choiceOnly = element.GetAttribute("choice_only").Equals("true");
		linkedID = Int32.Parse(element.GetAttribute("linked_id"));
		linkedLines = null;
	}
	
	public ArrayList ConnectLinks(LineCatalogue catalogue){
		if (linkedLines == null){
			Line line = catalogue.GetLine(linkedID);
			linkedLines = new ArrayList();
			if (choiceOnly){
				ArrayList responses = line.GetResponses();
				ArrayList originalResponses = new ArrayList(responses);
				for (int i = 0; i < originalResponses.Count; i++){
					LineInLoading response = (LineInLoading)originalResponses[i];
					if (response.IsLink()){
						ArrayList replacingList = response.ConnectLinks(catalogue);
						int index = responses.IndexOf(response);
						responses.RemoveAt(index);
						responses.InsertRange(i, replacingList);
					}
				}
				linkedLines.AddRange(responses);
			} else {
				linkedLines.Add(line);
			}
		}
		return linkedLines;
	}

	public bool IsLink(){
		return true;
	}
}
