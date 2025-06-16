// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.PlayerInteractableComponentUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Interactable;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Source.Components.Interactable;

#nullable disable
namespace Engine.Source.Components
{
  public static class PlayerInteractableComponentUtility
  {
    public static bool GetInteractCriminal(InteractableComponent interactable, InteractItem item)
    {
      if (interactable == null || interactable.Owner == null)
        return false;
      if (item.Type == InteractType.BreakPicklock)
        return !PlayerInteractableComponentUtility.IsFree(interactable.Owner);
      if (item.Type == InteractType.Autopsy)
      {
        IEntity player = ServiceLocator.GetService<ISimulation>().Player;
        if (player == null)
          return false;
        PlayerControllerComponent component = player.GetComponent<PlayerControllerComponent>();
        return component != null && component.IsCrime(ActionEnum.Autopsy, interactable.Owner);
      }
      if (item.Type != InteractType.Loot)
        return false;
      IEntity player1 = ServiceLocator.GetService<ISimulation>().Player;
      if (player1 == null)
        return false;
      PlayerControllerComponent component1 = player1.GetComponent<PlayerControllerComponent>();
      if (component1 == null)
        return false;
      bool flag = interactable?.Owner?.GetComponent<INpcControllerComponent>() != null;
      if (!flag)
      {
        ParametersComponent component2 = interactable?.Owner?.GetComponent<ParametersComponent>();
        if (component2 != null)
        {
          IParameter<bool> byName = component2.GetByName<bool>(ParameterNameEnum.LootAsNPC);
          if (byName != null)
            flag = byName.Value;
        }
      }
      if (!flag)
        return component1.IsCrime(ActionEnum.Theft, interactable.Owner);
      return PlayerInteractableComponentUtility.IsDead(interactable.Owner) ? component1.IsCrime(ActionEnum.LootDeadCharacter, interactable.Owner) : component1.IsCrime(ActionEnum.TakeItemsFromSurrender, interactable.Owner);
    }

    private static bool IsFree(IEntity entity)
    {
      if (entity == null)
        return false;
      ParametersComponent component = entity.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.IsFree);
        if (byName != null)
          return byName.Value;
      }
      return false;
    }

    private static bool IsDead(IEntity entity)
    {
      if (entity == null)
        return false;
      ParametersComponent component = entity.GetComponent<ParametersComponent>();
      if (component != null)
      {
        IParameter<bool> byName = component.GetByName<bool>(ParameterNameEnum.Dead);
        if (byName != null)
          return byName.Value;
      }
      return false;
    }
  }
}
