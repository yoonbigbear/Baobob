namespace BaobabDispatcher
{
	using System;

	public class HandlerNotFoundException : Exception
	{ }

	public class HandlerNotImplementedException : Exception
	{ }

	public class HandlerParameterNotMatchException : Exception
	{ }

	public class DuplicatedHandlerException : Exception
	{ }
}