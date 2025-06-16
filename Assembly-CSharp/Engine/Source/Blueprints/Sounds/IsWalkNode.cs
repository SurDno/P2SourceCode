// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.Sounds.IsWalkNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Components;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints.Sounds
{
  [Category("Sounds")]
  public class IsWalkNode : FlowControlNode
  {
    [FromLocator]
    private ISimulation simulation;

    [Port("Value")]
    private bool Value()
    {
      IEntity player = this.simulation.Player;
      if (player != null)
      {
        ControllerComponent component = player.GetComponent<ControllerComponent>();
        if (component != null)
          return component.IsWalk.Value;
      }
      return false;
    }
  }
}
