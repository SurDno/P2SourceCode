using Cofe.Proxies;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Drawing.Gradient;
using Engine.Source.Blenders;
using Inspectors;

namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Night : IBlendable<Night>
  {
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ColorGradient ambientColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float ambientMultiplier;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float brightness;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ColorGradient cloudColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float contrast;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float directionality;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ColorGradient fogColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float fogginess;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ColorGradient lightColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float lightIntensity;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float mieMultiplier;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ColorGradient moonColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ColorGradient rayColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float rayleighMultiplier;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float reflectionMultiplier;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    protected float shadowStrength;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    [Inspected]
    protected ColorGradient skyColor = ProxyFactory.Create<ColorGradient>();

    public ColorGradient MoonColor => this.moonColor;

    public ColorGradient LightColor => this.lightColor;

    public ColorGradient RayColor => this.rayColor;

    public ColorGradient SkyColor => this.skyColor;

    public ColorGradient CloudColor => this.cloudColor;

    public ColorGradient FogColor => this.fogColor;

    public ColorGradient AmbientColor => this.ambientColor;

    public float RayleighMultiplier
    {
      get => this.rayleighMultiplier;
      set => this.rayleighMultiplier = value;
    }

    public float MieMultiplier
    {
      get => this.mieMultiplier;
      set => this.mieMultiplier = value;
    }

    public float Brightness
    {
      get => this.brightness;
      set => this.brightness = value;
    }

    public float Contrast
    {
      get => this.contrast;
      set => this.contrast = value;
    }

    public float Directionality
    {
      get => this.directionality;
      set => this.directionality = value;
    }

    public float Fogginess
    {
      get => this.fogginess;
      set => this.fogginess = value;
    }

    public float LightIntensity
    {
      get => this.lightIntensity;
      set => this.lightIntensity = value;
    }

    public float ShadowStrength
    {
      get => this.shadowStrength;
      set => this.shadowStrength = value;
    }

    public float AmbientMultiplier
    {
      get => this.ambientMultiplier;
      set => this.ambientMultiplier = value;
    }

    public float ReflectionMultiplier
    {
      get => this.reflectionMultiplier;
      set => this.reflectionMultiplier = value;
    }

    public void Blend(Night a, Night b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.MoonColor.Blend(a.MoonColor, b.MoonColor, opp);
      this.LightColor.Blend(a.LightColor, b.LightColor, opp);
      this.RayColor.Blend(a.RayColor, b.RayColor, opp);
      this.SkyColor.Blend(a.SkyColor, b.SkyColor, opp);
      this.CloudColor.Blend(a.CloudColor, b.CloudColor, opp);
      this.FogColor.Blend(a.FogColor, b.FogColor, opp);
      this.AmbientColor.Blend(a.AmbientColor, b.AmbientColor, opp);
      this.RayleighMultiplier = blendOperation.Blend(a.RayleighMultiplier, b.RayleighMultiplier);
      this.MieMultiplier = blendOperation.Blend(a.MieMultiplier, b.MieMultiplier);
      this.Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
      this.Contrast = blendOperation.Blend(a.Contrast, b.Contrast);
      this.Directionality = blendOperation.Blend(a.Directionality, b.Directionality);
      this.Fogginess = blendOperation.Blend(a.Fogginess, b.Fogginess);
      this.LightIntensity = blendOperation.Blend(a.LightIntensity, b.LightIntensity);
      this.ShadowStrength = blendOperation.Blend(a.ShadowStrength, b.ShadowStrength);
      this.AmbientMultiplier = blendOperation.Blend(a.AmbientMultiplier, b.AmbientMultiplier);
      this.ReflectionMultiplier = blendOperation.Blend(a.ReflectionMultiplier, b.ReflectionMultiplier);
    }
  }
}
