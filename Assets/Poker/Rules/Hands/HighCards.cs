using UnityEngine;
using System.Collections;

public class HighCards : EvaluatedHand {
	
	private const int TYPE_VALUE = 0;
	
	public HighCards(ArrayList kickers) : base("high cards", TYPE_VALUE, kickers){
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		return CompareKickers(otherHand);
	}
	
		
	public override ArrayList GetCardsInOrder(){
		return new ArrayList(kickers);
	}
}
