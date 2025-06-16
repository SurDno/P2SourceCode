using Cofe.Proxies;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Drawing.Gradient;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class Day : IBlendable<Day> {
	[DataReadProxy(MemberEnum.OnlyCopy)] [DataWriteProxy] [CopyableProxy(MemberEnum.OnlyCopy)]
	protected ColorGradient ambientColor = ProxyFactory.Create<ColorGradient>();

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float ambientMultiplier;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float brightness;

	[DataReadProxy(MemberEnum.OnlyCopy)] [DataWriteProxy] [CopyableProxy(MemberEnum.OnlyCopy)]
	protected ColorGradient cloudColor = ProxyFactory.Create<ColorGradient>();

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float contrast;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float directionality;

	[DataReadProxy(MemberEnum.OnlyCopy)] [DataWriteProxy] [CopyableProxy(MemberEnum.OnlyCopy)]
	protected ColorGradient fogColor = ProxyFactory.Create<ColorGradient>();

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float fogginess;

	[DataReadProxy(MemberEnum.OnlyCopy)] [DataWriteProxy] [CopyableProxy(MemberEnum.OnlyCopy)]
	protected ColorGradient lightColor = ProxyFactory.Create<ColorGradient>();

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float lightIntensity;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float mieMultiplier;

	[DataReadProxy(MemberEnum.OnlyCopy)] [DataWriteProxy] [CopyableProxy(MemberEnum.OnlyCopy)]
	protected ColorGradient rayColor = ProxyFactory.Create<ColorGradient>();

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float rayleighMultiplier;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float reflectionMultiplier;

	[DataReadProxy] [DataWriteProxy] [CopyableProxy]
	protected float shadowStrength;

	[DataReadProxy(MemberEnum.OnlyCopy)] [DataWriteProxy] [CopyableProxy(MemberEnum.OnlyCopy)]
	protected ColorGradient skyColor = ProxyFactory.Create<ColorGradient>();

	[DataReadProxy(MemberEnum.OnlyCopy)] [DataWriteProxy()] [CopyableProxy(MemberEnum.OnlyCopy)]
	protected ColorGradient sunColor = ProxyFactory.Create<ColorGradient>();

	public ColorGradient SunColor => sunColor;

	public ColorGradient LightColor => lightColor;

	public ColorGradient RayColor => rayColor;

	public ColorGradient SkyColor => skyColor;

	public ColorGradient CloudColor => cloudColor;

	public ColorGradient FogColor => fogColor;

	public ColorGradient AmbientColor => ambientColor;

	public float RayleighMultiplier {
		get => rayleighMultiplier;
		set => rayleighMultiplier = value;
	}

	public float MieMultiplier {
		get => mieMultiplier;
		set => mieMultiplier = value;
	}

	public float Brightness {
		get => brightness;
		set => brightness = value;
	}

	public float Contrast {
		get => contrast;
		set => contrast = value;
	}

	public float Directionality {
		get => directionality;
		set => directionality = value;
	}

	public float Fogginess {
		get => fogginess;
		set => fogginess = value;
	}

	public float LightIntensity {
		get => lightIntensity;
		set => lightIntensity = value;
	}

	public float ShadowStrength {
		get => shadowStrength;
		set => shadowStrength = value;
	}

	public float AmbientMultiplier {
		get => ambientMultiplier;
		set => ambientMultiplier = value;
	}

	public float ReflectionMultiplier {
		get => reflectionMultiplier;
		set => reflectionMultiplier = value;
	}

	public void Blend(Day a, Day b, IPureBlendOperation opp) {
		var blendOperation = (IBlendOperation)opp;
		SunColor.Blend(a.SunColor, b.SunColor, opp);
		LightColor.Blend(a.LightColor, b.LightColor, opp);
		RayColor.Blend(a.RayColor, b.RayColor, opp);
		SkyColor.Blend(a.SkyColor, b.SkyColor, opp);
		CloudColor.Blend(a.CloudColor, b.CloudColor, opp);
		FogColor.Blend(a.FogColor, b.FogColor, opp);
		AmbientColor.Blend(a.AmbientColor, b.AmbientColor, opp);
		RayleighMultiplier = blendOperation.Blend(a.RayleighMultiplier, b.RayleighMultiplier);
		MieMultiplier = blendOperation.Blend(a.MieMultiplier, b.MieMultiplier);
		Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
		Contrast = blendOperation.Blend(a.Contrast, b.Contrast);
		Directionality = blendOperation.Blend(a.Directionality, b.Directionality);
		Fogginess = blendOperation.Blend(a.Fogginess, b.Fogginess);
		LightIntensity = blendOperation.Blend(a.LightIntensity, b.LightIntensity);
		ShadowStrength = blendOperation.Blend(a.ShadowStrength, b.ShadowStrength);
		AmbientMultiplier = blendOperation.Blend(a.AmbientMultiplier, b.AmbientMultiplier);
		ReflectionMultiplier = blendOperation.Blend(a.ReflectionMultiplier, b.ReflectionMultiplier);
	}
}