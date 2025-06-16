using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Engine.Source.Connections;
using Inspectors;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Source.Components.Repairing
{
  [Factory]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class RepairableSettings : EngineObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<RepairableLevel> levels = new List<RepairableLevel>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnitySubAsset<Sprite> nonItemImage;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<AudioClip> repairSound;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected string verbOverride;

    public List<RepairableLevel> Levels => this.levels;

    public Sprite NonItemImage => this.nonItemImage.Value;

    public AudioClip RepairSound => this.repairSound.Value;

    public string VerbOverride => this.verbOverride;
  }
}
