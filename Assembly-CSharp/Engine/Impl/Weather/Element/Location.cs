using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Location : IBlendable<Location>
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    protected float latitude;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    protected float longitude;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    protected float utc;

    public float Latitude
    {
      get => latitude;
      set => latitude = value;
    }

    public float Longitude
    {
      get => longitude;
      set => longitude = value;
    }

    public float Utc
    {
      get => utc;
      set => utc = value;
    }

    public void Blend(Location a, Location b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      Latitude = blendOperation.Blend(a.Latitude, b.Latitude);
      Longitude = blendOperation.Blend(a.Longitude, b.Longitude);
      Utc = blendOperation.Blend(a.Utc, b.Utc);
    }
  }
}
