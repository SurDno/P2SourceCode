// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.GameActionViewBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Services.Inputs;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public abstract class GameActionViewBase : GameActionView
  {
    [SerializeField]
    private GameActionType value;

    public override GameActionType GetValue() => this.value;

    public override void SetValue(GameActionType value, bool instant)
    {
      if (this.value == value)
        return;
      this.value = value;
      this.ApplyValue(instant);
    }

    protected abstract void ApplyValue(bool instant);
  }
}
