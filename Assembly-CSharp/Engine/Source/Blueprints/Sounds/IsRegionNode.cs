// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.IsRegionNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components.Regions;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsRegionNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;
    [Port("Region")]
    private ValueInput<RegionEnum> regionInput;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = this.simulation.Player;
      if (player != null)
      {
        NavigationComponent component = player.GetComponent<NavigationComponent>();
        if (component != null)
        {
          RegionComponent region = (RegionComponent) component.Region;
          if (region != null)
            return region.Region == this.regionInput.value;
        }
      }
      return false;
    }
  }
}
