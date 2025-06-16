// Decompiled with JetBrains decompiler
// Type: Engine.Source.Behaviours.Yields.WaitPlayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
namespace Engine.Source.Behaviours.Yields
{
  public class WaitPlayer : CustomYieldInstruction
  {
    public override bool keepWaiting
    {
      get
      {
        ISimulation service = ServiceLocator.GetService<ISimulation>();
        if (service == null)
          return true;
        IEntity player = service.Player;
        if (player == null || !((IEntityView) player).IsAttached)
          return true;
        LocationItemComponent component = player.GetComponent<LocationItemComponent>();
        return component != null && component.IsHibernation;
      }
    }
  }
}
