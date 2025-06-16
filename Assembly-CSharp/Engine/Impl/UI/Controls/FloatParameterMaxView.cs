using Engine.Common.Components.Parameters;
using Engine.Source.Components;

namespace Engine.Impl.UI.Controls
{
  public class FloatParameterMaxView : EntityViewBase
  {
    [SerializeField]
    private ProgressViewBase valueView;
    [SerializeField]
    private ParameterNameEnum parameterName;
    [SerializeField]
    private float defaultValue = 1f;
    private IParameter<float> parameter;

    private void Update() => ApplyParameter();

    protected override void ApplyValue()
    {
      parameter = Value?.GetComponent<ParametersComponent>()?.GetByName<float>(parameterName);
      ApplyParameter();
    }

    private void ApplyParameter()
    {
      if (!((Object) valueView != (Object) null))
        return;
      valueView.Progress = parameter == null ? defaultValue : parameter.MaxValue;
    }

    public override void SkipAnimation()
    {
      if (!((Object) valueView != (Object) null))
        return;
      valueView.SkipAnimation();
    }
  }
}
