using Engine.Common;
using Inspectors;

namespace Engine.Impl.UI.Controls
{
  public abstract class EntityViewBase : EntityView
  {
    [Inspected]
    private IEntity value;

    public override IEntity Value
    {
      get => this.value;
      set
      {
        if (this.value == value)
          return;
        this.value = value;
        this.ApplyValue();
      }
    }

    protected abstract void ApplyValue();
  }
}
