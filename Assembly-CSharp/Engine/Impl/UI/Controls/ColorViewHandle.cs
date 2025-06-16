using System;
using UnityEngine;

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
