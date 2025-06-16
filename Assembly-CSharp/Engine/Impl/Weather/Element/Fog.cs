using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Fog : IBlendable<Fog> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float density;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float height;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()]
	protected float startDistance;

	public float Density {
		get => density;
		set => density = value;
	}

	public float StartDistance {
		get => startDistance;
		set => startDistance = value;
	}

	public float Height {
		get => height;
		set => height = value;
	}

	public void Blend(Fog a, Fog b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		Density = blendOperation.Blend(a.Density, b.Density);
		StartDistance = blendOperation.Blend(a.StartDistance, b.StartDistance);
		Height = blendOperation.Blend(a.Height, b.Height);
	}
}