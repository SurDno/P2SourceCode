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
  public class BreakPicklockDoorNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> doorInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IDoorComponent door = doorInput.value;
        if (door != null && door.LockState.Value == LockState.Locked)
        {
          IEntity player = ServiceLocator.GetService<ISimulation>().Player;
          if (player != null)
          {
            StorageComponent component = player.GetComponent<StorageComponent>();
            if (component != null)
            {
              IStorableComponent storable = component.Items.FirstOrDefault(o => door.Picklocks.Select(p => p.Id).Contains(o.Owner.TemplateId));
              if (storable != null)
              {
                door.LockState.Value = LockState.Unlocked;
                StorableComponentUtility.Use(storable);
              }
            }
          }
        }
        output.Call();
      });
      doorInput = AddValueInput<IDoorComponent>("Door");
    }
  }
}
