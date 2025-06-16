// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.AnchorGameActionView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class AnchorGameActionView : GameActionViewBase
  {
    [SerializeField]
    private GameActionView prefab;
    private GameActionView instance;

    private void Awake()
    {
      if (!((Object) this.prefab != (Object) null) || this.GetValue() == 0)
        return;
      this.instance = Object.Instantiate<GameActionView>(this.prefab, this.transform, false);
      this.instance.SetValue(this.GetValue(), true);
    }

    protected override void ApplyValue(bool instant)
    {
      if (this.GetValue() != 0)
      {
        this.instance?.SetValue(this.GetValue(), instant);
        this.instance?.gameObject.SetActive(true);
      }
      else
        this.instance?.gameObject.SetActive(false);
    }
  }
}
