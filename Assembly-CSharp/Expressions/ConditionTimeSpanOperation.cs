// Decompiled with JetBrains decompiler
// Type: Expressions.ConditionTimeSpanOperation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Generator;
using Inspectors;
using System;

#nullable disable
namespace Expressions
{
  [TypeName(TypeName = "[condition ? true : false] : Time", MenuItem = "condition ? true : false/Time")]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ConditionTimeSpanOperation : ConditionOperation<TimeSpan>
  {
  }
}
