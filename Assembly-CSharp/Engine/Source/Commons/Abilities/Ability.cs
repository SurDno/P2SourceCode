using System.Collections.Generic;
using Cofe.Meta;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons.Abilities
{
  [Factory(typeof (IAbility))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Ability : EngineObject, IAbility, IObject
  {
    [DataReadProxy(Name = "AbilityItems")]
    [DataWriteProxy(Name = "AbilityItems")]
    [CopyableProxy()]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<AbilityItem> items = new List<AbilityItem>();
    private AbilityCache abilityCache;
    private IEntity owner;

    public AbilityCache AbilityCache => abilityCache;

    [Inspected]
    public List<AbilityItem> AbilityItems => items;

    public IEntity Owner => owner;

    public void Initialise(IEntity owner)
    {
      this.owner = owner;
      abilityCache = new AbilityCache();
      foreach (AbilityItem abilityItem in items)
        abilityItem?.Initialise(this);
    }

    public void Shutdown()
    {
      abilityCache = null;
      int count = items.Count;
      if (count != 0)
      {
        for (int index = count - 1; index >= 0; --index)
          items[index]?.Shutdown();
      }
      owner = null;
    }

    [ComputeBytes]
    private void ComputeBytes()
    {
      foreach (AbilityItem target in items)
      {
        if (target != null)
          MetaService.Compute(target, ComputeBytesAttribute.Id);
      }
    }
  }
}
