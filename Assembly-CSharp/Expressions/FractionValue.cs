using Engine.Common.Commons;
using Engine.Common.Generator;
using Inspectors;

namespace Expressions;

[TypeName(TypeName = "[value] : Fraction", MenuItem = "value/Fraction")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class FractionValue : ConstValue<FractionEnum> { }