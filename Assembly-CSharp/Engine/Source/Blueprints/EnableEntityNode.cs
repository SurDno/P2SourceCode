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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IEntity entity = this.entityInput.value;
        if (entity != null)
          entity.IsEnabled = this.enableInput.value;
        output.Call();
      }));
      this.entityInput = this.AddValueInput<IEntity>("Entity");
      this.enableInput = this.AddValueInput<bool>("Enable");
    }
  }
}
