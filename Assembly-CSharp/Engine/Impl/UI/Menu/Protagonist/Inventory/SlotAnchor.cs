using System;
using Engine.Common.Components.Storable;
using UnityEngine;
using UnityEngine.Serialization;

namespace Engine.Impl.UI.Menu.Protagonist.Inventory
{
  [Serializable]
  public struct SlotAnchor
  {
    [SerializeField]
    public StorableGroup Group;
    [SerializeField]
    [FormerlySerializedAs("UI_Base")]
    public UIControl UIBehaviour;
  }
}
