using System;

namespace Engine.Common.Generator;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class StateLoadProxyAttribute : Attribute {
	public string Name { get; set; }

	public MemberEnum Detail { get; set; }

	public StateLoadProxyAttribute(MemberEnum detail = MemberEnum.None) {
		Detail = detail;
	}
}