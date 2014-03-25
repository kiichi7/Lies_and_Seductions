using UnityEngine;
using System.Collections;

public abstract class EvaluatedHand {
	
	private string name;
	private int typeValue;
	protected ArrayList kickers;
	
	public EvaluatedHand(string name, int typeValue, ArrayList kickers){
		this.name = name;
		this.typeValue = typeValue;
		this.kickers = kickers;
	}
	
	public string GetName(){
		return name;
	}
	
	public int CompareTo(EvaluatedHand otherHand){
		if (typeValue < otherHand.typeValue){
			return -1;
		} else if (typeValue > otherHand.typeValue){
			return 1;
		} else {
			return CompareCardValues(otherHand);
		}
	}
	
	protected int CompareKickers(EvaluatedHand otherHand){
		for (int i = 0; i < kickers.Count; i++){
			int kickerComparison = ((Card)kickers[i]).CompareTo((Card)otherHand.kickers[i]);
			if (kickerComparison != 0){
				return kickerComparison;
			}
		}
		return 0;
	}
	
	protected abstract int CompareCardValues(EvaluatedHand otherHand);
	
	public abstract ArrayList GetCardsInOrder();

}
