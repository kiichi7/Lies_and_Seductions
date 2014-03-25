using UnityEngine;
using System.Collections;

public class FourOfAKind : EvaluatedHand {
	
	private const int TYPE_VALUE = 7;
	
	private ArrayList fourOfAKind;
	
	public FourOfAKind(ArrayList fourOfAKind, ArrayList kickers) : base("four of a kind", TYPE_VALUE, kickers){
		this.fourOfAKind = fourOfAKind;
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		FourOfAKind otherFourOfAKind = (FourOfAKind)otherHand;
		int fourOfAKindComparison = ((Card)fourOfAKind[0]).CompareTo((Card)otherFourOfAKind.fourOfAKind[0]);
		if (fourOfAKindComparison != 0){
			return fourOfAKindComparison;
		} else {
			return CompareKickers(otherHand);
		}
	}
	
	public override ArrayList GetCardsInOrder(){
		ArrayList cards = new ArrayList(fourOfAKind);
		cards.AddRange(kickers);
		return cards;
	}
}
