using System.Collections.Generic;
using Engine.Common;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      IEnumerable<DynamicModelComponent> dynamicModelComponents = componentsInput.value;
      if (dynamicModelComponents != null)
      {
        foreach (DynamicModelComponent dynamicModelComponent in dynamicModelComponents)
        {
          if (dynamicModelComponent != null)
          {
            IModel model = dynamicModelComponent.Models.Random();
            if (model != null)
              dynamicModelComponent.Model = model;
          }
        }
      }
      output.Call();
    }
  }
}
