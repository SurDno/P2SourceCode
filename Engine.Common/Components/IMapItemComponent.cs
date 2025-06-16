using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Maps;
using Engine.Common.MindMap;
using Engine.Common.Types;
using System.Collections.Generic;

namespace Engine.Common.Components
{
  public interface IMapItemComponent : IComponent
  {
    bool IsEnabled { get; set; }

    LocalizedText Title { get; set; }

    LocalizedText Text { get; set; }

    LocalizedText TooltipText { get; set; }

    IMapPlaceholder Resource { get; set; }

    IMapTooltipResource TooltipResource { get; set; }

    bool Discovered { get; set; }

    IParameterValue<BoundHealthStateEnum> BoundHealthState { get; }

    IParameterValue<bool> SavePointIcon { get; }

    IParameterValue<bool> SleepIcon { get; }

    IParameterValue<bool> CraftIcon { get; }

    IParameterValue<bool> StorageIcon { get; }

    IParameterValue<bool> MerchantIcon { get; }

    IEntity BoundCharacter { get; set; }

    IEnumerable<IMMNode> Nodes { get; }

    void AddNode(IMMNode node);

    void RemoveNode(IMMNode node);

    void ClearNodes();
  }
}
