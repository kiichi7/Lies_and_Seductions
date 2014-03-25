/****************************************************************************************
 *
 * CLASS CurrentImpression
 * 
 * (c) authors, 2008
 *
 * Authors: 
 * - Tommi Horttana
 * - Petri Lankoski
 *
 * PL: Class seems not to work with Serializer as a hash name. Removing it and using string...
 *     Anyway, encapsulation below just adds unnessary cycles in referenceing Impression.name (it
 *	   is lot faster to point directly to name than use a method to get it.
 *
 ****************************************************************************************/
using UnityEngine;
using System.Collections;
using System.Xml;
using System;

[Serializable()]
public class Impression {
	
	/*public string name;
	
	public Impression() {}
	
	public Impression(XmlElement element){
		name = element.GetAttribute("name");
	}
	
	public string GetName(){
		return name;
	}*/
}
