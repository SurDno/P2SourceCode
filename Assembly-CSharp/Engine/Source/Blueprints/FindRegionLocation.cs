// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.FindRegionLocation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Locations;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class FindRegionLocation : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.entityInput = this.AddValueInput<IEntity>("Entity");
      this.AddValueOutput<IEntity>("Location", (ValueHandler<IEntity>) (() =>
      {
        for (IEntity parent = this.entityInput.value; parent != null; parent = parent.Parent)
        {
          LocationComponent component = parent.GetComponent<LocationComponent>();
          if (component != null && component.LocationType == LocationType.Region)
            return component.Owner;
        }
        return (IEntity) null;
      }));
    }
  }
}
