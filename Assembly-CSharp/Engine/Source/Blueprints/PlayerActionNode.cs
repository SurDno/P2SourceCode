// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.PlayerActionNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlayerActionNode : FlowControlNode
  {
    private ValueInput<ActionEnum> actionInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<ISimulation>().Player?.GetComponent<PlayerControllerComponent>()?.ComputeAction(this.actionInput.value);
        output.Call();
      }));
      this.actionInput = this.AddValueInput<ActionEnum>("Action");
    }
  }
}
