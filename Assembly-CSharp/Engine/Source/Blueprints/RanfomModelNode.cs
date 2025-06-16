// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.RanfomModelNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

#nullable disable
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
