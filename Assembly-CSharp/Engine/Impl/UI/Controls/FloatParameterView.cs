using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.Serialization;

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
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.parameter = this.Value?.GetComponent<ParametersComponent>()?.GetByName<float>(this.parameterName);
      if (this.parameter != null)
        this.parameter.AddListener((IChangeParameterListener) this);
      this.ApplyParameter();
    }

    private void ApplyParameter()
    {
      if (!((Object) this.valueView != (Object) null))
        return;
      this.valueView.Progress = this.parameter == null ? this.defaultValue : (this.normalized ? this.parameter.Value / this.parameter.MaxValue : this.parameter.Value);
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.valueView != (Object) null))
        return;
      this.valueView.SkipAnimation();
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter != this.parameter)
        return;
      this.ApplyParameter();
    }
  }
}
