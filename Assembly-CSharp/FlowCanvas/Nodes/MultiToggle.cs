// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.MultiToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Toggle (Multi)")]
  [Description("Whenever any input is called the current output is called as well. Calling '+' or '-' also changes the current output")]
  [Category("Flow Controllers/Togglers")]
  [ContextDefinedOutputs(new System.Type[] {typeof (int)})]
  public class MultiToggle : FlowControlNode, IMultiPortNode
  {
    [SerializeField]
    private int _portCount = 4;
    private int current;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    public override string name
    {
      get => base.name + " " + string.Format("[{0}]", (object) this.current.ToString());
    }

    public override void OnGraphStarted() => this.current = 0;

    public override void OnGraphStoped() => this.current = 0;

    protected override void RegisterPorts()
    {
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < this.portCount; ++index)
        outs.Add(this.AddFlowOutput(index.ToString()));
      this.AddFlowInput("In", (FlowHandler) (() => outs[this.current].Call()));
      this.AddFlowInput("+", (FlowHandler) (() =>
      {
        this.current = (int) Mathf.Repeat((float) (this.current + 1), (float) this.portCount);
        outs[this.current].Call();
      }));
      this.AddFlowInput("-", (FlowHandler) (() =>
      {
        this.current = (int) Mathf.Repeat((float) (this.current - 1), (float) this.portCount);
        outs[this.current].Call();
      }));
      this.AddValueOutput<int>("Current", (ValueHandler<int>) (() => this.current));
    }
  }
}
