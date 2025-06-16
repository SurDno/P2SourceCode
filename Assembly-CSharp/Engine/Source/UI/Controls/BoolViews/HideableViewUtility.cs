// Decompiled with JetBrains decompiler
// Type: Engine.Source.UI.Controls.BoolViews.HideableViewUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Impl.UI.Controls;
using UnityEngine;

#nullable disable
namespace Engine.Source.UI.Controls.BoolViews
{
  public static class HideableViewUtility
  {
    public static void SetVisible(GameObject gameObject, bool value)
    {
      if ((Object) gameObject == (Object) null)
        return;
      HideableView component = gameObject.GetComponent<HideableView>();
      if ((Object) component != (Object) null)
        component.Visible = value;
      else
        gameObject.SetActive(value);
    }
  }
}
