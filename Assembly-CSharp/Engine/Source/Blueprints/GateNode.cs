// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.GateNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GateNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> gateInput;
    private ValueInput<bool> openInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IDoorComponent doorComponent = this.gateInput.value;
        if (doorComponent != null)
        {
          if (this.openInput.value)
            doorComponent.Opened.Value = true;
          else
            doorComponent.Opened.Value = false;
        }
        output.Call();
      }));
      this.gateInput = this.AddValueInput<IDoorComponent>("Gate");
      this.openInput = this.AddValueInput<bool>("Open");
    }
  }
}
