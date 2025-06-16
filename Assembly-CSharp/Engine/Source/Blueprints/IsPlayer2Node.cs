using Engine.Common;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsPlayer2Node : FlowControlNode
  {
    [Port("Target")]
    private ValueInput<IEntity> inputValue;
    [Port("True")]
    private FlowOutput trueOutput;
    [Port("False")]
    private FlowOutput falseOutput;

    [Port("In")]
    private void In()
    {
      IEntity entity = this.inputValue.value;
      if (entity != null && entity == ServiceLocator.GetService<ISimulation>().Player)
        this.trueOutput.Call();
      else
        this.falseOutput.Call();
    }
  }
}
