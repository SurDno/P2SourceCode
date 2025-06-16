using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine;

[TypeName(TypeName = "[parameter] : bool", MenuItem = "parameter/bool")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectContextBoolValue : EffectContextValue<bool> { }