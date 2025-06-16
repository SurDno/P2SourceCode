using Engine.Common.Components.Interactable;
using Engine.Common.Generator;
using Engine.Source.Connections;
using Engine.Source.Services.Inputs;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components.Interactable
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class InteractItem : IInteractItem
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected(Header = true)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected InteractType type;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<GameObject> blueprint;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected GameActionType action;
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string title;

    public InteractType Type => type;

    public UnityAsset<GameObject> Blueprint => blueprint;

    public GameActionType Action => action;

    public string Title => title;
  }
}
