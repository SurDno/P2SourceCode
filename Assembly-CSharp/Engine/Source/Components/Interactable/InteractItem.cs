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
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected InteractType type;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<GameObject> blueprint;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected GameActionType action;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string title;

    public InteractType Type => this.type;

    public UnityAsset<GameObject> Blueprint => this.blueprint;

    public GameActionType Action => this.action;

    public string Title => this.title;
  }
}
