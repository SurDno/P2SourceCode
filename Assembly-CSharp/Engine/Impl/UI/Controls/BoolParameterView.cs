using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Impl.UI.Controls
{
  public class BoolParameterView : EntityViewBase, IChangeParameterListener
  {
    [SerializeField]
    [FormerlySerializedAs("progressView")]
    private HideableView view;
    [SerializeField]
    private ParameterNameEnum parameterName;
    [SerializeField]
    private bool defaultValue = false;
    private IParameter<bool> parameter;

    protected override void ApplyValue()
    {
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.parameter = this.Value?.GetComponent<ParametersComponent>()?.GetByName<bool>(this.parameterName);
      if (this.parameter != null)
        this.parameter.AddListener((IChangeParameterListener) this);
      this.ApplyParameter();
    }

    private void ApplyParameter()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.Visible = this.parameter != null ? this.parameter.Value : this.defaultValue;
    }

    public override void SkipAnimation()
    {
      if (!((Object) this.view != (Object) null))
        return;
      this.view.SkipAnimation();
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter != this.parameter)
        return;
      this.ApplyParameter();
    }
  }
}
