using Engine.Common.Components.Parameters;
using Engine.Source.Components;

namespace Engine.Impl.UI.Controls
{
  public class FloatParameterView : EntityViewBase, IChangeParameterListener
  {
    [SerializeField]
    [FormerlySerializedAs("progressView")]
    private ProgressViewBase valueView;
    [SerializeField]
    private ParameterNameEnum parameterName;
    [SerializeField]
    private float defaultValue = 0.0f;
    [SerializeField]
    private bool normalized = true;
    private IParameter<float> parameter;

    protected override void ApplyValue()
    {
      if (parameter != null)
        parameter.RemoveListener(this);
      parameter = Value?.GetComponent<ParametersComponent>()?.GetByName<float>(parameterName);
      if (parameter != null)
        parameter.AddListener(this);
      ApplyParameter();
    }

    private void ApplyParameter()
    {
      if (!((Object) valueView != (Object) null))
        return;
      valueView.Progress = parameter == null ? defaultValue : (normalized ? parameter.Value / parameter.MaxValue : parameter.Value);
    }

    public override void SkipAnimation()
    {
      if (!((Object) valueView != (Object) null))
        return;
      valueView.SkipAnimation();
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter != this.parameter)
        return;
      ApplyParameter();
    }
  }
}
