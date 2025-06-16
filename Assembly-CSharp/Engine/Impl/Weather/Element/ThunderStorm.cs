using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class ThunderStorm : IBlendable<ThunderStorm> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float distanceFrom;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float distanceTo;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()]
	protected float frequency;

	public float Frequency {
		get => frequency;
		set => frequency = value;
	}

	public float DistanceTo {
		get => distanceTo;
		set => distanceTo = value;
	}

	public float DistanceFrom {
		get => distanceFrom;
		set => distanceFrom = value;
	}

	public void Blend(ThunderStorm a, ThunderStorm b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		Frequency = blendOperation.Blend(a.Frequency, b.Frequency);
		DistanceFrom = blendOperation.Blend(a.DistanceFrom, b.DistanceFrom);
		DistanceTo = blendOperation.Blend(a.DistanceTo, b.DistanceTo);
	}
}