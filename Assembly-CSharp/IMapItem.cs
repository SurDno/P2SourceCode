using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Engine.Common.Components.Regions;
using Engine.Common.MindMap;
using Engine.Common.Types;
using Engine.Source.Components.Maps;
using UnityEngine;

public interface IMapItem
{
  LocalizedText Title { get; }

  LocalizedText Text { get; }

  MapPlaceholder Resource { get; }

  Vector2 WorldPosition { get; }

  float Rotation { get; }

  float Reputation { get; }

  int Disease { get; }

  IEntity BoundCharacter { get; set; }

  IParameterValue<BoundHealthStateEnum> BoundHealthState { get; }

  IParameterValue<bool> SavePointIcon { get; }

  IParameterValue<bool> SleepIcon { get; }

  IParameterValue<bool> CraftIcon { get; }

  IParameterValue<bool> StorageIcon { get; }

  IParameterValue<bool> MerchantIcon { get; }

  IParameterValue<FastTravelPointEnum> FastTravelPoint { get; }

  IEnumerable<IMMNode> Nodes { get; }

  bool Discovered { get; }

  LocalizedText TooltipText { get; set; }

  IMapTooltipResource TooltipResource { get; set; }
}
