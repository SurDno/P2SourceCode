using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StorableTooltipInfo
{
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  public StorableTooltipNameEnum Name;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  public string Value;
  [DataReadProxy(MemberEnum.None)]
  [DataWriteProxy(MemberEnum.None)]
  [CopyableProxy(MemberEnum.None)]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  public Color Color = Color.white;
}
