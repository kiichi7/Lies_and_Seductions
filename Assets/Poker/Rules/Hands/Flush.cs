using UnityEngine;
using System.Collections;

public class Flush : EvaluatedHand {
	
	private const int TYPE_VALUE = 5;
	
	public Flush(ArrayList flush) : base("flush", TYPE_VALUE, flush){
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		return CompareKickers(otherHand);
	}
	
	public override ArrayList GetCardsInOrder(){
		return new ArrayList(kickers);
	}
}
