using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

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
