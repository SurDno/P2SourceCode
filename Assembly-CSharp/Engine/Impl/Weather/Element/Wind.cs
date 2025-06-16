using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Wind : IBlendable<Wind>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float degrees;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float speed;

    public float Degrees
    {
      get => this.degrees;
      set => this.degrees = value;
    }

    public float Speed
    {
      get => this.speed;
      set => this.speed = value;
    }

    public void Blend(Wind a, Wind b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Degrees = blendOperation.Blend(a.Degrees, b.Degrees);
      this.Speed = blendOperation.Blend(a.Speed, b.Speed);
    }
  }
}
