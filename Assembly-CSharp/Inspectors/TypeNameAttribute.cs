using System;
using Cofe.Meta;
using Cofe.Serializations.Data;

namespace Inspectors;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class TypeNameAttribute : TypeAttribute {
	public string TypeName { get; set; }

	public string MenuItem { get; set; }

	public override void PrepareType(Type type) {
		TypeResolver.AddType(type);
		TypeNameService.RegisterTypeName(type, TypeName, MenuItem);
	}
}