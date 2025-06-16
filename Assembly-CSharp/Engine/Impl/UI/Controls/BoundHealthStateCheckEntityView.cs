using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;

namespace Engine.Impl.UI.Controls
{
  public class BoundHealthStateCheckEntityView : EntityViewBase, IChangeParameterListener
  {
    [SerializeField]
    private HideableView view;
    [SerializeField]
    private ParameterNameEnum parameterName = ParameterNameEnum.BoundHealthState;
    [SerializeField]
    private BoundHealthStateEnum[] states;
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
      if (!((Object) view != (Object) null))
        return;
      if (parameter == null)
      {
        view.Visible = false;
      }
      else
      {
        bool flag = false;
        for (int index = 0; index < states.Length; ++index)
        {
          if (states[index] == parameter.Value)
          {
            flag = true;
            break;
          }
        }
        view.Visible = flag;
      }
    }

    public override void SkipAnimation()
    {
      if (!((Object) view != (Object) null))
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
