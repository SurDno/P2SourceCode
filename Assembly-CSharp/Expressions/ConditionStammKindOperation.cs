using Engine.Common.Commons;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[condition ? true : false] : StammKind", MenuItem = "condition ? true : false/StammKind")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ConditionStammKindOperation : ConditionOperation<StammKind> { }