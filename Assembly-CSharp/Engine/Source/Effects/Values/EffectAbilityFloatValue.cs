using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Values;

[TypeName(TypeName = "[ability value] : float", MenuItem = "ability value/float")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectAbilityFloatValue : EffectAbilityValue<float> { }