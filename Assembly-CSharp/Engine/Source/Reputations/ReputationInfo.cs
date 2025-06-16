using Engine.Common.Commons;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Components;
using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Reputations
{
  [GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class ReputationInfo
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Header = true)]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public ActionEnum Action;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public List<FractionEnum> Fractions = new List<FractionEnum>();
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public float Visible;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public float Invisible;
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected]
    [Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
    public bool AffectNearRegions;

    [Inspected]
    public void Apply()
    {
      ServiceLocator.GetService<ISimulation>().Player.GetComponent<PlayerControllerComponent>().ComputeAction(this.Action);
    }
  }
}
