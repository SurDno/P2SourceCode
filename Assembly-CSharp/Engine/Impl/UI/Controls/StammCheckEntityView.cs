using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class StammCheckEntityView : EntityViewBase, IChangeParameterListener
  {
    [SerializeField]
    private HideableView view;
    [SerializeField]
    private ParameterNameEnum parameterName = ParameterNameEnum.StammKind;
    [SerializeField]
    private StammKind[] stamms;
    private IParameter<StammKind> parameter;

    protected override void ApplyValue()
    {
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.parameter = this.Value?.GetComponent<ParametersComponent>()?.GetByName<StammKind>(this.parameterName);
      if (this.parameter != null)
        this.parameter.AddListener((IChangeParameterListener) this);
      this.ApplyParameter();
    }

    private void ApplyParameter()
    {
      if (!((Object) this.view != (Object) null))
        return;
      if (this.parameter == null)
      {
        this.view.Visible = false;
      }
      else
      {
        bool flag = false;
        for (int index = 0; index < this.stamms.Length; ++index)
        {
          if (this.stamms[index] == this.parameter.Value)
          {
            flag = true;
            break;
          }
        }
        this.view.Visible = flag;
      }
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
