// Decompiled with JetBrains decompiler
// Type: StorableTooltipInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;
using UnityEngine;

#nullable disable
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
