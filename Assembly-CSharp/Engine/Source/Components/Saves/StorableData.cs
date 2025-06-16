using Engine.Common.Components;
using Engine.Common.Generator;
using System;

namespace Engine.Source.Components.Saves
{
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class StorableData
  {
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    public IStorageComponent Storage;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    public Guid TemplateId;
  }
}
