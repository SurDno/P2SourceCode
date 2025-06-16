using System;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public abstract class IntView : FloatView
  {
    [SerializeField]
    private int intValue = 0;

    public int IntValue
    {
      get => this.intValue;
      set
      {
        if (this.intValue == value)
          return;
        this.intValue = value;
        this.ApplyIntValue();
      }
    }

    public override float FloatValue
    {
      get => (float) this.IntValue;
      set => this.IntValue = Convert.ToInt32(value);
    }

    protected virtual void OnValidate() => this.ApplyIntValue();

    protected abstract void ApplyIntValue();
  }
}
