using UnityEngine;
using System.Collections;

public class ThreeOfAKind : EvaluatedHand {
	
	private const int TYPE_VALUE = 3;
	
	private ArrayList threeOfAKind;
	
	public ThreeOfAKind(ArrayList threeOfAKind, ArrayList kickers) : base("three of a kind", TYPE_VALUE, kickers){
		this.threeOfAKind = threeOfAKind;
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		ThreeOfAKind otherThreeOfAKind = (ThreeOfAKind)otherHand;
		int threeOfAKindComparison = ((Card)threeOfAKind[0]).CompareTo((Card)otherThreeOfAKind.threeOfAKind[0]);
		if (threeOfAKindComparison != 0){
			return threeOfAKindComparison;
		} else {
			return CompareKickers(otherHand);
		}
	}
	
	public override ArrayList GetCardsInOrder(){
		ArrayList cards = new ArrayList(threeOfAKind);
		cards.AddRange(kickers);
		return cards;
	}
}