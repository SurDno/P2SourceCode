using Engine.Impl.UI.Controls;
using UnityEngine;

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
