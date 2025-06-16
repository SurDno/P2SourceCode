using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;
using UnityEngine;

public abstract class ActiveWindowCheck<T> : MonoBehaviour
{
  [SerializeField]
  private HideableView view;

  private void Start()
  {
    if (!((Object) this.view != (Object) null))
      return;
    this.view.Visible = ServiceLocator.GetService<UIService>().Active is T;
  }
}
