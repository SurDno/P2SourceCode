using Engine.Common;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class RanfomModelNode : FlowControlNode
  {
    [Port("DynamicModels")]
    private ValueInput<IEnumerable<DynamicModelComponent>> componentsInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      IEnumerable<DynamicModelComponent> dynamicModelComponents = this.componentsInput.value;
      if (dynamicModelComponents != null)
      {
        foreach (DynamicModelComponent dynamicModelComponent in dynamicModelComponents)
        {
          if (dynamicModelComponent != null)
          {
            IModel model = dynamicModelComponent.Models.Random<IModel>();
            if (model != null)
              dynamicModelComponent.Model = model;
          }
        }
      }
      this.output.Call();
    }
  }
}
