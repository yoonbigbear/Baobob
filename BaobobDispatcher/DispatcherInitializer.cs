namespace BaobabDispatcher
{
	using Google.FlatBuffers;
	using System.Linq.Expressions;
	using System.Reflection;

	public partial class HandlerDispatcher<T>
	{
		public void BindHandlerIMessageType(Assembly assembly)
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
						CompileMethodToDelegateIMessageType(methodinfo);
					}
				}
			}
		}

		private void CompileMethodToDelegateIMessageType(MethodInfo method)
		{
			// 비동기 메서드 여부 확인
			bool isAsync = method.ReturnType == typeof(Task)
				|| (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
				|| method.ReturnType == typeof(void)
				&& method.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>() != null;

			// 파마리터 타입 확인
			Type[] parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
			if (!typeof(IMessage).IsAssignableFrom(parameterTypes[0]))
				return;
			// 델리게이트 타입 확인
			var delegateType = Expression.GetDelegateType(parameterTypes.Concat(new[] { method.ReturnType, }).ToArray());
			// 델리게이트 생성
			var del = Delegate.CreateDelegate(delegateType, this, method);

			// 인스턴스의 매개변수
			var instanceParameter = Expression.Constant(this);

			// 메서드의 매개변수
			var messageParameter = Expression.Parameter(typeof(T), parameterTypes[0].Name);

			// 메시지를 구체적인 타입으로 캐스팅
			var castMessageParameter = Expression.Convert(messageParameter, parameterTypes[0]);

			// 메서드의 호출
			var methodCall = Expression.Call(instanceParameter, method, castMessageParameter);

			// IMesesage Key
			var messageId = Activator.CreateInstance(parameterTypes[0]) as IMessage;

			if (isAsync)
			{
				// lambda 생성
				var lambda = Expression.Lambda<Func<T, Task>>(methodCall, messageParameter);
				var func = lambda.Compile();
				packetHandler = packetHandler.Add(messageId!.MessageID, new AsyncCaller<T>(func));
			}
			else
			{
				var lambda = Expression.Lambda<Action<T>>(methodCall, messageParameter);
				var action = lambda.Compile();
				packetHandler = packetHandler.Add(messageId!.MessageID, new Caller<T>(action));
			}
		}

		public void BindHandlerIFlatbufferType(Assembly assembly)
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
						CompileMethodToDelegateIFlatbufferType(methodinfo);
					}
				}
			}
		}

		private void CompileMethodToDelegateIFlatbufferType(MethodInfo method)
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

			if (!typeof(IFlatbufferObject).IsAssignableFrom(parameterTypes[0]))
				throw new HandlerParameterNotMatchException();

			// 인스턴스의 매개변수
			var instanceParameter = Expression.Constant(this);

			// 메서드의 매개변수
			var messageParameter = Expression.Parameter(typeof(IFlatbufferObject), parameterTypes[0].Name);

			// 메시지를 구체적인 타입으로 캐스팅
			var castMessageParameter = Expression.Convert(messageParameter, parameterTypes[0]);

			// 메서드의 호출
			var methodCall = Expression.Call(instanceParameter, method, castMessageParameter);

			// flatbuffer Key
			var messageId = Activator.CreateInstance(parameterTypes[0]) as IFlatbufferObject;

			if (isAsync)
			{
				// lambda 생성
				var lambda = Expression.Lambda<Func<T, Task>>(methodCall, messageParameter);
				var func = lambda.Compile();
				packetHandler = packetHandler.Add(parameterTypes[0].FullName!.GetHashCode(), new AsyncCaller<T>(func));
			}
			else
			{
				var lambda = Expression.Lambda<Action<T>>(methodCall, messageParameter);
				var action = lambda.Compile();
				packetHandler = packetHandler.Add(parameterTypes[0].FullName!.GetHashCode(), new Caller<T>(action));
			}
		}
	}
}