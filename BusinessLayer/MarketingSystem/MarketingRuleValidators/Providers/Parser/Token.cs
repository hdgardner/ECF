using System;

namespace Mediachase.Commerce.Marketing.Validators.Providers.DomParser
{
	/// <summary>
	/// Represents a token that is parsed out by the <see cref="Tokenizer"/>.
	/// </summary>
	public sealed class Token
	{
		private string _Text;
		private object _ParsedObject;
		private TokenType _Type;
		private TokenPriority _Priority;

		/// <summary>
		/// The text that makes up the token.
		/// </summary>
		public string Text 
		{
			get { return _Text; }
		}

		/// <summary>
		/// If the token can be parsed into a type like an integer, this property holds that value.
		/// </summary>
		public object ParsedObject 
		{
			get { return _ParsedObject; }
		}

		/// <summary>
		/// Token type
		/// </summary>
		public TokenType Type 
		{
			get { return _Type; }
		}

		/// <summary>
		/// Token priority
		/// </summary>
		public TokenPriority Priority 
		{
			get { return _Priority; }
		}

		/// <summary>
		/// Constructor for tokens that are not parsed.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="type"></param>
		/// <param name="priority"></param>
		public Token(string text, TokenType type, TokenPriority priority) 
		{
			_Text = text;
			_Type = type;
			_Priority = priority;
			_ParsedObject = text;
		}

		/// <summary>
		/// Constructor for tokens that are parsed.
		/// </summary>
		/// <param name="parsedObj"></param>
		/// <param name="type"></param>
		/// <param name="priority"></param>
		public Token(object parsedObj, TokenType type, TokenPriority priority)
		{
			_ParsedObject = parsedObj;
			_Text = ParsedObject.ToString();
			_Type = type;
			_Priority = priority;
		}

		/// <summary>
		/// The null token represents a state where the <see cref="Tokenizer"/> encountered an error
		/// or has not begun parsing yet.
		/// </summary>
		public static Token NullToken = new Token("", TokenType.NotAToken, TokenPriority.None);
	}
}
