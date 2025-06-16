using Engine.Common;
using Engine.Common.Components;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;
using UnityEngine;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class TeleportByTransformNode : FlowControlNode
  {
    private ValueInput<IEntity> who;
    private ValueInput<ILocationComponent> targetLocation;
    private ValueInput<Transform> targetTransform;
    private FlowOutput output;
    private bool wait;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if (this.who.value == null)
          Debug.LogError((object) (typeof (TeleportByTransformNode).ToString() + " who.value == null"));
        else if ((UnityEngine.Object) this.targetTransform.value == (UnityEngine.Object) null)
          Debug.LogError((object) (typeof (TeleportByTransformNode).ToString() + " targetTransform.value == null"));
        else if (this.targetLocation.value == null)
        {
          Debug.LogError((object) (typeof (TeleportByTransformNode).ToString() + " targetLocation.value == null"));
        }
        else
        {
          NavigationComponent component = this.who.value.GetComponent<NavigationComponent>();
          if (component == null)
            Debug.LogError((object) (typeof (TeleportByTransformNode).ToString() + " navigation == null"));
          else if (this.wait)
          {
            Debug.LogError((object) (typeof (TeleportByTransformNode).ToString() + " is waiting"));
          }
          else
          {
            this.wait = true;
            component.OnTeleport += new Action<INavigationComponent, IEntity>(this.OnTeleport);
            component.TeleportTo(this.targetLocation.value, this.targetTransform.value.position, this.targetTransform.value.rotation);
          }
        }
      }));
      this.who = this.AddValueInput<IEntity>("Who");
      this.targetLocation = this.AddValueInput<ILocationComponent>("Location");
      this.targetTransform = this.AddValueInput<Transform>("Transform");
    }

    private void OnTeleport(INavigationComponent owner, IEntity target)
    {
      if (!this.wait)
        Debug.LogError((object) "OnTeleport event is not wait");
      this.wait = false;
      owner.OnTeleport -= new Action<INavigationComponent, IEntity>(this.OnTeleport);
      this.output.Call();
    }
  }
}
