using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Values;

[TypeName(TypeName = "[ability value] : bool", MenuItem = "ability value/bool")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectAbilityBoolValue : EffectAbilityValue<bool> { }