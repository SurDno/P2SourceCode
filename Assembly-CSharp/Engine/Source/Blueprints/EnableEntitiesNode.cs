using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EnableEntitiesNode : FlowControlNode
  {
    [Port("Entities")]
    private ValueInput<IEnumerable<IEntity>> entitiesInput;
    [Port("Enable")]
    private ValueInput<bool> enableInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      IEnumerable<IEntity> entities = this.entitiesInput.value;
      if (entities != null)
      {
        foreach (IEntity entity in entities)
          entity.IsEnabled = this.enableInput.value;
      }
      this.output.Call();
    }
  }
}
