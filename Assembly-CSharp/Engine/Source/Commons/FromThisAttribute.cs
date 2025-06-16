using System;
using System.Reflection;
using Cofe.Meta;
using Engine.Common;

namespace Engine.Source.Commons;

[AttributeUsage(AttributeTargets.Field)]
public class FromThisAttribute : MemberAttribute {
	public static readonly Guid Id = Guid.NewGuid();
	public static readonly Guid ClearId = Guid.NewGuid();

	public override void ComputeMember(Container container, MemberInfo member) {
		var handler = container.GetHandler(Id);
		var field = member as FieldInfo;
		var type = field.FieldType;
		handler.AddHandle((target, data) => {
			if (!(target is IComponent component2))
				return;
			var component3 = component2.Owner.GetComponent(type);
			field.SetValue(target, component3);
		});
		container.GetHandler(ClearId).AddHandle((target, data) => field.SetValue(target, null));
	}
}