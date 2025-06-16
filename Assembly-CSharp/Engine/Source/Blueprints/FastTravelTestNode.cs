using Engine.Common;
using Engine.Common.Components.Regions;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using PLVirtualMachine.Common.EngineAPI;
using System;

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
      GameTime travelTime = new GameTime((ushort) 1, (byte) 0, (byte) 0, (byte) 0);
      if (this.entity != null)
        this.entity.value.GetComponent<FastTravelComponent>()?.FireTravelToPoint(FastTravelPointEnum.Zavodi, (TimeSpan) travelTime);
      this.output.Call();
    }
  }
}
