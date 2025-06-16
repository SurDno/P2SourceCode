using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Commons
{
  [Factory(typeof (ILipSyncObject))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LipSyncObject : EngineObject, ILipSyncObject, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    protected List<LipSyncLanguage> languages = new List<LipSyncLanguage>();

    public List<LipSyncLanguage> Languages => this.languages;
  }
}
