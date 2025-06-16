using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[value] : bool", MenuItem = "value/bool")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class BoolValue : ConstValue<bool> { }