// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Weather.Element.Clouds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

#nullable disable
namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Clouds : IBlendable<Clouds>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float attenuation;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float brightness;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float coverage;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float opacity;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float saturation;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float scattering;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float sharpness;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float size;

    public float Size
    {
      get => this.size;
      set => this.size = value;
    }

    public float Opacity
    {
      get => this.opacity;
      set => this.opacity = value;
    }

    public float Coverage
    {
      get => this.coverage;
      set => this.coverage = value;
    }

    public float Sharpness
    {
      get => this.sharpness;
      set => this.sharpness = value;
    }

    public float Attenuation
    {
      get => this.attenuation;
      set => this.attenuation = value;
    }

    public float Saturation
    {
      get => this.saturation;
      set => this.saturation = value;
    }

    public float Scattering
    {
      get => this.scattering;
      set => this.scattering = value;
    }

    public float Brightness
    {
      get => this.brightness;
      set => this.brightness = value;
    }

    public void Blend(Clouds a, Clouds b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Size = blendOperation.Blend(a.Size, b.Size);
      this.Opacity = blendOperation.Blend(a.Opacity, b.Opacity);
      this.Coverage = blendOperation.Blend(a.Coverage, b.Coverage);
      this.Sharpness = blendOperation.Blend(a.Sharpness, b.Sharpness);
      this.Attenuation = blendOperation.Blend(a.Attenuation, b.Attenuation);
      this.Saturation = blendOperation.Blend(a.Saturation, b.Saturation);
      this.Scattering = blendOperation.Blend(a.Scattering, b.Scattering);
      this.Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
    }
  }
}
