// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Weather.Element.Moon
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
  public class Moon : IBlendable<Moon>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float brightness;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float contrast;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float size;

    public float Size
    {
      get => this.size;
      set => this.size = value;
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

    public void Blend(Moon a, Moon b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Size = blendOperation.Blend(a.Size, b.Size);
      this.Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
      this.Contrast = blendOperation.Blend(a.Contrast, b.Contrast);
    }
  }
}
