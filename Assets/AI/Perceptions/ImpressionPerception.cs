using UnityEngine;
using System.Collections;

public class ImpressionPerception : AbstractPerception {

	private ImpressionAdjuster impressionAdjuster;

	public ImpressionPerception(GameObject source, ImpressionAdjuster impressionAdjuster) : base(source) {
		this.source = source;
		this.impressionAdjuster = impressionAdjuster;
	}
	
	public override ImpressionAdjuster GetImpressionAdjuster(){
		return impressionAdjuster;
	}
}
