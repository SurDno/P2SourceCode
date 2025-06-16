// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Parameters.ListTypedEntityPriorityContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Source.Connections;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Commons.Parameters
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave | TypeEnum.StateLoad, Type = typeof (PriorityContainer<List<Typed<IEntity>>>))]
  public class ListTypedEntityPriorityContainer : PriorityContainer<List<Typed<IEntity>>>
  {
    protected override bool IsDefault(List<Typed<IEntity>> value)
    {
      return value == null || value.Count == 0;
    }
  }
}
