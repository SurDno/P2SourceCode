using Engine.Common.Commons;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[value] : StammKind", MenuItem = "value/StammKind")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StammKindValue : ConstValue<StammKind> { }