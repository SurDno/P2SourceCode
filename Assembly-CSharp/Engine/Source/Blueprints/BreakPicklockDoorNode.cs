// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.BreakPicklockDoorNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Gate;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;
using System.Linq;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class BreakPicklockDoorNode : FlowControlNode
  {
    private ValueInput<IDoorComponent> doorInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        IDoorComponent door = this.doorInput.value;
        if (door != null && door.LockState.Value == LockState.Locked)
        {
          IEntity player = ServiceLocator.GetService<ISimulation>().Player;
          if (player != null)
          {
            StorageComponent component = player.GetComponent<StorageComponent>();
            if (component != null)
            {
              IStorableComponent storable = component.Items.FirstOrDefault<IStorableComponent>((Func<IStorableComponent, bool>) (o => door.Picklocks.Select<IEntity, Guid>((Func<IEntity, Guid>) (p => p.Id)).Contains<Guid>(o.Owner.TemplateId)));
              if (storable != null)
              {
                door.LockState.Value = LockState.Unlocked;
                StorableComponentUtility.Use(storable);
              }
            }
          }
        }
        output.Call();
      }));
      this.doorInput = this.AddValueInput<IDoorComponent>("Door");
    }
  }
}
