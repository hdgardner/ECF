using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;

namespace Mediachase.Commerce.Marketing.Validators.Providers.DomParser
{
	/// <summary>
	/// Parses expressions written in strings into CodeDom expressions.  There is a certain 
	/// amount of context that the parser may need to be familiar with.  This is why the 
	/// parsing methods are not exposed as static.
	/// </summary>
	public class Parser
	{
		private Dictionary<string, CodeTypeReference> _RecognizedTypes;
		private Dictionary<string, CodeTypeReference> _Enums;
		private StringCollection _Fields;

		/// <summary>
		/// A collection of identifiers that should be recognized as types.
		/// </summary>
		public Dictionary<string, CodeTypeReference> RecognizedTypes 
		{
			get { return _RecognizedTypes; }
		}

		/// <summary>
		/// A collection of identifiers that should be recognized as enums.
		/// </summary>
		public Dictionary<string, CodeTypeReference> Enums 
		{
			get { return _Enums; }
		}

		/// <summary>
		/// A collection of names of fields.
		/// </summary>
		public StringCollection Fields 
		{
			get { return _Fields; }
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Parser() 
		{
			_RecognizedTypes = new Dictionary<string, CodeTypeReference>();
			_RecognizedTypes.Add("string", new CodeTypeReference(typeof(string)));
			_RecognizedTypes.Add("int", new CodeTypeReference(typeof(int)));
			_RecognizedTypes.Add("float", new CodeTypeReference(typeof(float)));
			_RecognizedTypes.Add("double", new CodeTypeReference(typeof(double)));
			_RecognizedTypes.Add("DateTime", new CodeTypeReference(typeof(DateTime)));
			_RecognizedTypes.Add("TimeSpan", new CodeTypeReference(typeof(TimeSpan)));
			_RecognizedTypes.Add("Guid", new CodeTypeReference(typeof(Guid)));
			_RecognizedTypes.Add("char", new CodeTypeReference(typeof(char)));
			_RecognizedTypes.Add("long", new CodeTypeReference(typeof(long)));
			_RecognizedTypes.Add("Convert", new CodeTypeReference(typeof(Convert)));
			_RecognizedTypes.Add("decimal", new CodeTypeReference(typeof(decimal)));
			_RecognizedTypes.Add("bool", new CodeTypeReference(typeof(bool)));
			_RecognizedTypes.Add("IEnumerable", new CodeTypeReference(typeof(IEnumerable)));
			_RecognizedTypes.Add("RulesContext", new CodeTypeReference(typeof(RulesContext)));
			_Fields = new StringCollection();
			_Enums = new Dictionary<string, CodeTypeReference>();
		}

		/// <summary>
		/// Parses an expression into a <see cref="CodeExpression"/>.
		/// </summary>
		/// <param name="exp">expression to parse</param>
		/// <returns>CodeDom representing the expression</returns>
		public CodeExpression ParseExpression(string exp) 
		{
			Tokenizer t = new Tokenizer(exp);
			if (!t.IsInvalid)
			{
				t.GetNextToken();
				return ReadExpression(t, TokenPriority.None);
			}
			return null;
		}

		/// <summary>
		/// Recursive method that reads an expression.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="priority"></param>
		/// <returns></returns>
		private CodeExpression ReadExpression(Tokenizer t, TokenPriority priority)
		{
			CodeExpression left = null, right = null;
			bool cont = true, applyNot = false, applyNegative = false;
			while (cont) 
			{
				switch (t.Current.Type)
				{
					case TokenType.Primitive:
						left = new CodePrimitiveExpression(t.Current.ParsedObject);
						t.GetNextToken();
						cont = false;
						break;
					case TokenType.Operator:
					{
						// An operator here is considered a unary operator.
						switch (t.Current.Text)
						{
							case "-": applyNegative = true; break;
							case "!": applyNot = true; break;
							default: throw new Exception("Unexpected operator: " + t.Current.Text);
						}
						t.GetNextToken();
						continue;
					}
					case TokenType.Identifier:
						left = ReadIdentifier(t);
						cont = false;
						break;
					case TokenType.OpenParens:
						t.GetNextToken();
						left = ReadExpression(t, TokenPriority.None);
						t.GetNextToken();
						if (left is CodeTypeReferenceExpression)
							left = new CodeCastExpression((left as CodeTypeReferenceExpression).Type, ReadExpression(t, TokenPriority.None));
						cont = false;
						break;
				}
				if (t.IsInvalid)
					cont = false;
			}
			if (left == null)
				throw new Exception("No expression found.");
			if (applyNot)
				left = new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(false));
			else if (applyNegative)
				left = new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, left);
			if (t.IsInvalid || t.Current.Type == TokenType.CloseParens || t.Current.Type == TokenType.Comma 
				|| t.Current.Type == TokenType.CloseBracket)
				return left;
			cont = true;
			while (cont && !t.IsInvalid) 
			{
				Token token = t.Current;
				switch (token.Type) 
				{
					case TokenType.Operator:
					{
						if (t.Current.Priority < priority) 
							cont = false;
						else 
						{
							// In the case we have an operator, we'll assume it's a binary operator.
							CodeBinaryOperatorType binOp;
							bool notEquals = false;
							switch (token.Text)
							{
								case ">":	binOp = CodeBinaryOperatorType.GreaterThan; break;
								case ">=":	binOp = CodeBinaryOperatorType.GreaterThanOrEqual; break;
								case "<":	binOp = CodeBinaryOperatorType.LessThan; break;
								case "<=":	binOp = CodeBinaryOperatorType.LessThanOrEqual; break;
								case "=":	
								case "==":	binOp = CodeBinaryOperatorType.ValueEquality; break;
								case "!=":	binOp = CodeBinaryOperatorType.ValueEquality; notEquals = true; break;
								case "|":	binOp = CodeBinaryOperatorType.BitwiseOr; break;
								case "||":	binOp = CodeBinaryOperatorType.BooleanOr; break;
								case "&":	binOp = CodeBinaryOperatorType.BitwiseAnd; break;
								case "&&":	binOp = CodeBinaryOperatorType.BooleanAnd; break;
								case "-":	binOp = CodeBinaryOperatorType.Subtract; break;
								case "+":	binOp = CodeBinaryOperatorType.Add; break;
								case "/":	binOp = CodeBinaryOperatorType.Divide; break;
								case "%":	binOp = CodeBinaryOperatorType.Modulus; break;
								case "*":	binOp = CodeBinaryOperatorType.Multiply; break;
								default:	throw new Exception("Unrecognized operator: " + t.Current.Text);
							}
							if (t.IsInvalid)
								throw new Exception("Expected token for right side of binary expression.");
							t.GetNextToken();
							right = ReadExpression(t, token.Priority);
							left = new CodeBinaryOperatorExpression(left, binOp, right);

							// If the operator was the not equals operator, we just negate the previous binary expression.
							if (notEquals)
								left = new CodeBinaryOperatorExpression(left, binOp, new CodePrimitiveExpression(false));
						}
						break;
					}
					case TokenType.CloseParens:
						//t.GetNextToken();
						cont = false;
						break;
					case TokenType.Dot:
						// A dot could appear after some parentheses.  In this case we need to parse 
						// what's after the dot as an identifier.
						t.GetNextToken();
						right = ReadIdentifier(t);
						CodeExpression ceTemp = right;
						while (true) 
						{
							if (ceTemp is CodeVariableReferenceExpression)
							{
								left = new CodePropertyReferenceExpression(left, (ceTemp as CodeVariableReferenceExpression).VariableName);
								break;
							}
							else if (ceTemp is CodePropertyReferenceExpression)
							{
								CodePropertyReferenceExpression cpre = ceTemp as CodePropertyReferenceExpression;
								if (cpre.TargetObject is CodeThisReferenceExpression)
								{
									cpre.TargetObject = left;
									left = cpre;
									break;
								}
								else 
									ceTemp = cpre.TargetObject;
							}
							else if (ceTemp is CodeFieldReferenceExpression)
							{
								CodeFieldReferenceExpression cfre = ceTemp as CodeFieldReferenceExpression;
								if (cfre.TargetObject is CodeThisReferenceExpression)
								{
									cfre.TargetObject = left;
									left = cfre;
									break;
								}
							}
							else if (ceTemp is CodeMethodInvokeExpression)
							{
								CodeMethodInvokeExpression cmie = ceTemp as CodeMethodInvokeExpression;
								if (cmie.Method.TargetObject is CodeThisReferenceExpression)
								{
									cmie.Method.TargetObject = left;
									left = cmie;
									break;
								}
								else
									ceTemp = cmie.Method.TargetObject;
							}
							else
								throw new Exception("Unexpected identifier found after .");
						}
						cont = false;
						break;
					default:
						throw new Exception("Token not expected: " + token.Text);
				}
			}
			return left;
		}

