using UnityEngine;
using System.Collections;

public class OnePair : EvaluatedHand {
	
	private const int TYPE_VALUE = 1;
	
	private ArrayList pair;
	
	public OnePair(ArrayList pair, ArrayList kickers) : base("one pair", TYPE_VALUE, kickers){
		this.pair = pair;
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		OnePair otherOnePair = (OnePair)otherHand;
		int pairComparison = ((Card)pair[0]).CompareTo((Card)otherOnePair.pair[0]);
		if (pairComparison != 0){
			return pairComparison;
		} else {
			return CompareKickers(otherHand);
		}
	}
	
	public override ArrayList GetCardsInOrder(){
		ArrayList cards = new ArrayList(pair);
		cards.AddRange(kickers);
		return cards;
	}
}
