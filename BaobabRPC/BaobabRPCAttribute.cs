namespace BaobabRPC
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	[AttributeUsage(AttributeTargets.Method)]
	public class BaobabRPCIDLAttribute : Attribute
	{
	}

	public class RPCProxy
	{
	}

	public class RPCHelper<T>
	{
		public static List<MemberInfo> GetIDLMemberInfo()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			// 어셈블리 내의 모든 타입 가져온다.
			Type[] types = Assembly.GetExecutingAssembly().GetTypes();

			// 각 타입을 순회하면서 [BaobabRPCIDL]를 사용 중인 메서드 검사
			foreach (Type type in types)
			{
				var interfaces = type.GetInterfaces();

				foreach (var itf in interfaces)
				{
					if (itf.GetCustomAttributes(typeof(BaobabRPCIDLAttribute), false).Length > 0)
					{
						list.AddRange(itf.FindMembers(MemberTypes.Method, BindingFlags.Default, null, null));
					}
				}
			}
			return list;
		}
	}

	/// <summary>
	/// IDL이 된다.
	/// </summary>
	public interface ISampleRPC
	{
		[BaobabRPCIDL]
		public void SampleRPC();
	}

	public class RpcRequest
	{
		public string Method { get; set; }
		public object[] Parameters { get; set; }
	}

	public class RpcResponse
	{
		public object Result { get; set; }
		public string Error { get; set; }
	}
}