		/// <summary>
		/// When an identifier is encountered, it could be a number of things.  A single identifer by itself
		/// is considered a variable.  The pattern identifier[.identifier]+ will consider the 
		/// first identifier as a variable and the others as properties.  Any identifier that is followed
		/// by an open parenthesis is considered to be a function call.  Indexes are not handled yet, but
		/// should be handled in the future.  If the identifier is "this" then a this reference is used.
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private CodeExpression ReadIdentifier(Tokenizer t) 
		{
			CodeExpression ce = null;
			Token token = t.Current;
			if (token.Text == "this")
				ce = new CodeThisReferenceExpression();
			else if (_RecognizedTypes.ContainsKey(token.Text))
			{
				// In this case, the identifier is a recognized type.  It is likely a cast expression.
				// We return a CodeTypeReferenceExpression.  The parsing method will see this and 
				// try to match it to open and close parentheses.  It will then turn it into a 
				// CodeCastExpression.  The other possibility is that this method was called from 
				// reading a typeof(), in which case the CodeTypeReferenceExpression is still valid.
				CodeTypeReferenceExpression ctre = new CodeTypeReferenceExpression(_RecognizedTypes[token.Text] as CodeTypeReference);
				t.GetNextToken();
				return ctre;
			}
			else if (Enums.ContainsKey(token.Text))
			{
				// In this case, the identifier is a recognized enum.  We'll return the value of this
				// immediately since enums are always in the same format.
				CodeTypeReferenceExpression ctre = new CodeTypeReferenceExpression(_Enums[token.Text] as CodeTypeReference);
				t.GetNextToken();
				if (t.Current.Type != TokenType.Dot)
					throw new Exception("Expected dot after enum type: " + token.Text);
				t.GetNextToken();
				if (t.Current.Type != TokenType.Identifier)
					throw new Exception("Expected identifier token after enum type: " + token.Text);
				ce = new CodeFieldReferenceExpression(ctre, t.Current.Text);
				t.GetNextToken();
				return ce;
			}
			else if (token.Text == "typeof")
			{
				token = t.GetNextToken();
				if (token.Type != TokenType.OpenParens)
					throw new Exception("Expected open parenthesis after typeof, instead found: " + token.Text);
				t.GetNextToken();
				ce = ReadIdentifier(t);
				if (!(ce is CodeTypeReferenceExpression))
					throw new Exception("Illegal argument in typeof.  Expecting a type.");
				if (t.Current.Type != TokenType.CloseParens)
					throw new Exception("Expected close parenthesis after type in typeof expression, instead found: " + t.Current.Text);
				CodeTypeOfExpression ctoe = new CodeTypeOfExpression((ce as CodeTypeReferenceExpression).Type);
				return ctoe;
			}
			else
				ce = new CodeVariableReferenceExpression(token.Text);
			token = t.GetNextToken();
			bool cont = true;
			while (cont) 
			{
				if (token.Type == TokenType.Dot) 
				{
					token = t.GetNextToken();
					if (token.Type != TokenType.Identifier)
						throw new Exception("Expected identifier after dot, instead found: " + token.Text);
					if (_Fields.Contains(token.Text))
						ce = new CodeFieldReferenceExpression(ce, token.Text);
					else
						ce = new CodePropertyReferenceExpression(ce, token.Text);
					token = t.GetNextToken();
					if (token.Type != TokenType.OpenParens)
						continue;
				}
				else if (token.Type == TokenType.OpenBracket)
				{
					// If an open bracket follows, then we're dealing with an indexer.
					CodeIndexerExpression cie = new CodeIndexerExpression();
					cie.TargetObject = ce;
					ce = cie;
					t.GetNextToken();
					cie.Indices.Add(ReadExpression(t, TokenPriority.None));
					if (t.Current.Type != TokenType.CloseBracket)
						throw new Exception("Unexpected token: " + t.Current.Text);
					token = t.GetNextToken();
					continue;
				}
				if (token.Type == TokenType.OpenParens)
				{
					// An open parenthesis indicates a method, so we'll change from a variable or property reference into 
					// a CodeMethodInvokeExpression.
					if (ce is CodeThisReferenceExpression)
						throw new Exception("Cannot use parentheses after this keyword.");
					CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression();
					if (ce is CodeVariableReferenceExpression)
						cmie.Method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), (ce as CodeVariableReferenceExpression).VariableName);
					else if (ce is CodeFieldReferenceExpression)
					{
						CodeFieldReferenceExpression cfre = ce as CodeFieldReferenceExpression;
						cmie.Method = new CodeMethodReferenceExpression(cfre.TargetObject, cfre.FieldName);
					}
					else // must be a property reference
					{
						CodePropertyReferenceExpression cpre = ce as CodePropertyReferenceExpression;
						cmie.Method = new CodeMethodReferenceExpression(cpre.TargetObject, cpre.PropertyName);
					}
					ce = cmie;
					token = t.GetNextToken();
					if (token.Type != TokenType.CloseParens)
					{
						cmie.Parameters.Add(ReadExpression(t, TokenPriority.None));
						while (true) 
						{
							token = t.Current;
							if (token.Type == TokenType.CloseParens)
							{
								t.GetNextToken();
								break;
							}
							if (token.Type == TokenType.Comma)
							{
								t.GetNextToken();
								cmie.Parameters.Add(ReadExpression(t, TokenPriority.None));
							}
							else
								throw new Exception("Unexpected token: " + token.Text);
						}
					}
					else
						token = t.GetNextToken();
				}
				else
					cont = false;
			}
			return ce;
		}
	}
}
