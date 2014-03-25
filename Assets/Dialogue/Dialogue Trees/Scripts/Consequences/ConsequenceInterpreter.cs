/**********************************************************************
 *
 * CLASS 
 *
 * Copyright 2008 Tommi Horttana, Petri Lankoski, Jari Suominen
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License 
 * at http://www.apache.org/licenses/LICENSE-2.0 Unless required 
 * by applicable law or agreed to in writing, software distributed 
 * under the License is distributed on an "AS IS" BASIS, WITHOUT 
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 *
 ***********************************************************************/
using UnityEngine;
using System.Collections;
using System;

public class ConsequenceInterpreter {
	
	public static ArrayList Interpret(string code, string defaultNPCName, Conversation conversation){
		return InterpretConsequenceNodes(code, defaultNPCName, conversation);
	}	
	
	public static ArrayList InterpretConsequenceNodes(string code, string defaultNPCName, Conversation conversation){
		ArrayList consequenceNodes = new ArrayList();
		if (!code.EndsWith(";")){
			code = code + ";";
		}
		while (code.Contains(";")){
			int firstSemicolonIndex =  code.IndexOf(';');
			string firstPart = code.Substring(0, firstSemicolonIndex);
			ConsequenceNode consequenceNode = InterpretConsequenceNode(firstPart, defaultNPCName, conversation);
			code = code.Substring(firstSemicolonIndex + 1);
			consequenceNodes.Add(consequenceNode);
		}		
		return consequenceNodes;
	}
	
	private static ConsequenceNode InterpretConsequenceNode(string code, string defaultNPCName, Conversation conversation){
		string assignmentOperator;
		if (code.Contains("->")){
			assignmentOperator = "->";
		} else if (code.Contains("=")){
			assignmentOperator = "=";
		} else if (code.Contains("+")){
			assignmentOperator = "+";
		} else if (code.Contains("-")){
			assignmentOperator = "-";
		} else {
			assignmentOperator = null;
		}
		if (assignmentOperator != null){
			return InterpretValueAdjuster(code, assignmentOperator, defaultNPCName, conversation);	
		} else {	
			return InterpretAction(code, defaultNPCName, conversation);
		}
	}
	
	private static ConsequenceNode InterpretValueAdjuster(string code, string assignmentOperator, string defaultNPCName, Conversation conversation){
		int operatorIndex = code.IndexOf(assignmentOperator);
		string variablePart = code.Substring(0, operatorIndex);
		string valuePart = CodeInterpreter.RemoveSpaces(code.Substring(operatorIndex + assignmentOperator.Length));
		string npcNameNoDefault = CodeInterpreter.GetNPCName(variablePart);
		string variableName = CodeInterpreter.GetVariableName(variablePart);
		if (variableName.StartsWith("_")){
			throw new InvalidScriptException("Can't assign values to external variables!");
		} else if (variableName.EndsWith("$")){
			return new SetString(variableName, npcNameNoDefault, CodeInterpreter.InterpretString(valuePart, defaultNPCName, conversation), conversation);	
		} else if (variableName.EndsWith("#")){
			if (assignmentOperator.Equals("=")){
				return new SetInteger(variableName, npcNameNoDefault, CodeInterpreter.InterpretNumeric(valuePart, defaultNPCName, conversation), conversation);
			} else if (assignmentOperator.Equals("+")){
				return new AddToInteger(variableName, npcNameNoDefault, CodeInterpreter.InterpretNumeric(valuePart, defaultNPCName, conversation), conversation);
			} else if (assignmentOperator.Equals("-")){
				return new SubtractFromInteger(variableName, npcNameNoDefault, CodeInterpreter.InterpretNumeric(valuePart, defaultNPCName, conversation), conversation);
			} else {
				throw new InvalidScriptException("Invalid assignment script: " + code);
			}
		} else {
			return InterpretImpressionAdjuster(assignmentOperator, variableName, CodeInterpreter.InterpretNumeric(valuePart, defaultNPCName, conversation), npcNameNoDefault, conversation);
		}
	}
	
	private static ImpressionAdjuster InterpretImpressionAdjuster(string assignmentOperator, string impressionName, NumericOperand operand, string npcName, Conversation conversation){
		bool excludeSpeaker = npcName != null && npcName.ToLower().Equals("others");
		if (excludeSpeaker){
			npcName = null;
		}
		if (assignmentOperator.Equals("->")){
			return new ImpressionPush(impressionName, operand, excludeSpeaker, npcName);
		} else if (assignmentOperator.Equals("=")){
			return new SetImpression(impressionName, operand, excludeSpeaker, npcName);
		} else {
			throw new InvalidScriptException("No arrow or equals sign in consequence script!");
		}
	}
	
