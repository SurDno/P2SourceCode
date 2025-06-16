using Cofe.Meta;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Commons.Abilities
{
  [Factory(typeof (IAbility))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Ability : EngineObject, IAbility, IObject
  {
    [DataReadProxy(MemberEnum.None, Name = "AbilityItems")]
    [DataWriteProxy(MemberEnum.None, Name = "AbilityItems")]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<AbilityItem> items = new List<AbilityItem>();
    private AbilityCache abilityCache;
    private IEntity owner;

    public AbilityCache AbilityCache => this.abilityCache;

    [Inspected]
    public List<AbilityItem> AbilityItems => this.items;

    public IEntity Owner => this.owner;

    public void Initialise(IEntity owner)
    {
      this.owner = owner;
      this.abilityCache = new AbilityCache();
      foreach (AbilityItem abilityItem in this.items)
        abilityItem?.Initialise(this);
    }

    public void Shutdown()
    {
      this.abilityCache = (AbilityCache) null;
      int count = this.items.Count;
      if (count != 0)
      {
        for (int index = count - 1; index >= 0; --index)
          this.items[index]?.Shutdown();
      }
      this.owner = (IEntity) null;
    }

    [global::ComputeBytes]
    private void ComputeBytes()
    {
      foreach (AbilityItem target in this.items)
      {
        if (target != null)
          MetaService.Compute((object) target, ComputeBytesAttribute.Id);
      }
    }
  }
}
