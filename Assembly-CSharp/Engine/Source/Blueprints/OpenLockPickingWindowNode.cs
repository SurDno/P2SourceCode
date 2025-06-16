using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class OpenLockPickingWindowNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> doorInput;
    private FlowOutput output;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      doorInput = AddValueInput<IDoorComponent>("Door");
      AddFlowInput("In", Execute);
      output = AddFlowOutput("Out");
    }

    private void Execute()
    {
      IDoorComponent door = doorInput.value;
      if (door != null && door.LockState.Value == LockState.Locked)
      {
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        if (player != null)
        {
          IStorageComponent storage = player.GetComponent<IStorageComponent>();
          if (storage != null)
          {
            UIServiceUtility.PushWindow<ILockPickingWindow>(output, window =>
            {
              window.Actor = storage;
              window.Target = door;
            });
            return;
          }
        }
      }
      output.Call();
    }
  }
}
