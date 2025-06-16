using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Clouds : IBlendable<Clouds> {
	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float attenuation;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float brightness;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float coverage;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float opacity;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float saturation;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float scattering;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float sharpness;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy()]
	protected float size;

	public float Size {
		get => size;
		set => size = value;
	}

	public float Opacity {
		get => opacity;
		set => opacity = value;
	}

	public float Coverage {
		get => coverage;
		set => coverage = value;
	}

	public float Sharpness {
		get => sharpness;
		set => sharpness = value;
	}

	public float Attenuation {
		get => attenuation;
		set => attenuation = value;
	}

	public float Saturation {
		get => saturation;
		set => saturation = value;
	}

	public float Scattering {
		get => scattering;
		set => scattering = value;
	}

	public float Brightness {
		get => brightness;
		set => brightness = value;
	}

	public void Blend(Clouds a, Clouds b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		Size = blendOperation.Blend(a.Size, b.Size);
		Opacity = blendOperation.Blend(a.Opacity, b.Opacity);
		Coverage = blendOperation.Blend(a.Coverage, b.Coverage);
		Sharpness = blendOperation.Blend(a.Sharpness, b.Sharpness);
		Attenuation = blendOperation.Blend(a.Attenuation, b.Attenuation);
		Saturation = blendOperation.Blend(a.Saturation, b.Saturation);
		Scattering = blendOperation.Blend(a.Scattering, b.Scattering);
		Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
	}
}