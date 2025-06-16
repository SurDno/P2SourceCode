using System.Collections;
using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Blueprints.Utility;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WaitingLoadingLocationNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", (FlowHandler) (() => StartCoroutine(WaitingLoading(output))));
    }

    private IEnumerator WaitingLoading(FlowOutput output)
    {
      WaitingLoadingUtility.Logs.Clear();
      IEntity player = ServiceLocator.GetService<ISimulation>().Player;
      if (player == null)
      {
        Debug.LogError((object) "player == null");
        output.Call();
      }
      else
      {
        LocationItemComponent locationItem = player.GetComponent<LocationItemComponent>();
        if (locationItem == null)
        {
          Debug.LogError((object) "locationItem == null");
          output.Call();
        }
        else
        {
          do
          {
            yield return null;
          }
          while (locationItem.IsHibernation);
          output.Call();
        }
      }
    }
  }
}
