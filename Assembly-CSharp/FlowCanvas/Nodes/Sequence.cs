// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.Sequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using ParadoxNotion.Design;
using ParadoxNotion.FlowCanvas.Module;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Flip Flop (Multi)")]
  [Category("Flow Controllers/Togglers")]
  [Description("Each time input is signaled, the next output in order is called. After the last output, the order loops from the start.\nReset, resets the current index to zero.")]
  [ContextDefinedOutputs(new System.Type[] {typeof (int)})]
  public class Sequence : FlowControlNode, IMultiPortNode
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [SerializeField]
    private int _portCount = 4;
    private int current;

    public int portCount
    {
      get => this._portCount;
      set => this._portCount = value;
    }

    public override void OnGraphStarted() => this.current = 0;

    public override void OnGraphStoped() => this.current = 0;

    public override string name
    {
      get => base.name + " " + string.Format("[{0}]", (object) this.current.ToString());
    }

    protected override void RegisterPorts()
    {
      List<FlowOutput> outs = new List<FlowOutput>();
      for (int index = 0; index < this.portCount; ++index)
        outs.Add(this.AddFlowOutput(index.ToString()));
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        outs[this.current].Call();
        this.current = (int) Mathf.Repeat((float) (this.current + 1), (float) this.portCount);
      }));
      this.AddFlowInput("Reset", (FlowHandler) (() => this.current = 0));
      this.AddValueOutput<int>("Current", (ValueHandler<int>) (() => this.current));
    }
  }
}
