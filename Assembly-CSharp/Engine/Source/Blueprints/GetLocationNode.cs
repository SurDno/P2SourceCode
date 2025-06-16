// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.GetLocationNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetLocationNode : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.entityInput = this.AddValueInput<IEntity>("Entity");
      this.AddValueOutput<ILocationComponent>("Location", (ValueHandler<ILocationComponent>) (() =>
      {
        IEntity entity = this.entityInput.value;
        if (entity != null)
        {
          ILocationComponent component1 = entity.GetComponent<ILocationComponent>();
          if (component1 != null)
            return component1;
          ILocationItemComponent component2 = entity.GetComponent<ILocationItemComponent>();
          if (component2 != null)
            return component2.LogicLocation;
        }
        return (ILocationComponent) null;
      }));
    }
  }
}
