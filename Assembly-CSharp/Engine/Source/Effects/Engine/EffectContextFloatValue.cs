using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine;

[TypeName(TypeName = "[parameter] : float", MenuItem = "parameter/float")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectContextFloatValue : EffectContextValue<float> { }