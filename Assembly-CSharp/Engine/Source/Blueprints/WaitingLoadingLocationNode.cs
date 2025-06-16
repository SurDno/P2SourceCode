// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.WaitingLoadingLocationNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Blueprints.Utility;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WaitingLoadingLocationNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() => this.StartCoroutine(this.WaitingLoading(output))));
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
            yield return (object) null;
          }
          while (locationItem.IsHibernation);
          output.Call();
        }
      }
    }
  }
}
