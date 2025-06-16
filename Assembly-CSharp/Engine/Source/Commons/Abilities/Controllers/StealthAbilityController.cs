using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class StealthAbilityController : IAbilityController
  {
    private IControllerComponent controller;
    private AbilityItem abilityItem;

    public void Initialise(AbilityItem abilityItem)
    {
      this.abilityItem = abilityItem;
      controller = this.abilityItem.Ability.Owner.GetComponent<IControllerComponent>();
      if (controller == null)
        return;
      controller.IsStelth.ChangeValueEvent += OnStelthEnableChanged;
      OnStelthEnableChanged(controller.IsStelth.Value);
    }

    public void Shutdown()
    {
      if (controller == null)
        return;
      controller.IsStelth.ChangeValueEvent -= OnStelthEnableChanged;
      controller = null;
    }

    private void OnStelthEnableChanged(bool enabled) => abilityItem.Active = enabled;
  }
}
