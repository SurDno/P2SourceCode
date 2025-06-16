// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.OpenLockPickingWindowNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Services.Utilities;
using Engine.Source.UI;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class OpenLockPickingWindowNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> doorInput;
    private FlowOutput output;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.doorInput = this.AddValueInput<IDoorComponent>("Door");
      this.AddFlowInput("In", new FlowHandler(this.Execute));
      this.output = this.AddFlowOutput("Out");
    }

    private void Execute()
    {
      IDoorComponent door = this.doorInput.value;
      if (door != null && door.LockState.Value == LockState.Locked)
      {
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        if (player != null)
        {
          IStorageComponent storage = player.GetComponent<IStorageComponent>();
          if (storage != null)
          {
            UIServiceUtility.PushWindow<ILockPickingWindow>(this.output, (Action<ILockPickingWindow>) (window =>
            {
              window.Actor = storage;
              window.Target = door;
            }));
            return;
          }
        }
      }
      this.output.Call();
    }
  }
}
