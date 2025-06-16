using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Effects.Values;

[Factory(typeof(FloatAbilityValue))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class FloatAbilityValue : AbilityValue<float> { }