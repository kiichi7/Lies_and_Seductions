using UnityEngine;
using System.Collections;

public class StraightFlush : EvaluatedHand {
	
	private const int TYPE_VALUE = 8;
	
	public StraightFlush(ArrayList straight) : base("straight flush", TYPE_VALUE, straight){
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		return CompareKickers(otherHand);
	}
	
	public override ArrayList GetCardsInOrder(){
		return new ArrayList(kickers);
	}
}
