using UnityEngine;
using System.Collections;

public class Straight : EvaluatedHand {
	
	private const int TYPE_VALUE = 4;
	
	public Straight(ArrayList straight) : base("straight", TYPE_VALUE, straight){
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		return CompareKickers(otherHand);
	}
			
	public override ArrayList GetCardsInOrder(){
		return new ArrayList(kickers);
	}
}
