using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using IUpdatable = NodeCanvas.Framework.IUpdatable;

namespace Assets.Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EveryHourNode : FlowControlNode, IUpdatable
  {
    private const int distanceMinutes = 5;
    private const float updateTime = 2f;
    private float accomulate;
    [Port("Out")]
    private FlowOutput output;
    private bool activate;

    public void Update()
    {
      accomulate += Time.deltaTime;
      if (accomulate < 2.0)
        return;
      accomulate = 0.0f;
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
        activate = false;
      }
      else
      {
        if (activate)
          return;
        activate = true;
        output.Call();
      }
    }
  }
}
