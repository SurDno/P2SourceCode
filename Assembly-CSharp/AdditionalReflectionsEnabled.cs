using Engine.Impl.UI.Controls;
using Engine.Source.Commons;
using Engine.Source.Settings;

public class AdditionalReflectionsEnabled : MonoBehaviour
{
  [SerializeField]
  private HideableView view;

  private void UpdateValue()
  {
    if (!((UnityEngine.Object) view != (UnityEngine.Object) null))
      return;
    view.Visible = InstanceByRequest<GraphicsGameSettings>.Instance.AdditionalReflections.Value;
  }

  private void OnEnable()
  {
    UpdateValue();
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply += UpdateValue;
  }

  private void OnDisable()
  {
    InstanceByRequest<GraphicsGameSettings>.Instance.OnApply -= UpdateValue;
  }
}
