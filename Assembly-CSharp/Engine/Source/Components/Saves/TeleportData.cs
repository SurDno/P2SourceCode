using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using UnityEngine;

namespace Engine.Source.Components.Saves
{
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class TeleportData
  {
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    public ILocationComponent Location;
    [StateSaveProxy(MemberEnum.CustomReference)]
    [StateLoadProxy(MemberEnum.CustomReference)]
    public IEntity Target;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    public Vector3 Position;
    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    public Quaternion Rotation;
  }
}
