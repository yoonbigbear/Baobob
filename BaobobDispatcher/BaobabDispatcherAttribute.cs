using System.Linq.Expressions;
using System.Reflection;

namespace BaobabDispatcher
{
	[AttributeUsage(AttributeTargets.Method)]
	public class BaobabDispatcherAttribute : Attribute
	{
	}
}