using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EnableEntityNode : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;
    private ValueInput<bool> enableInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        IEntity entity = entityInput.value;
        if (entity != null)
          entity.IsEnabled = enableInput.value;
        output.Call();
      });
      entityInput = AddValueInput<IEntity>("Entity");
      enableInput = AddValueInput<bool>("Enable");
    }
  }
}
