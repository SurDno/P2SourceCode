using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class FallingLeaves : IBlendable<FallingLeaves> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float deviation;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float minLandingNormalY;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected int poolCapacity;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float radius;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()]
	protected float rate;

	public int PoolCapacity {
		get => poolCapacity;
		set => poolCapacity = value;
	}

	public float Radius {
		get => radius;
		set => radius = value;
	}

	public float MinLandingNormalY {
		get => minLandingNormalY;
		set => minLandingNormalY = value;
	}

	public float Deviation {
		get => deviation;
		set => deviation = value;
	}

	public float Rate {
		get => rate;
		set => rate = value;
	}

	public void Blend(FallingLeaves a, FallingLeaves b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		PoolCapacity = blendOperation.Blend(a.PoolCapacity, b.PoolCapacity);
		Radius = blendOperation.Blend(a.Radius, b.Radius);
		MinLandingNormalY = blendOperation.Blend(a.MinLandingNormalY, b.MinLandingNormalY);
		Deviation = blendOperation.Blend(a.Deviation, b.Deviation);
		Rate = blendOperation.Blend(a.Rate, b.Rate);
	}
}