using System;
using System.CodeDom;
using System.Text;

namespace Mediachase.Commerce.Marketing.Validators.Providers.DomParser
{
	/// <summary>
	/// Divides the string into tokens.
	/// </summary>
	public class Tokenizer
	{
		private CharEnumerator _En;
		private bool _IsInvalid = false;
		private Token _PrevToken = Token.NullToken;

		/// <summary>
		/// A tokenizer is always constructed on a single string.  Create one tokenizer per string.
		/// </summary>
		/// <param name="s">string to tokenize</param>
		public Tokenizer(string s)
		{
			_En = s.GetEnumerator();
			MoveNext();
		}

		/// <summary>
		/// Moves to the next character.  If there are no more characters, then the tokenizer is
		/// invalid.
		/// </summary>
		private void MoveNext() 
		{
			if (!_En.MoveNext())
				_IsInvalid = true;
		}

		/// <summary>
		/// Allows access to the token most recently parsed.
		/// </summary>
		public Token Current 
		{
			get { return _PrevToken; }
		}

		/// <summary>
		/// Indicates that there are no more characters in the string and tokenizer is finished.
		/// </summary>
		public bool IsInvalid 
		{
			get { return _IsInvalid; }
		}

		/// <summary>
		/// Is the current character a letter or underscore?
		/// </summary>
		public bool IsChar
		{
			get 
			{
				if (_IsInvalid) return false;
				return ((_En.Current >= 'A' && _En.Current <= 'Z') || (_En.Current >= 'a' && _En.Current <= 'z') ||
					_En.Current == '_');
			}
		}

		/// <summary>
		/// Is the current character a dot (".")?
		/// </summary>
		public bool IsDot 
		{
			get 
			{ 
				if (_IsInvalid) return false;
				return _En.Current == '.'; 
			}
		}

		/// <summary>
		/// Is the current character a comma?
		/// </summary>
		public bool IsComma 
		{
			get 
			{
				if (_IsInvalid) return false;
				return _En.Current == ',';
			}
		}

		/// <summary>
		/// Is the current character a number?
		/// </summary>
		public bool IsNumber 
		{
			get 
			{ 
				if (_IsInvalid) return false;
				return (_En.Current >= '0' && _En.Current <= '9'); 
			}
		}

		/// <summary>
		/// Is the current character a whitespace character?
		/// </summary>
		public bool IsSpace 
		{
			get 
			{ 
				if (_IsInvalid) return false;
				return (_En.Current == ' ' || _En.Current == '\t'); 
			}
		}

		/// <summary>
		/// Is the current character an operator?
		/// </summary>
		public bool IsOperator 
		{
			get 
			{
				if (_IsInvalid) return false;
				switch (_En.Current) 
				{
					case '>' : 
					case '<' : 
					case '=' : 
					case '-' : 
					case '+' : 
					case '!' : 
					case '/' : 
					case '%' :
					case '*' : 
					case '&' : 
					case '|' : 
					case '(' : 
					case ')' : 
					case '[' : 
					case ']' : 
					case '"' : return true;
					default: return false;
				}
			}
		}

		/// <summary>
		/// Gets the next token in the string.  Reads as many characters as necessary to retrieve
		/// that token.
		/// </summary>
		/// <returns>next token</returns>
		public Token GetNextToken()
		{
			if (_IsInvalid)
				return Token.NullToken;

			Token token;
			if (IsChar)
				token = GetString();
			else if (IsComma)
			{
				token = new Token(",", TokenType.Comma, TokenPriority.None);
				MoveNext();
			}
			else if (IsDot)
			{
				token = new Token(".", TokenType.Dot, TokenPriority.None);
				MoveNext();
			}
			else if (IsNumber)
				token = GetNumber();
			else if (IsSpace)
			{
				// Eat space and do recursive call.
				MoveNext();
				token = GetNextToken();
			}
			else if (IsOperator)
				token = GetOperator();
			else
			{
				token = Token.NullToken;
				MoveNext();
			}

			_PrevToken = token;
			return token;
		}

		/// <summary>
		/// Anything that starts with a character is considered a string.  This could be a 
		/// primitive quoted string, a primitive expression, or an identifier
		/// </summary>
		/// <returns></returns>
		private Token GetString()
		{
			// Handle empty strings
			if (_PrevToken.Type == TokenType.Quote && _En.Current == '"')
			{
				MoveNext();
				return new Token(string.Empty, TokenType.Primitive, TokenPriority.None);
			}
			StringBuilder sb = new StringBuilder();
			sb.Append(_En.Current);
			while (true)
			{
				if (_IsInvalid) break;
				MoveNext();
				if (_IsInvalid) break;

				if (IsChar)
					sb.Append(_En.Current);
				else if (IsNumber)
					sb.Append(_En.Current);
				else
				{
					if (_PrevToken.Type == TokenType.Quote)
					{
						if (_En.Current == '"')
						{
							MoveNext();
							break;
						}
						else if (_En.Current == '\\')
						{
							// In the case of \, we'll add that character and whatever character follows it.
							sb.Append(_En.Current);
							MoveNext();
							if (!_IsInvalid)
								sb.Append(_En.Current);
						}
						else
							sb.Append(_En.Current);
					}
					else
						break;
				}
			}
			string s = sb.ToString();

			// "false" or "true" is a primitive expression.
			if (s == "false" || s == "true")
				return new Token(Boolean.Parse(s), TokenType.Primitive, TokenPriority.None);

			// The previous token was a quote, so this is a primitive string.
			if (_PrevToken.Type == TokenType.Quote)
				return new Token(s, TokenType.Primitive, TokenPriority.None);

			// The default is that the string indicates an identifier.
			return new Token(s, TokenType.Identifier, TokenPriority.None);
		}

