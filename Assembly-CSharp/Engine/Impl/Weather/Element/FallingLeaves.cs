using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class FallingLeaves : IBlendable<FallingLeaves>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float deviation;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float minLandingNormalY;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected int poolCapacity;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float radius;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float rate;

    public int PoolCapacity
    {
      get => this.poolCapacity;
      set => this.poolCapacity = value;
    }

    public float Radius
    {
      get => this.radius;
      set => this.radius = value;
    }

    public float MinLandingNormalY
    {
      get => this.minLandingNormalY;
      set => this.minLandingNormalY = value;
    }

    public float Deviation
    {
      get => this.deviation;
      set => this.deviation = value;
    }

    public float Rate
    {
      get => this.rate;
      set => this.rate = value;
    }

    public void Blend(FallingLeaves a, FallingLeaves b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.PoolCapacity = blendOperation.Blend(a.PoolCapacity, b.PoolCapacity);
      this.Radius = blendOperation.Blend(a.Radius, b.Radius);
      this.MinLandingNormalY = blendOperation.Blend(a.MinLandingNormalY, b.MinLandingNormalY);
      this.Deviation = blendOperation.Blend(a.Deviation, b.Deviation);
      this.Rate = blendOperation.Blend(a.Rate, b.Rate);
    }
  }
}
