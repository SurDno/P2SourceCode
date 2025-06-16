using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class UnlockAchievementNode : FlowControlNode
  {
    private ValueInput<string> nameInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.AddFlowInput("In", (FlowHandler) (() => ServiceLocator.GetService<IAchievementService>().Unlock(this.nameInput.value)));
      this.nameInput = this.AddValueInput<string>("Name");
    }
  }
}
