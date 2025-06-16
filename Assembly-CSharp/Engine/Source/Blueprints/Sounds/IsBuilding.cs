// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.IsBuilding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsBuilding : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;
    [Port("Building")]
    private ValueInput<BuildingEnum> buildingInput;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = this.simulation.Player;
      if (player != null)
      {
        INavigationComponent component = player.GetComponent<INavigationComponent>();
        if (component != null)
        {
          IBuildingComponent building = component.Building;
          if (building != null)
            return building.Building == this.buildingInput.value;
        }
      }
      return false;
    }
  }
}