		/// <summary>
		/// A token that starts with a number can be an integer, a long, or a double.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// An integer is the default for numbers.  Numbers can also be followed by a
		/// l, L, d, or D character to indicate a long or a double value respectively.
		/// Any numbers containing a dot (".") are considered doubles.
		/// </remarks>
		private Token GetNumber()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(_En.Current);
			bool isDouble = false;
			bool isLong = false;
			bool cont = true;
			while (cont) 
			{
				if (_IsInvalid) break;
				MoveNext();
				if (_IsInvalid) break;

				if (IsNumber)
					sb.Append(_En.Current);
				else if (IsChar)
				{
					switch (_En.Current) 
					{
						case 'D': 
						case 'd': 
							isDouble = true; 
							MoveNext();
							if (IsChar || IsNumber)
							{
								sb.Append(_En.Current);
								throw new ArgumentException("Invalid number: " + sb.ToString());
							}
							else
								cont = false;
							break;
						case 'L': 
						case 'l': 
							isLong = true;
							MoveNext();
							if (IsChar || IsNumber)
							{
								sb.Append(_En.Current);
								throw new ArgumentException("Invalid number: " + sb.ToString());
							}
							else
								cont = false;
							break;
						default:
							sb.Append(_En.Current);
							throw new ArgumentException("Invalid number: " + sb.ToString());
					}
				}
				else if (IsDot)
				{
					sb.Append(_En.Current);
					if (isDouble)
						// The number has already been marked as a double, which means it already
						// contains a number.
						throw new ArgumentException("Invalid number: " + sb.ToString());
					else 
						isDouble = true;
				}
				else
					break;
			}
			string s = sb.ToString();
			if (isLong)
				return new Token(Int64.Parse(s), TokenType.Primitive, TokenPriority.None);
			if (isDouble)
				return new Token(Double.Parse(s), TokenType.Primitive, TokenPriority.None);
			return new Token(Int32.Parse(s), TokenType.Primitive, TokenPriority.None);
		}

		/// <summary>
		/// Some operators take more than one character.  Also, the tokenizer is able to 
		/// categorize the token's priority based on what kind of operator it is.
		/// </summary>
		/// <returns></returns>
		private Token GetOperator()
		{
			string op = new string(_En.Current, 1);
			switch (_En.Current) 
			{
				case '<' : 
				case '=' : 
				case '>' : 
					MoveNext();
					if (_En.Current == '=')
					{
						op += _En.Current;
						MoveNext();
					}
					return new Token(op, TokenType.Operator, TokenPriority.Equality);
				case '-' :
					MoveNext();
					if (_PrevToken.Type == TokenType.Primitive || _PrevToken.Type == TokenType.Identifier 
						|| _PrevToken.Type == TokenType.CloseParens)
						return new Token(op, TokenType.Operator, TokenPriority.PlusMinus);
					else
						return new Token(op, TokenType.Operator, TokenPriority.UnaryMinus);
				case '+' : 
					MoveNext();
					return new Token(op, TokenType.Operator, TokenPriority.PlusMinus);
				case '!' :
					MoveNext();
					if (_En.Current == '=') 
					{
						op += _En.Current;
						MoveNext();
						return new Token(op, TokenType.Operator, TokenPriority.Equality);
					}
					else
						return new Token(op, TokenType.Operator, TokenPriority.Not);
				case '*' : 
				case '/' :
					MoveNext();
					return new Token(op, TokenType.Operator, TokenPriority.MulDiv);
				case '%' :
					MoveNext();
					return new Token(op, TokenType.Operator, TokenPriority.Mod);
				case '|' :
					MoveNext();
					if (_En.Current == '|')
					{
						op += _En.Current;
						MoveNext();
					}
					return new Token(op, TokenType.Operator, TokenPriority.Or);
				case '&' :
					MoveNext();
					if (_En.Current == '&')
					{
						op += _En.Current;
						MoveNext();
					}
					return new Token(op, TokenType.Operator, TokenPriority.And);
				case '(' :
					MoveNext();
					return new Token(op, TokenType.OpenParens, TokenPriority.None);
				case ')' : 
					MoveNext();
					return new Token(op, TokenType.CloseParens, TokenPriority.None);
				case '[' :
					MoveNext();
					return new Token(op, TokenType.OpenBracket, TokenPriority.None);
				case ']' : 
					MoveNext();
					return new Token(op, TokenType.CloseBracket, TokenPriority.None);
				case '"' :
					// When we detect a quote, we can just ignore it since the user doesn't really need to know about it.
					MoveNext();
					_PrevToken = new Token(op, TokenType.Quote, TokenPriority.None);
					return GetString();
			}
			return Token.NullToken;
		}
	}
}
