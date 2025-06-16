// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.IsDiseasedNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsDiseasedNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;
    [Port("Diseased")]
    private ValueInput<DiseasedStateEnum> diseasedInput;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = this.simulation.Player;
      if (player != null)
      {
        INavigationComponent component = player.GetComponent<INavigationComponent>();
        if (component != null && component.Region is RegionComponent region)
          return this.diseasedInput.value == DiseasedUtility.GetStateByLevel(region.DiseaseLevel.Value);
      }
      return false;
    }
  }
}
