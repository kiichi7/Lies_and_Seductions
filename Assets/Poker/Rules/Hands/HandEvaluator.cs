using UnityEngine;
using System.Collections;

public class HandEvaluator {
	
	private static Card[][] GenerateCardArray(ArrayList cards){
		Card[][] cardArray = new Card[4][];
		for (int i = 0; i < cardArray.Length; i++){
			cardArray[i] = new Card[13];
		}
		foreach (Card card in cards){
			cardArray[(int)card.GetSuit()][card.GetRank() - 2] = card;
		}
		return cardArray;
	}
	
	private static ArrayList FindBiggestXOfAKind(Card[][] cardArray, 
			int numberToFind, ArrayList reservedCards){
		for (int i = cardArray[0].Length - 1; i >= 0; i--){
			ArrayList matchingCards = new ArrayList();
			for (int j = 0; j < cardArray.Length; j++){
				Card card = cardArray[j][i];
				if (card != null && !reservedCards.Contains(card)){
					matchingCards.Add(card);
					if (matchingCards.Count == numberToFind){
						return matchingCards;
					}
				} else if (numberToFind - matchingCards.Count >= cardArray.Length 
						- j){
					break;
				}
			}
		}
		return null;
	}
	
	private static ArrayList FindBiggestXOfAKind(Card[][] cardArray, 
			int numberToFind){
		return FindBiggestXOfAKind(cardArray, numberToFind, new ArrayList());
	}
	
	private static ArrayList FindKickers(Card[][] cardArray, 
			ArrayList reservedCards){
		int numberOfKickers = 5 - reservedCards.Count;
		ArrayList kickers = new ArrayList();
		for (int i = cardArray[0].Length - 1; i >= 0; i--){
			for (int j = 0; j < cardArray.Length; j++){
				Card card = cardArray[j][i];
				if (card != null && !reservedCards.Contains(card)){
					for (int k = 0; k < numberOfKickers; k++){
						if (k == kickers.Count || card.CompareTo((Card)kickers[k]) > 0){
							kickers.Insert(k, card);
							break;
						}
					}
					if (kickers.Count > numberOfKickers){
						kickers.RemoveAt(kickers.Count - 1);
					}
				}
			}
		}
		if (kickers.Count + reservedCards.Count != 5){
			Debug.LogError("Too few kickers!");
		}
		return kickers;
	}
	
	private static StraightFlush CreateStraightFlush(Card[][] cardArray){
		for (int i = 0; i < cardArray.Length; i++){
			ArrayList matchingCards = new ArrayList();
			for (int j = cardArray[0].Length - 1; j >= 0; j--){
				Card card = cardArray[i][j];
				if (card != null){
					matchingCards.Add(card);
					if (matchingCards.Count == 5){
						return new StraightFlush(matchingCards);
					}
				} else {
					matchingCards.Clear();
				}
			}
		}
		return null;
	}
	
	private static FourOfAKind CreateFourOfAKind(Card[][] cardArray){
		ArrayList matchingCards = FindBiggestXOfAKind(cardArray, 4);
		if (matchingCards != null){
			return new FourOfAKind(matchingCards, FindKickers(cardArray, 
					matchingCards));
		} else {
			return null;
		}
	}
	
	private static FullHouse CreateFullHouse(Card[][] cardArray){
		ArrayList threeOfAKind = FindBiggestXOfAKind(cardArray, 3);
		if (threeOfAKind != null){
			ArrayList pair = FindBiggestXOfAKind(cardArray, 2, threeOfAKind);
			if (pair != null){
				return new FullHouse(threeOfAKind, pair);
			} else {
				return null;
			}
		} else {
			return null;
		}
	}
	
	private static Flush CreateFlush(Card[][] cardArray){
		for (int i = 0; i < cardArray.Length; i++){
			ArrayList matchingCards = new ArrayList();
			for (int j = cardArray[0].Length - 1; j >= 0; j--){
				Card card = cardArray[i][j];
				if (card != null){
					matchingCards.Add(card);
					if (matchingCards.Count == 5){
						return new Flush(matchingCards);
					}
				}
			}
		}
		return null;
	}
	
	private static Straight CreateStraight(Card[][] cardArray){
		ArrayList matchingCards = new ArrayList();
		for (int i = cardArray[0].Length - 1; i >= 0; i--){
			for (int j = 0; j < cardArray.Length; j++){
				Card card = cardArray[j][i];
				if (card != null){
					matchingCards.Add(card);
					if (matchingCards.Count == 5){
						return new Straight(matchingCards);
					}
					break;
				} else if (j == cardArray.Length - 1){
					matchingCards.Clear();
				}
			}
		}
		return null;
	}
	
	private static ThreeOfAKind CreateThreeOfAKind(Card[][] cardArray){
		ArrayList matchingCards = FindBiggestXOfAKind(cardArray, 3);
		if (matchingCards != null){
			return new ThreeOfAKind(matchingCards, FindKickers(cardArray, 
					matchingCards));
		} else {
			return null;
		}
	}
	
	private static TwoPair CreateTwoPair(Card[][] cardArray){
		ArrayList pair1 = FindBiggestXOfAKind(cardArray, 2);
		if (pair1 != null){
			ArrayList pair2 = FindBiggestXOfAKind(cardArray, 2, pair1);
			if (pair2 != null){
				ArrayList reservedCards = new ArrayList(pair1);
				reservedCards.AddRange(pair2);
				return new TwoPair(pair1, pair2, FindKickers(cardArray,
						reservedCards));
			} else {
				return null;
			}
		} else {
			return null;
		}
	}
	
	private static OnePair CreateOnePair(Card[][] cardArray){
		ArrayList pair = FindBiggestXOfAKind(cardArray, 2);
		if (pair != null){
			return new OnePair(pair, FindKickers(cardArray, pair));
		} else {
			return null;
		}
	}
	
	private static HighCards CreateHighCards(Card[][] cardArray){
		return new HighCards(FindKickers(cardArray, new ArrayList()));
	}
	
	public static EvaluatedHand EvaluateHand(ArrayList hand, ArrayList board){
		ArrayList cards = new ArrayList(hand);
		cards.AddRange(board);
		Card[][] cardArray = GenerateCardArray(cards);
		EvaluatedHand evaluatedHand = CreateStraightFlush(cardArray);
		if (evaluatedHand == null){
			evaluatedHand = CreateFourOfAKind(cardArray);
		}
		if (evaluatedHand == null){
			evaluatedHand = CreateFullHouse(cardArray);
		}
		if (evaluatedHand == null){
			evaluatedHand = CreateFlush(cardArray);
		}
		if (evaluatedHand == null){
			evaluatedHand = CreateStraight(cardArray);
		}
		if (evaluatedHand == null){
			evaluatedHand = CreateThreeOfAKind(cardArray);
		}
		if (evaluatedHand == null){
			evaluatedHand = CreateTwoPair(cardArray);
		}
		if (evaluatedHand == null){
			evaluatedHand = CreateOnePair(cardArray);
		}
		if (evaluatedHand == null){
			evaluatedHand = CreateHighCards(cardArray);
		}
		return evaluatedHand;
	}

}
