// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.RepairerComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Components;
using Engine.Common.Components.Storable;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Engine.Source.Components
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RepairerComponent : EngineComponent
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<StorableGroup> repairableGroups = new List<StorableGroup>();

    public IEnumerable<StorableGroup> RepairableGroups
    {
      get => (IEnumerable<StorableGroup>) this.repairableGroups;
    }

    public bool CanRepairItem(IStorableComponent item)
    {
      foreach (StorableGroup repairableGroup in this.repairableGroups)
      {
        if (item.Groups.Contains<StorableGroup>(repairableGroup))
          return true;
      }
      return false;
    }
  }
}
