using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ThunderStorm : IBlendable<ThunderStorm>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float distanceFrom;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float distanceTo;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float frequency;

    public float Frequency
    {
      get => this.frequency;
      set => this.frequency = value;
    }

    public float DistanceTo
    {
      get => this.distanceTo;
      set => this.distanceTo = value;
    }

    public float DistanceFrom
    {
      get => this.distanceFrom;
      set => this.distanceFrom = value;
    }

    public void Blend(ThunderStorm a, ThunderStorm b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Frequency = blendOperation.Blend(a.Frequency, b.Frequency);
      this.DistanceFrom = blendOperation.Blend(a.DistanceFrom, b.DistanceFrom);
      this.DistanceTo = blendOperation.Blend(a.DistanceTo, b.DistanceTo);
    }
  }
}
