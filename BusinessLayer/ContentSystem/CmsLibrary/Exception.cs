using System;
using System.Runtime.Serialization;

namespace Mediachase.Cms
{
	#region FeatureNotAvailableException
	[Serializable]
	public class FeatureNotAvailableException : System.Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureNotAvailableException"/> class.
        /// </summary>
		public FeatureNotAvailableException()
			: this(null)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureNotAvailableException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
		public FeatureNotAvailableException(Exception innerException)
			: base("Feature not available.", innerException)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureNotAvailableException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected FeatureNotAvailableException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion

	#region AccessDeniedException
	[Serializable]
	public class AccessDeniedException : System.Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
		public AccessDeniedException()
			: this(null)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
		public AccessDeniedException(Exception innerException)
			: base("Access denied.", innerException)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected AccessDeniedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion

	#region LoginDuplicationException
	[Serializable]
	public class LoginDuplicationException : System.Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginDuplicationException"/> class.
        /// </summary>
		public LoginDuplicationException()
			: this(null)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginDuplicationException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
		public LoginDuplicationException(Exception innerException)
			: base("Login duplication.", innerException)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginDuplicationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected LoginDuplicationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion

	#region InvalidAccountException
	[Serializable]
	public class InvalidAccountException: System.Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAccountException"/> class.
        /// </summary>
		public InvalidAccountException()
			: this(null)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAccountException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
		public InvalidAccountException(Exception innerException)
			: base("Invalid Account.", innerException)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAccountException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected InvalidAccountException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion

	#region InvalidPasswordException
	[Serializable]
	public class InvalidPasswordException: System.Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException"/> class.
        /// </summary>
		public InvalidPasswordException()
			: this(null)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
		public InvalidPasswordException(Exception innerException)
			: base("Invalid Password.", innerException)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPasswordException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected InvalidPasswordException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion

	#region NotActiveAccountException
	[Serializable]
	public class NotActiveAccountException: System.Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="NotActiveAccountException"/> class.
        /// </summary>
		public NotActiveAccountException()
			: this(null)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="NotActiveAccountException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
		public NotActiveAccountException(Exception innerException)
			: base("The account is not active.", innerException)
		{
		}
        /// <summary>
        /// Initializes a new instance of the <see cref="NotActiveAccountException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected NotActiveAccountException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
	#endregion
}
