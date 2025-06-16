// Decompiled with JetBrains decompiler
// Type: Engine.Source.Commons.Model
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Common.Generator;
using Engine.Impl.Services.Factories;
using Engine.Source.Connections;
using Inspectors;
using UnityEngine;

#nullable disable
namespace Engine.Source.Commons
{
  [Factory(typeof (IModel))]
  [GenerateProxy(TypeEnum.Copyable | TypeEnum.EngineCloneable | TypeEnum.DataRead | TypeEnum.DataWrite)]
  public class Model : EngineObject, IModel, IObject
  {
    [DataReadProxy(MemberEnum.None)]
    [DataWriteProxy(MemberEnum.None)]
    [CopyableProxy(MemberEnum.None)]
    [Inspected(Name = "Prefab", Mutable = true, Mode = ExecuteMode.Edit)]
    protected UnityAsset<GameObject> connection;

    [Inspected]
    public UnityAsset<GameObject> Connection => this.connection;
  }
}
