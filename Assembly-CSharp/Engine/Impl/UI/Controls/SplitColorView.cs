// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SplitColorView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class SplitColorView : SingleColorView
  {
    [SerializeField]
    private ColorViewHandle[] views;

    protected override void ApplyValue(bool instant)
    {
      if (this.views == null)
        return;
      for (int index = 0; index < this.views.Length; ++index)
        this.views[index].SetValue(this.GetValue(), instant);
    }
  }
}
