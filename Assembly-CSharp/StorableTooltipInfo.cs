using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
public class StorableTooltipInfo
{
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  public StorableTooltipNameEnum Name;
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  public string Value;
  [DataReadProxy]
  [DataWriteProxy]
  [CopyableProxy()]
  [Inspected]
  [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
  public Color Color = Color.white;
}
