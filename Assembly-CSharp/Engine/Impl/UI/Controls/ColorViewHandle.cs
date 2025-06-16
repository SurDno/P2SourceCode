// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ColorViewHandle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  [Serializable]
  public struct ColorViewHandle
  {
    [SerializeField]
    [AssignableObject(typeof (IValueView<Color>))]
    private UnityEngine.Object view;
    [SerializeField]
    private int id;

    public void SetValue(Color value, bool instant)
    {
      if (!(this.view is IValueView<Color> view))
        return;
      view.SetValue(this.id, value, instant);
    }

    public Color GetValue()
    {
      return this.view is IValueView<Color> view ? view.GetValue(this.id) : new Color();
    }
  }
}
