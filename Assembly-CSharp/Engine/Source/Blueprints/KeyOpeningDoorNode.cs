using System.Linq;
using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class KeyOpeningDoorNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> gateInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IDoorComponent door = gateInput.value;
        if (door != null && door.LockState.Value == LockState.Locked)
        {
          IEntity player = ServiceLocator.GetService<ISimulation>().Player;
          if (player != null)
          {
            StorageComponent component = player.GetComponent<StorageComponent>();
            if (component != null && component.Items.FirstOrDefault(o => door.Keys.Select(p => p.Id).Contains(o.Owner.TemplateId)) != null)
              door.LockState.Value = LockState.Unlocked;
          }
        }
        output.Call();
      });
      gateInput = AddValueInput<IDoorComponent>("Door");
    }
  }
}
