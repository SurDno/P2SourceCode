using Engine.Source.Commons;
using Inspectors;
using System;

namespace Engine.Source.Settings
{
  public abstract class SettingsInstanceByRequest<T> : InstanceByRequest<T> where T : class, new()
  {
    public SettingsInstanceByRequest() => SettingsViewService.AddSettings((object) this);

    protected virtual void OnInvalidate()
    {
    }

    public event Action OnApply;

    [Inspected]
    public void Apply()
    {
      this.OnInvalidate();
      Action onApply = this.OnApply;
      if (onApply == null)
        return;
      onApply();
    }
  }
}
