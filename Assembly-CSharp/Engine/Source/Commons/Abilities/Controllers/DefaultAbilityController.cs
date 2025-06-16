using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons.Abilities.Controllers
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class DefaultAbilityController : IAbilityController
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.EditAndRuntime)]
    protected bool active = true;

    public void Initialise(AbilityItem abilityItem) => abilityItem.Active = this.active;

    public void Shutdown()
    {
    }
  }
}
