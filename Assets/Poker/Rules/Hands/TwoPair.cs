using UnityEngine;
using System.Collections;

public class TwoPair : EvaluatedHand {
	
	private const int TYPE_VALUE = 2;
	
	private ArrayList pair1;
	private ArrayList pair2;
	
	public TwoPair(ArrayList pair1, ArrayList pair2, ArrayList kickers) : base("two pair", TYPE_VALUE, kickers){
		this.pair1 = pair1;
		this.pair2 = pair2;
	}
	
	protected override int CompareCardValues(EvaluatedHand otherHand){
		TwoPair otherTwoPair = (TwoPair)otherHand;
		int pair1Comparison = ((Card)pair1[0]).CompareTo((Card)otherTwoPair.pair1[0]);
		if (pair1Comparison != 0){
			return pair1Comparison;
		} else {
			int pair2Comparison = ((Card)pair2[0]).CompareTo((Card)otherTwoPair.pair2[0]);
			if (pair2Comparison != 0){
				return pair2Comparison;
			} else {
				return CompareKickers(otherHand);
			}
		}
	}
	
	public override ArrayList GetCardsInOrder(){
		ArrayList cards = new ArrayList(pair1);
		cards.AddRange(pair2);
		cards.AddRange(kickers);
		return cards;
	}
}