using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Controls;

public abstract class ActiveWindowCheck<T> : MonoBehaviour
{
  [SerializeField]
  private HideableView view;

  private void Start()
  {
    if (!((Object) view != (Object) null))
      return;
    view.Visible = ServiceLocator.GetService<UIService>().Active is T;
  }
}
