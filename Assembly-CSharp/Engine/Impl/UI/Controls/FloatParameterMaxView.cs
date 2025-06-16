using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;

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

    private void Update() => this.ApplyParameter();

    protected override void ApplyValue()
    {
      this.parameter = this.Value?.GetComponent<ParametersComponent>()?.GetByName<float>(this.parameterName);
      this.ApplyParameter();
    }

    private void ApplyParameter()
    {
      if (!((Object) this.valueView != (Object) null))
        return;
      this.valueView.Progress = this.parameter == null ? this.defaultValue : this.parameter.MaxValue;
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.valueView != (Object) null))
        return;
      this.valueView.SkipAnimation();
    }
  }
}
