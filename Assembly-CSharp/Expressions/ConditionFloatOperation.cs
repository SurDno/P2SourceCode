using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[condition ? true : false] : float", MenuItem = "condition ? true : false/float")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ConditionFloatOperation : ConditionOperation<float> { }