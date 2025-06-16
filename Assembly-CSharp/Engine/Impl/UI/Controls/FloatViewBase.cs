// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.FloatViewBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class FloatViewBase : FloatView
  {
    [SerializeField]
    private float floatValue = 0.0f;

    public override float FloatValue
    {
      get => this.floatValue;
      set
      {
        if ((double) this.floatValue == (double) value)
          return;
        this.floatValue = value;
        this.ApplyFloatValue();
      }
    }

    protected virtual void OnValidate() => this.ApplyFloatValue();

    protected abstract void ApplyFloatValue();
  }
}
