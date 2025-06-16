using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Assets.Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EveryHourNode : FlowControlNode, NodeCanvas.Framework.IUpdatable
  {
    private const int distanceMinutes = 5;
    private const float updateTime = 2f;
    private float accomulate;
    [Port("Out")]
    private FlowOutput output;
    private bool activate;

    public void Update()
    {
      this.accomulate += Time.deltaTime;
      if ((double) this.accomulate < 2.0)
        return;
      this.accomulate = 0.0f;
      if (EngineApplication.Sleep)
        return;
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
        return;
      ParametersComponent component = player.GetComponent<ParametersComponent>();
      if (component == null)
        return;
      IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.CanReceiveMail);
      if (byName == null || !byName.Value)
        return;
      if (ServiceLocator.GetService<ITimeService>().SolarTime.Minutes > 5)
      {
        this.activate = false;
      }
      else
      {
        if (this.activate)
          return;
        this.activate = true;
        this.output.Call();
      }
    }
  }
}
