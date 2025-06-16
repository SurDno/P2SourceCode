using System;
using System.Reflection;
using Cofe.Meta;

namespace Engine.Common.Services;

[AttributeUsage(AttributeTargets.Field)]
public class FromLocatorAttribute : MemberAttribute {
	public static readonly Guid Id = Guid.NewGuid();

	public override void ComputeMember(Container container, MemberInfo member) {
		var handler = container.GetHandler(Id);
		var field = member as FieldInfo;
		var type = field.FieldType;
		ComputeHandle handle = (target, data) => {
			var service = ServiceLocator.GetService(type);
			field.SetValue(target, service);
		};
		handler.AddHandle(handle);
	}
}