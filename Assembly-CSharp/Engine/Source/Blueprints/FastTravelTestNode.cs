using System;
using Engine.Common;
using Engine.Common.Components.Regions;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using PLVirtualMachine.Common.EngineAPI;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class FastTravelTestNode : FlowControlNode
  {
    [Port("Entity")]
    private ValueInput<IEntity> entity;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      GameTime travelTime = new GameTime(1, 0, 0, 0);
      if (entity != null)
        entity.value.GetComponent<FastTravelComponent>()?.FireTravelToPoint(FastTravelPointEnum.Zavodi, (TimeSpan) travelTime);
      output.Call();
    }
  }
}
