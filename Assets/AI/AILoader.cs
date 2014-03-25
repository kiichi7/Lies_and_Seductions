/**********************************************************************
 *
 * CLASS AiLoader
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

public class AILoader : MonoBehaviour {

	public string fileName;
	public bool enableDebug=false;

	private static AILoader instance;
	
	private XmlElement rootElement;
	private ArrayList impressions;
	private Hashtable opinions;
	private Hashtable conversations;
	
	public void Awake(){
		instance = this;
	
		// PL, 28.7.2008, this way we can load the file "fileName" in OSX standalone player,
		// Windows standalone player and Unity Editor.
		string fileNameWithPath = Application.dataPath + "/" + fileName;
	
		XmlTextReader reader = new XmlTextReader(fileNameWithPath);
		reader.WhitespaceHandling = WhitespaceHandling.None;
		reader.Read();
		XmlDocument document = new XmlDocument();
		document.Load(reader);
		rootElement = (XmlElement)document.GetElementsByTagName("root").Item(0);
	
		impressions = LoadImpressions(rootElement);
		opinions = LoadOpinions(rootElement, impressions);
	}
	
	public void Start(){
		//Debug.Log("AILoader,Start()");
		conversations = LoadConversations(rootElement, impressions);
		enabled = false;
	}
	
	public static void Save() {
		foreach(Conversation conversation in instance.conversations.Values) {
			conversation.Save();	
		}	
	}
	
	private static ArrayList LoadImpressions(XmlElement rootElement){
		ArrayList impressions = new ArrayList();
		XmlNodeList impressionNodeList = rootElement.GetElementsByTagName("impression");
		foreach (XmlElement impressionElement in impressionNodeList){
			//Debug.Log("LoadImpressions: " + impressionElement.GetAttribute("name"));
			//impressions.Add(new Impression(impressionElement));
			impressions.Add(impressionElement.GetAttribute("name"));
		}
		return impressions;
	}
	
	private static Hashtable LoadOpinions(XmlElement rootElement, ArrayList impressions){
		Hashtable opinions = new Hashtable();
		XmlNodeList npcNodeList = rootElement.GetElementsByTagName("npc");
		foreach (XmlElement npcElement in npcNodeList){
			string npcName = npcElement.GetAttribute("name");
			Hashtable opinionsOfNPC = new Hashtable();
			XmlNodeList opinionNodeList = npcElement.GetElementsByTagName("opinion");
			foreach (XmlElement opinionElement in opinionNodeList){
				int impressionIndex = Int32.Parse(opinionElement.GetAttribute("impression_index"));
				//Impression impression = (Impression)impressions[impressionIndex];
				string impression = (string)impressions[impressionIndex];
				opinionsOfNPC.Add(impression, new Opinion(Int32.Parse(opinionElement.GetAttribute("multiplier"))));
			}
			opinions.Add(npcName, opinionsOfNPC);
		}
		return opinions;
	}
	
	private static Hashtable LoadConversations(XmlElement rootElement, ArrayList impressions){
		Hashtable conversations = new Hashtable();
		XmlNodeList conversationNodeList = rootElement.GetElementsByTagName("conversation");
		foreach (XmlElement conversationElement in conversationNodeList){
			string name = conversationElement.GetAttribute("name");
			LineCatalogue catalogue = new LineCatalogue();
			conversations.Add(name, new Conversation(conversationElement, catalogue));
		}
		return conversations;
	}
	
	public static string GetImpression(string name){
		foreach (string impression in instance.impressions){
			if (impression.Equals(name)){
				return impression;
			}
		}
		return null;
	}
	
	public static ArrayList GetImpressions(){
		return instance.impressions;
	}
	
	public static Hashtable GetOpinionsOfNPC(string name){
		return (Hashtable)instance.opinions[name];
	}
	
	public static ArrayList GetConversations(){
		return new ArrayList(instance.conversations.Values);
	}
	
	public static Conversation GetConversation(string name){
		return (Conversation)instance.conversations[name];
	}
	
}
