using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Stars : IBlendable<Stars> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float brightness;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()]
	protected float size;

	public float Size {
		get => size;
		set => size = value;
	}

	public float Brightness {
		get => brightness;
		set => brightness = value;
	}

	public void Blend(Stars a, Stars b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		Size = blendOperation.Blend(a.Size, b.Size);
		Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
	}
}