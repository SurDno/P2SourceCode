using Engine.Common.Components.Detectors;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Effects.Engine;

[TypeName(TypeName = "[parameter] : DetectType", MenuItem = "parameter/DetectType")]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class EffectContextDetectTypeValue : EffectContextValue<DetectType> { }