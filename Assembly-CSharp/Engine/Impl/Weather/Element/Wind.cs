using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Wind : IBlendable<Wind> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float degrees;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()]
	protected float speed;

	public float Degrees {
		get => degrees;
		set => degrees = value;
	}

	public float Speed {
		get => speed;
		set => speed = value;
	}

	public void Blend(Wind a, Wind b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		Degrees = blendOperation.Blend(a.Degrees, b.Degrees);
		Speed = blendOperation.Blend(a.Speed, b.Speed);
	}
}