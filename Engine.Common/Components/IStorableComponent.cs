using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;
using Engine.Common.Types;
using System.Collections.Generic;

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
