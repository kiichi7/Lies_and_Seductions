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

public class PrerequisiteInterpreter {
	
	public static BooleanOperand Interpret(string code, string defaultNPCName, Conversation conversation){
		return InterpretAndOr(code, defaultNPCName, conversation);
	}
	
	private static int GetLastIndexOutsideBrackets(string code, char charToFind){
		int bracketCount = 0;
		for (int i = code.Length - 1; i >= 0; i--){
			char charAtIndex = code[i];
			if (charAtIndex == charToFind && bracketCount == 0){
				return i;
			} else if (charAtIndex == ')'){
				bracketCount++;
			} else if (charAtIndex == '('){
				bracketCount--;
			}
		}
		return -1;
	}
	
	private static BooleanOperand InterpretAndOr(string code, string defaultNPCName, Conversation conversation){
		int lastIndexAnd = GetLastIndexOutsideBrackets(code, '&');
		int lastIndexOr = GetLastIndexOutsideBrackets(code,'|');
		int lastIndex = Math.Max(lastIndexAnd, lastIndexOr);
		if (lastIndex == -1){
			return InterpretNot(code, defaultNPCName, conversation);
		} else {
			string firstPart = code.Substring(0, lastIndex);
			string lastPart = code.Substring(lastIndex + 1);
			if (lastIndex == lastIndexAnd){
				return new And(InterpretAndOr(firstPart, defaultNPCName, conversation), InterpretNot(lastPart, defaultNPCName, conversation));
			} else {
				return new Or(InterpretAndOr(firstPart, defaultNPCName, conversation), InterpretNot(lastPart, defaultNPCName, conversation));
			}
		}
	}
	
	private static BooleanOperand InterpretNot(string code, string defaultNPCName, Conversation conversation){
		int lastIndexNot = GetLastIndexOutsideBrackets(code, '!');
		if (lastIndexNot == 0){
			return new Not(InterpretLogicBrackets(code.Substring(1), defaultNPCName, conversation));
		} else if (lastIndexNot == -1 || (code.Length >= lastIndexNot + 2 && code[lastIndexNot + 1] == '=')){
			return InterpretLogicBrackets(code, defaultNPCName, conversation);
		} else {
			throw new InvalidScriptException("Incorrect negation clause: " + code);
		}
	}
	
	private static BooleanOperand InterpretLogicBrackets(string code, string defaultNPCName, Conversation conversation){
		code = CodeInterpreter.RemoveSpaces(code);
		if (code.StartsWith("(") && code.EndsWith(")")){
			return InterpretAndOr(code.Substring(1, code.Length - 2), defaultNPCName, conversation);
		} else {
			return InterpretComparison(code, defaultNPCName, conversation);
		}
	}
	
	private static BooleanOperand InterpretComparison(string code, string defaultNPCName, Conversation conversation){
		string comparisonOperator;
		int operatorIndex;
		
		if (code.Contains(">=")){
			comparisonOperator = ">=";
		} else if (code.Contains("<=")){
			comparisonOperator = "<=";
		} else if (code.Contains("!=")){
			comparisonOperator = "!=";
		} else if (code.Contains("=")){
			comparisonOperator = "=";
		} else if (code.Contains(">")){
			comparisonOperator = ">";
		} else if (code.Contains("<")){
			comparisonOperator = "<";
		} else {
			comparisonOperator = null;
		}
		if (comparisonOperator == null){
			throw new InvalidScriptException("No comparison operator: " + code);
		} else {
			operatorIndex = code.IndexOf(comparisonOperator);
			string firstPart = code.Substring(0, operatorIndex);
			string lastPart = code.Substring(operatorIndex + comparisonOperator.Length);
			if (code.Contains("$")){
				return InterpretStringComparison(firstPart, lastPart, comparisonOperator, operatorIndex, defaultNPCName, conversation);
			} else {
				return InterpretNumericComparison(firstPart, lastPart, comparisonOperator, operatorIndex, defaultNPCName, conversation);
			}
		}
	}
	
	private static BooleanOperand InterpretStringComparison(string firstPart, string lastPart, string comparisonOperator, int operatorIndex, string defaultNPCName, Conversation conversation){
		if (comparisonOperator.Equals("=")){
			return new StringEquals(CodeInterpreter.InterpretString(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretString(lastPart, defaultNPCName, conversation));
		} else if (comparisonOperator.Equals("!=")){
			return new StringNotEqualTo(CodeInterpreter.InterpretString(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretString(lastPart, defaultNPCName, conversation));
		} else {
			throw new InvalidScriptException("Invalid comparison operator for a string comparison comparing: " + firstPart + " and " + lastPart);
		}
	}
	

	private static BooleanOperand InterpretNumericComparison(string firstPart, string lastPart, string comparisonOperator, int operatorIndex, string defaultNPCName, Conversation conversation){			
		if (comparisonOperator.Equals(">=")){
			return new GreaterThanOrEqualTo(CodeInterpreter.InterpretNumeric(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretNumeric(lastPart, defaultNPCName, conversation));
		} else if (comparisonOperator.Equals("<=")){
			return new LessThanOrEqualTo(CodeInterpreter.InterpretNumeric(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretNumeric(lastPart, defaultNPCName, conversation));
		} else if (comparisonOperator.Equals("!=")){
			return new NotEqualTo(CodeInterpreter.InterpretNumeric(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretNumeric(lastPart, defaultNPCName, conversation));
		} else if (comparisonOperator.Equals("=")){
			return new Equals(CodeInterpreter.InterpretNumeric(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretNumeric(lastPart, defaultNPCName, conversation));
		} else if (comparisonOperator.Equals(">")){
			return new GreaterThan(CodeInterpreter.InterpretNumeric(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretNumeric(lastPart, defaultNPCName, conversation));
		} else {
			return new LessThan(CodeInterpreter.InterpretNumeric(firstPart, defaultNPCName, conversation), CodeInterpreter.InterpretNumeric(lastPart, defaultNPCName, conversation));
		} 
		
	}
	
}
