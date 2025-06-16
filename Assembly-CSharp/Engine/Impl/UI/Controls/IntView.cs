// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.IntView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
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
