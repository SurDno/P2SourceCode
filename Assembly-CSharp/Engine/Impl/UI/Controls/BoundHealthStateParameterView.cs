using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class BoundHealthStateParameterView : EntityViewBase, IChangeParameterListener
  {
    [SerializeField]
    private IntView view;
    [SerializeField]
    private ParameterNameEnum parameterName;
    [SerializeField]
    private int defaultValue;
    private IParameter<BoundHealthStateEnum> parameter;

    protected override void ApplyValue()
    {
      if (parameter != null)
        parameter.RemoveListener(this);
      parameter = Value?.GetComponent<ParametersComponent>()?.GetByName<BoundHealthStateEnum>(parameterName);
      if (parameter != null)
        parameter.AddListener(this);
      ApplyParameter();
    }

    private void ApplyParameter()
    {
      if (!(view != null))
        return;
      view.IntValue = parameter != null ? (int) parameter.Value : defaultValue;
    }

    public override void SkipAnimation()
    {
      if (!(view != null))
        return;
      view.SkipAnimation();
    }

    public void OnParameterChanged(IParameter parameter)
    {
      if (parameter != this.parameter)
        return;
      ApplyParameter();
    }
  }
}
