using Engine.Common.Blenders;
using Engine.Common.Generator;
using Engine.Source.Blenders;

namespace Engine.Impl.Weather.Element
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Stars : IBlendable<Stars>
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    protected float brightness;
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

    public void Blend(Stars a, Stars b, IPureBlendOperation opp)
    {
      IBlendOperation blendOperation = (IBlendOperation) opp;
      this.Size = blendOperation.Blend(a.Size, b.Size);
      this.Brightness = blendOperation.Blend(a.Brightness, b.Brightness);
    }
  }
}
