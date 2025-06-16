using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class MapBullModeNode : FlowControlNode
  {
    private ValueInput<bool> enabledInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        MapService service = ServiceLocator.GetService<MapService>();
        service.BullModeAvailable = enabledInput.value;
        service.BullModeForced = enabledInput.value;
        output.Call();
      });
      enabledInput = AddValueInput<bool>("Enabled");
    }
  }
}
