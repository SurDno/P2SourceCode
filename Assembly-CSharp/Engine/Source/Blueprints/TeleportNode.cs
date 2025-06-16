using Engine.Common;
using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class TeleportNode : FlowControlNode
  {
    private ValueInput<IEntity> who;
    private ValueInput<IEntity> target;
    private FlowOutput output;
    private bool wait;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        if (who.value == null || target.value == null)
          return;
        INavigationComponent component = who.value.GetComponent<INavigationComponent>();
        if (component == null)
          return;
        if (wait)
        {
          Debug.LogWarning(typeof (TeleportNode) + " is waiting");
        }
        else
        {
          wait = true;
          component.OnTeleport += OnTeleport;
          component.TeleportTo(target.value);
        }
      });
      who = AddValueInput<IEntity>("Who");
      target = AddValueInput<IEntity>("Target");
    }

    private void OnTeleport(INavigationComponent owner, IEntity target)
    {
      wait = false;
      owner.OnTeleport -= OnTeleport;
      output.Call();
    }
  }
}
