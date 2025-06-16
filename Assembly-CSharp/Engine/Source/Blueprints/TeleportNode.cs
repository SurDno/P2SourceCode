// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.TeleportNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;
using UnityEngine;

#nullable disable
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
      this.output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        if (this.who.value == null || this.target.value == null)
          return;
        INavigationComponent component = this.who.value.GetComponent<INavigationComponent>();
        if (component == null)
          return;
        if (this.wait)
        {
          Debug.LogWarning((object) (typeof (TeleportNode).ToString() + " is waiting"));
        }
        else
        {
          this.wait = true;
          component.OnTeleport += new Action<INavigationComponent, IEntity>(this.OnTeleport);
          component.TeleportTo(this.target.value);
        }
      }));
      this.who = this.AddValueInput<IEntity>("Who");
      this.target = this.AddValueInput<IEntity>("Target");
    }

    private void OnTeleport(INavigationComponent owner, IEntity target)
    {
      this.wait = false;
      owner.OnTeleport -= new Action<INavigationComponent, IEntity>(this.OnTeleport);
      this.output.Call();
    }
  }
}
