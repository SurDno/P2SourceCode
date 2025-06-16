using Inspectors;
using System;

namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (QuestCompassService)})]
  public class QuestCompassService
  {
    private bool enabled;
    public Action<bool> OnEnableChanged;

    [Inspected(Mutable = true)]
    public bool IsEnabled
    {
      get => this.enabled;
      set
      {
        if (this.enabled == value)
          return;
        this.enabled = value;
        Action<bool> onEnableChanged = this.OnEnableChanged;
        if (onEnableChanged == null)
          return;
        onEnableChanged(value);
      }
    }
  }
}
