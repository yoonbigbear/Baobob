namespace BaobabDispatcher
{
	using System.Linq.Expressions;
	using System.Reflection;

	public partial class HandlerDispatcher
	{
		public void BindHandlers(Assembly assembly)
		{
			// 어셈블리 내의 모든 타입 가져오기
			Type[] types = assembly.GetTypes();

			// 각 타입을 순회하면서 메서드 검사
			foreach (Type type in types)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Public
					| BindingFlags.NonPublic
					| BindingFlags.Instance
					| BindingFlags.Static);

				foreach (MethodInfo methodinfo in methods)
				{
					var attributes = methodinfo.GetCustomAttributes(typeof(BaobabDispatcherAttribute), false);
					if (attributes.Length > 0)
					{
						CompileMethodToDelegate(methodinfo);
					}
				}
			}
		}

		private void CompileMethodToDelegate(MethodInfo method)
		{
			// 비동기 메서드 여부 확인
			bool isAsync = method.ReturnType == typeof(Task)
				|| (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
				|| method.ReturnType == typeof(void)
				&& method.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>() != null;

			// 파마리터 타입 확인
			var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
			// 델리게이트 타입 확인
			var delegateType = Expression.GetDelegateType(parameterTypes.Concat(new[] { method.ReturnType }).ToArray());
			// 리턴 타입
			var returnType = method.ReturnType;
			// 델리게이트 생성
			var del = Delegate.CreateDelegate(delegateType, this, method);

			if (!typeof(IMessage).IsAssignableFrom(parameterTypes[0]))
				throw new HandlerParameterNotMatchException();

			// 인스턴스의 매개변수
			var instanceParameter = Expression.Constant(this);

			// 메서드의 매개변수
			var messageParameter = Expression.Parameter(typeof(IMessage), parameterTypes[0].Name);

			// 메시지를 구체적인 타입으로 캐스팅
			var castMessageParameter = Expression.Convert(messageParameter, parameterTypes[0]);

			// 메서드의 호출
			var methodCall = Expression.Call(instanceParameter, method, castMessageParameter);

			// IMesesage Key
			var messageId = Activator.CreateInstance(parameterTypes[0]) as IMessage;

			if (isAsync)
			{
				// lambda 생성
				var lambda = Expression.Lambda<Func<IMessage, Task>>(methodCall, messageParameter);
				var func = lambda.Compile();
				packetHandler = packetHandler.Add(messageId!.MessageID, new AsyncCaller(func));
			}
			else
			{
				var lambda = Expression.Lambda<Action<IMessage>>(methodCall, messageParameter);
				var action = lambda.Compile();
				packetHandler = packetHandler.Add(messageId!.MessageID, new Caller(action));
			}
		}
	}
}