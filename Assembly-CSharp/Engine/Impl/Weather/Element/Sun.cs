using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Sun : IBlendable<Sun> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float brightness;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float contrast;

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

	public float Contrast {
		get => contrast;
		set => contrast = value;
	}

	public void Blend(Sun a, Sun b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		Size = blendOperation.Blend(a.Size, b.Size);
		Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
		Contrast = blendOperation.Blend(a.Contrast, b.Contrast);
	}
}