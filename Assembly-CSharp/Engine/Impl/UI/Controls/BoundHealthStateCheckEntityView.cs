// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.BoundHealthStateCheckEntityView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
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
      if (this.parameter != null)
        this.parameter.RemoveListener((IChangeParameterListener) this);
      this.parameter = this.Value?.GetComponent<ParametersComponent>()?.GetByName<BoundHealthStateEnum>(this.parameterName);
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
        for (int index = 0; index < this.states.Length; ++index)
        {
          if (this.states[index] == this.parameter.Value)
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