	private static ArrayList CreateArgumentList(string code){
		ArrayList arguments = new ArrayList();
		if (!code.Contains("(")){
			return arguments;
		} else {
			int firstBracketIndex = code.IndexOf("(");
			int secondBracketIndex = code.IndexOf(")");
			string argumentPart = code.Substring(firstBracketIndex + 1, secondBracketIndex - firstBracketIndex - 1);
			while (argumentPart.Contains(",")){
				int firstCommaIndex = argumentPart.IndexOf(",");
				string firstPart = CodeInterpreter.RemoveSpaces(argumentPart.Substring(0, firstCommaIndex));
				/*if (firstPart.StartsWith("\"") && firstPart.EndsWith("\"")){
					firstPart = firstPart.Substring(1, firstPart.Length - 2);
				}*/
				arguments.Add(firstPart);
				argumentPart = argumentPart.Substring(firstCommaIndex + 1, argumentPart.Length - firstCommaIndex - 1);
			}
			arguments.Add(CodeInterpreter.RemoveSpaces(argumentPart));
		}
		return arguments;
	}
	
	private static AbstractActionConsequenceNode InterpretAction(string code, string defaultNPCName, Conversation conversation){
		string npcName = CodeInterpreter.GetNPCName(code, defaultNPCName);
		string command = CodeInterpreter.GetVariableName(code);
		GameObject actor = CharacterManager.GetCharacter(npcName);
		ArrayList arguments = CreateArgumentList(command);
		if (command.Equals("_dance")){
			if (actor == CharacterManager.GetPC()){
				throw new InvalidScriptException("Can't command Abby to dance with an NPC! Do it the other way around!");
			} else {
				return new FollowConsequenceNode(actor, FollowAction.Reason.DANCE);
			}
		} else if (command.StartsWith("_give_item(")){
			return new GiveItemConsequenceNode(actor, (string)arguments[0], false);
		} else if (command.StartsWith("_split_item(")){
			return new GiveItemConsequenceNode(actor, (string)arguments[0], true);
		} else if (command.StartsWith("_give_money(")){
			return new GiveMoneyConsequenceNode(actor, (string)arguments[0], CodeInterpreter.InterpretNumeric((string)arguments[1], defaultNPCName, conversation));
		} else if (command.StartsWith("_order(")){
			return new BringPCDrinkConsequenceNode(actor, CodeInterpreter.InterpretString((string)arguments[0], defaultNPCName, conversation));
		} else if (command.Equals("_kiss")){
			if (actor == CharacterManager.GetMajorNPC("Chris") || actor == CharacterManager.GetPC()){
				return new ChrisKissConsequenceNode(actor);
			} else {
				return new KissConsequenceNode(actor, CharacterManager.GetPC());
			}
		} else if (command.Equals("_sex")){
			if (actor == CharacterManager.GetPC()){
				throw new InvalidScriptException("Can't command Abby to have sex with an NPC! Do it the other way around!");
			} else {
				return new FollowConsequenceNode(actor, FollowAction.Reason.SEX);
			}
		} else if (command.Equals("_poker")){
			if (actor == CharacterManager.GetPC()){
				throw new InvalidScriptException("Can't command Abby to play poker with an NPC! Do it the other way around!");
			} else {
				return new FollowConsequenceNode(actor, FollowAction.Reason.POKER_WITH_MONEY);
			}
		} else if (command.Equals("_poker_without_money")){
			if (actor == CharacterManager.GetPC()){
				throw new InvalidScriptException("Can't command Abby to play poker with an NPC! Do it the other way around!");
			} else {
				return new FollowConsequenceNode(actor, FollowAction.Reason.POKER_WITHOUT_MONEY);
			}
		} else if (command.Equals("_poker_with_money")){
			if (actor == CharacterManager.GetPC()){
				throw new InvalidScriptException("Can't command Abby to play poker with an NPC! Do it the other way around!");
			} else {
				return new FollowConsequenceNode(actor, FollowAction.Reason.POKER_WITH_MONEY);
			}
		} else if(command.StartsWith("_wait")) {
			if (actor == CharacterManager.GetPC()){
				throw new InvalidScriptException("Can't command Abby to wait! Do it the other way around!");
			}
			else {
				return new FollowConsequenceNode(actor, FollowAction.Reason.WAIT);
			}
		}
		 else if(command.StartsWith("_follow_drink")) {
			if (actor == CharacterManager.GetPC()){
				throw new InvalidScriptException("Can't command Abby to follow for drink! Do it the other way around!");
			}
			else {
				return new FollowConsequenceNode(actor, FollowAction.Reason.DRINK);
			}
		 }
		else if (command.StartsWith("_blackmail(")){
			return new BlackmailConsequenceNode(CharacterManager.GetPC(), (string)arguments[0]);
		} else if (command.Equals("_victory_love")){
			return new EndingActionConsequenceNode(CharacterManager.GetPC(), Ending.VICTORY_LOVE);
		} else if (command.Equals("_victory_no_love")){
			return new EndingActionConsequenceNode(CharacterManager.GetPC(), Ending.VICTORY_NO_LOVE);
		} else if (command.Equals("_lost_bet")){
			return new EndingActionConsequenceNode(CharacterManager.GetPC(), Ending.LOST_BET);
		} else {
			throw new InvalidScriptException("\"" + code + "\" is not a valid consequence script!");
		}
	}
}
