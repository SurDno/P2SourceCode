using Engine.Common.Services;
using Engine.Impl.UI.Controls;
using Engine.Source.Services;

public class UndiscoveredMindmapCheck : MonoBehaviour
{
  [SerializeField]
  private HideableView view;
  private MMService service;

  private void OnDisable()
  {
    if (service == null)
      return;
    service.ChangeUndiscoveredEvent -= UpdateView;
    service = null;
  }

  private void OnEnable()
  {
    if ((UnityEngine.Object) view == (UnityEngine.Object) null)
      return;
    service = ServiceLocator.GetService<MMService>();
    if (service == null)
      return;
    UpdateView();
    view.SkipAnimation();
    service.ChangeUndiscoveredEvent += UpdateView;
  }

  private void UpdateView() => view.Visible = service.HasUndiscovered();
}
