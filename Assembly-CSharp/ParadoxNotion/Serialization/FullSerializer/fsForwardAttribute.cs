using System;

namespace ParadoxNotion.Serialization.FullSerializer;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
public sealed class fsForwardAttribute : Attribute {
	public string MemberName;

	public fsForwardAttribute(string memberName) {
		MemberName = memberName;
	}
}