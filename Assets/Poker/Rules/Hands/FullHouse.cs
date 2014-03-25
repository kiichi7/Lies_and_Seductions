using UnityEngine;
using System.Collections;

public class FullHouse : EvaluatedHand {
	
	private const int TYPE_VALUE = 6;
	
	private ArrayList threeOfAKind;
	private ArrayList pair;
	
	public FullHouse(ArrayList threeOfAKind, ArrayList pair) : base("full house", TYPE_VALUE, new ArrayList()){
		this.threeOfAKind = threeOfAKind;
		this.pair = pair;
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		FullHouse otherFullHouse = (FullHouse)otherHand;
		int threeOfAKindComparison = ((Card)threeOfAKind[0]).CompareTo((Card)otherFullHouse.threeOfAKind[0]);
		if (threeOfAKindComparison != 0){
			return threeOfAKindComparison;
		} else {
			int pairComparison = ((Card)pair[0]).CompareTo((Card)otherFullHouse.pair[0]);
			return pairComparison;
		}
	}
	
	public override ArrayList GetCardsInOrder(){
		ArrayList cards = new ArrayList(threeOfAKind);
		cards.AddRange(pair);
		return cards;
	}
}