// Decompiled with JetBrains decompiler
// Type: Engine.Impl.Weather.Element.Location
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
  public class Location : IBlendable<Location>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float latitude;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float longitude;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float utc;

    public float Latitude
    {
      get => this.latitude;
      set => this.latitude = value;
    }

    public float Longitude
    {
      get => this.longitude;
      set => this.longitude = value;
    }

    public float Utc
    {
      get => this.utc;
      set => this.utc = value;
    }

    public void Blend(Location a, Location b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Latitude = blendOperation.Blend(a.Latitude, b.Latitude);
      this.Longitude = blendOperation.Blend(a.Longitude, b.Longitude);
      this.Utc = blendOperation.Blend(a.Utc, b.Utc);
    }
  }
}
