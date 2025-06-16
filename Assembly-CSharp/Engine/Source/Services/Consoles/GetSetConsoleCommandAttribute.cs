using System;
using System.Reflection;
using Cofe.Meta;
using Cofe.Utility;
using Engine.Assets.Internal;
using Engine.Source.Services.Consoles.Binds;

namespace Engine.Source.Services.Consoles;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class GetSetConsoleCommandAttribute : InitialiseAttribute {
	private string name;

	public GetSetConsoleCommandAttribute(string name) {
		this.name = name;
	}

	public override void ComputeMember(Container container, MemberInfo member) {
		if (name.IsNullOrEmpty())
			return;
		var valueType = member.GetValueType();
		var type = member.DeclaringType;
		container.GetHandler(Id).AddHandle((target, data) => {
			GetConsoleCommand.AddBind(type, valueType, name, true, target2 => member.GetValue(target2));
			SetConsoleCommand.AddBind(type, valueType, name, true, (target2, value) => member.SetValue(target2, value));
		});
	}
}