using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;
using System.Linq;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class KeyOpeningDoorNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> gateInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IDoorComponent door = this.gateInput.value;
        if (door != null && door.LockState.Value == LockState.Locked)
        {
          IEntity player = ServiceLocator.GetService<ISimulation>().Player;
          if (player != null)
          {
            StorageComponent component = player.GetComponent<StorageComponent>();
            if (component != null && component.Items.FirstOrDefault<IStorableComponent>((Func<IStorableComponent, bool>) (o => door.Keys.Select<IEntity, Guid>((Func<IEntity, Guid>) (p => p.Id)).Contains<Guid>(o.Owner.TemplateId))) != null)
              door.LockState.Value = LockState.Unlocked;
          }
        }
        output.Call();
      }));
      this.gateInput = this.AddValueInput<IDoorComponent>("Door");
    }
  }
}
