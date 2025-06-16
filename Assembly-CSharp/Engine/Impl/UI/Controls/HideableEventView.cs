// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.HideableEventView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class HideableEventView : EventView
  {
    [SerializeField]
    private HideableView hideableView;
    [SerializeField]
    private float length = 1f;

    public void Hide() => this.hideableView.Visible = false;

    public void Show() => this.hideableView.Visible = true;

    public override void Invoke()
    {
      this.Show();
      this.CancelInvoke("Hide");
      this.Invoke("Hide", this.length);
    }

    private void OnDisable()
    {
      this.CancelInvoke("Hide");
      this.Hide();
    }
  }
}
