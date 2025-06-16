// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.VariableChangedEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Variable Change")]
  [Category("Events/Other")]
  [Description("Fires Out when the target variable change. (Not whenever it is set)")]
  public class VariableChangedEvent : EventNode
  {
    [BlackboardOnly]
    public BBParameter<object> targetVariable;
    private FlowOutput outFlow;

    public override string name
    {
      get => string.Format("{0} [{1}]", (object) base.name, (object) this.targetVariable);
    }

    public override void OnGraphStarted()
    {
      if (this.targetVariable.varRef == null)
        return;
      this.targetVariable.varRef.onValueChanged += new Action<string, object>(this.OnChanged);
    }

    public override void OnGraphStoped()
    {
      if (this.targetVariable.varRef == null)
        return;
      this.targetVariable.varRef.onValueChanged -= new Action<string, object>(this.OnChanged);
    }

    protected override void RegisterPorts() => this.outFlow = this.AddFlowOutput("Out");

    private void OnChanged(string name, object value) => this.outFlow.Call();
  }
}
