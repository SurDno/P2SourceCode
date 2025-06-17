using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.Commons
{
  [Factory(typeof (ILipSyncObject))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LipSyncObject : EngineObject, ILipSyncObject, IObject
  {
    [DataReadProxy]
    [DataWriteProxy]
    [CopyableProxy()]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<LipSyncLanguage> languages = [];

    public List<LipSyncLanguage> Languages => languages;
  }
}
