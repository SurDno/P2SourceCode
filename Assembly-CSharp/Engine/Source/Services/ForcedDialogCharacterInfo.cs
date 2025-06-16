using Engine.Common;
using Engine.Common.Generator;
using Inspectors;

namespace Engine.Source.Services
{
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class ForcedDialogCharacterInfo
  {
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected]
    public float Distance;
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    [Inspected]
    public IEntity Character;
  }
}
