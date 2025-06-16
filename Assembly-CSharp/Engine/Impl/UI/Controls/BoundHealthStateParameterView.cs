// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.BoundHealthStateParameterView
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
  public class BoundHealthStateParameterView : EntityViewBase, IChangeParameterListener
  {
    [SerializeField]
    private IntView view;
    [SerializeField]
    private ParameterNameEnum parameterName;
    [SerializeField]
    private int defaultValue = 0;
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
      this.view.IntValue = this.parameter != null ? (int) this.parameter.Value : this.defaultValue;
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
