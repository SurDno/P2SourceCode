// Decompiled with JetBrains decompiler
// Type: Engine.Source.Components.Saves.TeleportData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Components;
using Engine.Common.Generator;
using UnityEngine;

#nullable disable
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
