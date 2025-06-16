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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        MapService service = ServiceLocator.GetService<MapService>();
        service.BullModeAvailable = this.enabledInput.value;
        service.BullModeForced = this.enabledInput.value;
        output.Call();
      }));
      this.enabledInput = this.AddValueInput<bool>("Enabled");
    }
  }
}
