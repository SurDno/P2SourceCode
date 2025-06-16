// Decompiled with JetBrains decompiler
// Type: Engine.Common.Components.IStorableComponent
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Types;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Components
{
  public interface IStorableComponent : IComponent
  {
    IStorageComponent Storage { get; }

    IInventoryComponent Container { get; }

    IParameterValue<float> Durability { get; }

    int Max { get; set; }

    int Count { get; set; }

    IEnumerable<StorableGroup> Groups { get; }

    Invoice Invoice { get; set; }

    LocalizedText Description { get; set; }

    LocalizedText Tooltip { get; set; }

    LocalizedText Title { get; set; }

    LocalizedText SpecialDescription { get; set; }
  }
}
