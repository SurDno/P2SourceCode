using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Effects.Values;
using UnityEngine;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class PlagueZoneAbilityController : IAbilityController, IAbilityValueContainer, IUpdatable
  {
    private AbilityItem abilityItem;
    private AbilityValue<float> abilityValue = new AbilityValue<float>();

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      abilityItem.Active = true;
      InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable((IUpdatable) this);
    }

    public void Shutdown()
    {
      InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable((IUpdatable) this);
    }

    public void ComputeUpdate()
    {
      if (((IEntityView) this.abilityItem.Self).IsAttached)
      {
        Vector3 position = ((IEntityView) this.abilityItem.Self).Position;
        this.abilityValue.Value = PlagueZone.GetLevel(new Vector2(position.x, position.z));
      }
      else
        this.abilityValue.Value = 0.0f;
    }

    public IAbilityValue<T> GetAbilityValue<T>(AbilityValueNameEnum parameter) where T : struct
    {
      return parameter == AbilityValueNameEnum.PlagueZoneLevel ? (IAbilityValue<T>) (this.abilityValue as AbilityValue<T>) : (IAbilityValue<T>) null;
    }
  }
}
