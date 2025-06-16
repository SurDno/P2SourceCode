using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;

namespace Engine.Source.Components;

[Factory]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class TestComponent : EngineComponent { }