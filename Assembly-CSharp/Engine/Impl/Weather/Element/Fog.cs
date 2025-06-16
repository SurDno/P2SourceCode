// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Weather.Element.Fog
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
  public class Fog : IBlendable<Fog>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float density;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float height;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float startDistance;

    public float Density
    {
      get => this.density;
      set => this.density = value;
    }

    public float StartDistance
    {
      get => this.startDistance;
      set => this.startDistance = value;
    }

    public float Height
    {
      get => this.height;
      set => this.height = value;
    }

    public void Blend(Fog a, Fog b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Density = blendOperation.Blend(a.Density, b.Density);
      this.StartDistance = blendOperation.Blend(a.StartDistance, b.StartDistance);
      this.Height = blendOperation.Blend(a.Height, b.Height);
    }
  }
}
