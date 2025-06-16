// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Weather.Element.Day
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Drawing.Gradient;
using Engine.Source.Blenders;

#nullable disable
namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Day : IBlendable<Day>
  {
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    protected ColorGradient ambientColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float ambientMultiplier;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float brightness;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    protected ColorGradient cloudColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float contrast;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float directionality;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    protected ColorGradient fogColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float fogginess;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    protected ColorGradient lightColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float lightIntensity;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float mieMultiplier;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    protected ColorGradient rayColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float rayleighMultiplier;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float reflectionMultiplier;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float shadowStrength;
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    protected ColorGradient skyColor = ProxyFactory.Create<ColorGradient>();
    [DataReadProxy(MemberEnum.OnlyCopy)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.OnlyCopy)]
    protected ColorGradient sunColor = ProxyFactory.Create<ColorGradient>();

    public ColorGradient SunColor => this.sunColor;

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

    public void Blend(Day a, Day b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.SunColor.Blend(a.SunColor, b.SunColor, opp);
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
