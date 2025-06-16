using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services.Factories;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Commons
{
  [Factory]
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class LipSyncLanguage
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public LanguageEnum Language;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<LipSyncInfo> LipSyncs = new List<LipSyncInfo>();
  }
}
